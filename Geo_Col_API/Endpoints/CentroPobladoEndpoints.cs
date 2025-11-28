using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Geo_Col_API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Geo_Col_API.Endpoints;

public static class CentroPobladoEndpoints
{
    public static void MapCentroPobladoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/centros_poblados");

        group.MapGet("/", async (GeoDBContext db, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("CentroPobladoEndpoints");
            const string cacheKey = "centros_poblados:all";
            var centrosPoblados = await cache.GetOrSetAsync(
                cacheKey,
                async () => await db.CentrosPoblados
                    .Include(cp => cp.TipoCentroPoblado)
                    .Include(cp => cp.Municipio)
                    .Select(cp => new CentrosPobladosDetailDto
                    {
                        Id = cp.Id,
                        CentroPoblado = cp.Nombre,
                        TipoCentroPoblado = cp.TipoCentroPoblado.Nombre,
                        MunicipioId = cp.MunicipioId,
                        Municipio = cp.Municipio.Nombre,
                    })
                    .ToListAsync(),
                TimeSpan.FromHours(24),
                logger
            );
            return Results.Ok(centrosPoblados);
        });
        
        group.MapGet("/{id:int}", async (GeoDBContext db, int id) => await db.CentrosPoblados.FindAsync(id) is { } cp ? Results.Ok(new CentrosPobladosDetailDto
        {
            Id = cp.Id,
            CentroPoblado = cp.Nombre,
            TipoCentroPoblado = cp.TipoCentroPoblado.Nombre,
            MunicipioId = cp.MunicipioId,
            Municipio = cp.Municipio.Nombre,
        }) : Results.NotFound());
        
        group.MapGet("/search", async (GeoDBContext db, string query) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Results.BadRequest("Query no puede estar vacio.");

                query = query.Trim().ToLower();
                
                if (query.Length > 100)
                    return Results.BadRequest("Query muy extensa, solo se permiten maximo 100 caracteres.");
                
                if (!System.Text.RegularExpressions.Regex.IsMatch(query, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$"))
                    return Results.BadRequest("Query contiene caracteres invalidos, solo se permiten letras, espacios y guiones.");

                var results = await db.Database.SqlQueryRaw<CentrosPobladosDetailDto>(@"
        SELECT 
            cp.id as Id,
            cp.nombre as CentroPoblado,
            t.nombre as TipoCentroPoblado,
            m.id as MunicipioId,
            m.nombre as Municipio
        FROM ""centros_poblados"" cp
        INNER JOIN ""municipio"" m ON cp.municipio_id = m.id
        INNER JOIN ""tipo_centro_poblado"" t ON cp.tipo = t.id
        WHERE
            to_tsvector('spanish', cp.""nombre_unaccent"") @@ plainto_tsquery('spanish', immutable_unaccent(@p0))
            OR cp.""nombre_unaccent"" ILIKE '%' || immutable_unaccent(@p0) || '%'
            OR similarity(cp.""nombre_unaccent"", immutable_unaccent(@p0)) > 0.3
        ORDER BY
            (
                CASE 
                    WHEN cp.""nombre_unaccent"" ~ ('(^|\s)' || immutable_unaccent(@p0) || '(\s|$)') THEN 10000.0
                    WHEN cp.""nombre_unaccent"" = immutable_unaccent(@p0) THEN 5000.0
                    WHEN cp.""nombre_unaccent"" ILIKE immutable_unaccent(@p0) || '%' THEN 1000.0
                    ELSE 0
                END
                +
                (1000.0 * similarity(cp.""nombre_unaccent"", immutable_unaccent(@p0)))
                +
                (100.0 * ts_rank_cd(to_tsvector('spanish', cp.""nombre_unaccent""),
                                   plainto_tsquery('spanish', immutable_unaccent(@p0))))
            ) DESC
        LIMIT 30
    ", query).ToListAsync();

                return results.Any() ? Results.Ok(results) : Results.NotFound();
            })
            .WithName("SearchCentrosPobladosHybrid")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Hybrid full-text + trigram + accent-insensitive centros poblados search.";
                operation.Parameters[0].Description = "Centros poblados name or partial text to search for.";
                return operation;
            });
    }
}