using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Geo_Col_API.Endpoints;

public static class ComunaBarrioEndpoints
{
    public static void MapComunaBarrioEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("comunas_barrios");
        var barriosGroup = app.MapGroup("barrios");
        var comunasGroup = app.MapGroup("comunas");

        group.MapGet("/", async (GeoDBContext db) => await db.ComunaBarrios.Select(cm => new BarrioDetailDto
        {
            Id = cm.Id,
            NombreBarrio = cm.NombreBarrio,
            NumComuna = cm.NumComuna,
            NombreComuna = cm.NombreComuna,
            MunicipioId = cm.MunicipioId,
            DepartamentoId = cm.DepartamentoId,
        }).ToListAsync());

        group.MapGet("/{id:int}", async (GeoDBContext db, int id) => await db.ComunaBarrios.FindAsync(id) is { } barr ? Results.Ok(new BarrioDetailDto
        {
            Id = barr.Id,
            NombreBarrio = barr.NombreBarrio,
            NumComuna = barr.NumComuna,
            NombreComuna = barr.NombreComuna,
            MunicipioId = barr.MunicipioId,
            DepartamentoId = barr.DepartamentoId
        }) : Results.NotFound());
        
        barriosGroup.MapGet("/search", async (GeoDBContext db, string query) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Results.BadRequest("Query no puede estar vacio.");

                query = query.Trim().ToLower();
                
                if (query.Length > 100)
                    return Results.BadRequest("Query muy extensa, solo se permiten maximo 100 caracteres.");
                
                if (!System.Text.RegularExpressions.Regex.IsMatch(query, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$"))
                    return Results.BadRequest("Query contiene caracteres invalidos, solo se permiten letras, espacios y guiones.");

                var results = await db.Database.SqlQueryRaw<BarrioDto>(@"
        SELECT 
            cm.id as Id,
            cm.nombre_barrio as NombreBarrio,
            cm.num_comuna as NumComuna,
            cm.nombre_comuna as NombreComuna
        FROM ""comuna_barrio"" cm
        WHERE
            to_tsvector('spanish', cm.""barrio_unaccent"") @@ plainto_tsquery('spanish', immutable_unaccent(@p0))
            OR cm.""barrio_unaccent"" ILIKE '%' || immutable_unaccent(@p0) || '%'
            OR similarity(cm.""barrio_unaccent"", immutable_unaccent(@p0)) > 0.3
        ORDER BY
            (
                CASE 
                    WHEN cm.""barrio_unaccent"" ~ ('(^|\s)' || immutable_unaccent(@p0) || '(\s|$)') THEN 10000.0
                    WHEN cm.""barrio_unaccent"" = immutable_unaccent(@p0) THEN 5000.0
                    WHEN cm.""barrio_unaccent"" ILIKE immutable_unaccent(@p0) || '%' THEN 1000.0
                    ELSE 0
                END
                +
                (1000.0 * similarity(cm.""barrio_unaccent"", immutable_unaccent(@p0)))
                +
                (100.0 * ts_rank_cd(to_tsvector('spanish', cm.""barrio_unaccent""),
                                   plainto_tsquery('spanish', immutable_unaccent(@p0))))
            ) DESC
        LIMIT 30
    ", query).ToListAsync();

                return results.Any() ? Results.Ok(results) : Results.NotFound();
            })
            .WithName("SearchBarriosHybrid")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Hybrid full-text + trigram + accent-insensitive barrio search.";
                operation.Parameters[0].Description = "Barrio name or partial text to search for.";
                return operation;
            });

        // comunasGroup.MapGet("/comunas", async (GeoDBContext db) =>
        // {
        //     await db.ComunaBarrios.
        // } );
    }
}