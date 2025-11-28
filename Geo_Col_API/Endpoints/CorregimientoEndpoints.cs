using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Geo_Col_API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Geo_Col_API.Endpoints;

public static class CorregimientoEndpoints
{
    public static void MapCorregimientoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("corregimientos");
        group.MapGet("/", async (GeoDBContext db, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("CorregimientoEndpoints");
            const string cacheKey = "corregimientos:all";
            var corregimientos = await cache.GetOrSetAsync(
                cacheKey,
                async () => await db.Corregimientos
                    .Include(c => c.Municipio)
                    .Select(c => new CorregimientoDetailDto
                    {
                        Id = c.Id,
                        Corregimiento = c.Nombre,
                        MunicipioId = c.MunicipioId,
                        Municipio = c.Municipio.Nombre
                    })
                    .ToListAsync(),
                TimeSpan.FromHours(24),
                logger
            );
            return Results.Ok(corregimientos);
        });
        
        group.MapGet("/search", async (GeoDBContext db, string query) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Results.BadRequest("Query no puede estar vacio.");

                query = query.Trim().ToLower();
                
                if (query.Length > 100)
                    return Results.BadRequest("Query muy extensa, solo se permiten maximo 100 caracteres.");
                
                if (!System.Text.RegularExpressions.Regex.IsMatch(query, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$"))
                    return Results.BadRequest("Query contiene caracteres invalidos, solo se permiten letras, espacios y guiones.");

                var results = await db.Database.SqlQueryRaw<CorregimientoDetailDto>(@"
        SELECT 
            c.id as Id,
            c.nombre as Corregimiento,
            m.id as MunicipioId,
            m.nombre as Municipio
        FROM ""corregimiento"" c
        INNER JOIN ""municipio"" m ON c.municipio_id = m.id
        WHERE
            to_tsvector('spanish', c.""nombre_unaccent"") @@ plainto_tsquery('spanish', immutable_unaccent(@p0))
            OR c.""nombre_unaccent"" ILIKE '%' || immutable_unaccent(@p0) || '%'
            OR similarity(c.""nombre_unaccent"", immutable_unaccent(@p0)) > 0.3
        ORDER BY
            (
                CASE 
                    WHEN c.""nombre_unaccent"" ~ ('(^|\s)' || immutable_unaccent(@p0) || '(\s|$)') THEN 10000.0
                    WHEN c.""nombre_unaccent"" = immutable_unaccent(@p0) THEN 5000.0
                    WHEN c.""nombre_unaccent"" ILIKE immutable_unaccent(@p0) || '%' THEN 1000.0
                    ELSE 0
                END
                +
                (1000.0 * similarity(c.""nombre_unaccent"", immutable_unaccent(@p0)))
                +
                (100.0 * ts_rank_cd(to_tsvector('spanish', c.""nombre_unaccent""),
                                   plainto_tsquery('spanish', immutable_unaccent(@p0))))
            ) DESC
        LIMIT 30
    ", query).ToListAsync();

                return results.Any() ? Results.Ok(results) : Results.NotFound();
            })
            .WithName("SearchCorregimientoHybrid")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Hybrid full-text + trigram + accent-insensitive corregimiento search.";
                operation.Parameters[0].Description = "Corregimiento name or partial text to search for.";
                return operation;
            });

    }
}