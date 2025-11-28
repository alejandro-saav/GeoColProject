using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Geo_Col_API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Geo_Col_API.Endpoints;

public static class VeredaEndpoints
{
    public static void MapVeredaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/veredas");

        group.MapGet("/", async (GeoDBContext db, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("VeredaEndpoints");
            const string cacheKey = "veredas:all";
            var veredas = await cache.GetOrSetAsync(
                cacheKey,
                async () => await db.Veredas
                    .Include(v => v.Corregimiento)
                    .Include(m => m.Municipio)
                    .Select(v => new VeredaDetailDto
                    {
                        Id = v.Id,
                        Vereda = v.Nombre,
                        MunicipioId = v.Municipio.Id,
                        Municipio = v.Municipio.Nombre,
                        CorregimientoId = v.CorregimientoId,
                        Corregimiento = v.Corregimiento.Nombre,
                    })
                    .ToListAsync(),
                TimeSpan.FromHours(24),
                logger
            );
            return Results.Ok(veredas);
        });

        group.MapGet("/{id:int}", async (GeoDBContext db, int id) => await db.Veredas.Where(v => v.Id == id).Include(v => v.Corregimiento).Include(v => v.Municipio).Select(v => new VeredaDetailDto
        {
            Id = v.Id,
            Vereda = v.Nombre,
            MunicipioId = v.Municipio.Id,
            Municipio = v.Municipio.Nombre,
            CorregimientoId = v.CorregimientoId,
            Corregimiento = v.Corregimiento.Nombre,
        }).ToListAsync());
        
        group.MapGet("/search", async (GeoDBContext db, string query) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Results.BadRequest("Query no puede estar vacio.");

                query = query.Trim().ToLower();
                
                if (query.Length > 100)
                    return Results.BadRequest("Query muy extensa, solo se permiten maximo 100 caracteres.");
                
                if (!System.Text.RegularExpressions.Regex.IsMatch(query, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$"))
                    return Results.BadRequest("Query contiene caracteres invalidos, solo se permiten letras, espacios y guiones.");

                var results = await db.Database.SqlQueryRaw<VeredaDetailDto>(@"
        SELECT 
            v.id as Id,
            v.nombre as Vereda,
            v.municipio_id as MunicipioId,
            v.corregimiento_id as CorregimientoId,
            m.nombre as Municipio,
            c.nombre as Corregimiento
        FROM ""vereda"" v
        INNER JOIN ""municipio"" m ON v.municipio_id = m.id
        INNER JOIN ""corregimiento"" c ON v.corregimiento_id = c.id
        WHERE
            to_tsvector('spanish', v.""nombre_unaccent"") @@ plainto_tsquery('spanish', immutable_unaccent(@p0))
            OR v.""nombre_unaccent"" ILIKE '%' || immutable_unaccent(@p0) || '%'
            OR similarity(v.""nombre_unaccent"", immutable_unaccent(@p0)) > 0.3
        ORDER BY
            (
                CASE 
                    WHEN v.""nombre_unaccent"" ~ ('(^|\s)' || immutable_unaccent(@p0) || '(\s|$)') THEN 10000.0
                    WHEN v.""nombre_unaccent"" = immutable_unaccent(@p0) THEN 5000.0
                    WHEN v.""nombre_unaccent"" ILIKE immutable_unaccent(@p0) || '%' THEN 1000.0
                    ELSE 0
                END
                +
                (1000.0 * similarity(v.""nombre_unaccent"", immutable_unaccent(@p0)))
                +
                (100.0 * ts_rank_cd(to_tsvector('spanish', v.""nombre_unaccent""),
                                   plainto_tsquery('spanish', immutable_unaccent(@p0))))
            ) DESC
        LIMIT 30
    ", query).ToListAsync();

                return results.Any() ? Results.Ok(results) : Results.NotFound();
            })
            .WithName("SearchVeredaHybrid")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Hybrid full-text + trigram + accent-insensitive vereda search.";
                operation.Parameters[0].Description = "Vereda name or partial text to search for.";
                return operation;
            });
    }
}