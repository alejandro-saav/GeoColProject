using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Geo_Col_API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Geo_Col_API.Endpoints;

public static class DepartamentoEndpoints
{
    public static void MapDepartamentoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/departamentos");

        group.MapGet("/", async (GeoDBContext db, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("DepartamentoEndpoints");
            const string cacheKey = "departamentos:all";
            var departamentos = await cache.GetOrSetAsync(
                cacheKey,
                async () => await db.Departamentos.Select(dp => new DepartamentosDto
                {
                    Id = dp.Id,
                    Departamento = dp.Nombre,
                }).ToListAsync(),
                TimeSpan.FromHours(24), logger // Cache for 24 hours
            );
            return Results.Ok(departamentos);
        });

        group.MapGet("/{id:int}",
            async (GeoDBContext db, IDistributedCache cache, int id, [FromServices] ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("DepartamentoEndpoints");
                var cacheKey = $"departamento:{id}";
                var departamento = await cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var dep = await db.Departamentos.FindAsync(id);
                        return dep != null ? new DepartamentosDto { Id = dep.Id, Departamento = dep.Nombre } : null;
                    },
                    TimeSpan.FromHours(24), logger
                );

                return departamento != null ? Results.Ok(departamento) : Results.NotFound();
            });

        group.MapGet("/{id:int}/municipios", async (GeoDBContext db, IDistributedCache cache, int id, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("DepartamentoEndpoints");
            var cacheKey = $"departamento:{id}:municipios";
            var dto = await cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var departamentoNombre = await db.Departamentos
                        .Where(d => d.Id == id)
                        .Select(d => d.Nombre)
                        .FirstOrDefaultAsync();

                    if (departamentoNombre == null) return null;

                    var municipios = await db.Municipios
                        .Where(m => m.DepartamentoId == id)
                        .Select(m => new MunicipioDto
                        {
                            Id = m.Id,
                            Municipio = m.Nombre,
                            // LinkMapaMunicipio = m.LinkMapaMunicipio
                        }).ToListAsync();

                    return new MunicipiosPorDepartamentoDto
                    {
                        DepartamentoId = id,
                        Departamento = departamentoNombre,
                        Municipios = municipios
                    };
                },
                TimeSpan.FromHours(12), logger // Cache for 12 hours
            );

            return dto != null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapGet("/{nombre}/municipios", async (GeoDBContext db, IDistributedCache cache, string nombre, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("DepartamentoEndpoints");
            var normalizedNombre = nombre.Trim().ToLowerInvariant();
            var cacheKey = $"departamento:nombre:{normalizedNombre}:municipios";
            
            var dto = await cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var departamento = await db.Departamentos
                        .FirstOrDefaultAsync(d =>
                            EF.Functions.ILike(
                                EF.Functions.Unaccent(d.Nombre),
                                EF.Functions.Unaccent(nombre.Trim())
                            ));
                    
                    if (departamento == null) return null;

                    var municipios = await db.Municipios
                        .Where(m => m.DepartamentoId == departamento.Id)
                        .Select(m => new MunicipioDto
                        {
                            Id = m.Id,
                            Municipio = m.Nombre,
                            // LinkMapaMunicipio = m.LinkMapaMunicipio
                        }).ToListAsync();

                    return new MunicipiosPorDepartamentoDto
                    {
                        DepartamentoId = departamento.Id,
                        Departamento = departamento.Nombre,
                        Municipios = municipios
                    };
                },
                TimeSpan.FromHours(12), logger
            );

            return dto != null ? Results.Ok(dto) : Results.NotFound();
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

                var results = await db.Database.SqlQueryRaw<DepartamentosDto>(@"
        SELECT 
            d.id as Id,
            d.nombre as Departamento
        FROM ""departamento"" d
        WHERE
            to_tsvector('spanish', d.""nombre_unaccent"") @@ plainto_tsquery('spanish', immutable_unaccent(@p0))
            OR d.""nombre_unaccent"" ILIKE '%' || immutable_unaccent(@p0) || '%'
            OR similarity(d.""nombre_unaccent"", immutable_unaccent(@p0)) > 0.3
        ORDER BY
            (
                CASE 
                    WHEN d.""nombre_unaccent"" ~ ('(^|\s)' || immutable_unaccent(@p0) || '(\s|$)') THEN 10000.0
                    WHEN d.""nombre_unaccent"" = immutable_unaccent(@p0) THEN 5000.0
                    WHEN d.""nombre_unaccent"" ILIKE immutable_unaccent(@p0) || '%' THEN 1000.0
                    ELSE 0
                END
                +
                (1000.0 * similarity(d.""nombre_unaccent"", immutable_unaccent(@p0)))
                +
                (100.0 * ts_rank_cd(to_tsvector('spanish', d.""nombre_unaccent""),
                                   plainto_tsquery('spanish', immutable_unaccent(@p0))))
            ) DESC
        LIMIT 30
    ", query).ToListAsync();

                return results.Any() ? Results.Ok(results) : Results.NotFound();
            })
            .WithName("SearchDepartamentosHybrid")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Hybrid full-text + trigram + accent-insensitive departamento search.";
                operation.Parameters[0].Description = "Departamento name or partial text to search for.";
                return operation;
            });
    }
}