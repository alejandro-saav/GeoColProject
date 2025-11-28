using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Geo_Col_API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;


namespace Geo_Col_API.Endpoints;

public static class MunicipioEndpoints
{
    public static void MapMunicipioEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/municipios");

        group.MapGet("/", async (GeoDBContext db, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            const string cacheKey = "municipios:all";
            var municipios = await cache.GetOrSetAsync(cacheKey, async () => await db.Municipios.Select(m => new MunicipioDto
            {
                Id = m.Id,
                Municipio = m.Nombre,
                // LinkMapaMunicipio = m.LinkMapaMunicipio
            }).ToListAsync(), TimeSpan.FromHours(24), logger);
            return Results.Ok(municipios);
        });

        group.MapGet("/{id:int}",
            async (GeoDBContext db, int id, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
                var cacheKey = $"municipio:{id}";
                var municipio = await cache.GetOrSetAsync(cacheKey, async () =>
                    {
                        var mun =  await db.Municipios.FindAsync(id);
                        return mun != null ? new MunicipioDto
                        {
                            Id = mun.Id, Municipio = mun.Nombre,
                            // LinkMapaMunicipio = mun.LinkMapaMunicipio 
                        } : null;
                    },
                    TimeSpan.FromHours(25), logger);
                return municipio != null ? Results.Ok(municipio) : Results.NotFound();
            });
        
        group.MapGet("/{id:int}/comunas", async (GeoDBContext db, int id, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var cacheKey = $"municipio:{id}:comunas";
            var comunas = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios.FindAsync(id);
                if (municipio is null) return null;
                var dto = new ComunasPorMunicipio
                {
                    MunicipioId = id,
                    Municipio = municipio.Nombre,
                    Comunas = await db.ComunaBarrios.Where(cm => cm.MunicipioId == id).Select(cm => new ComunasDto
                    {
                        Id = cm.Id,
                        NumeroComuna = cm.NumComuna,
                        NombreComuna = cm.NombreComuna,
                    }).ToListAsync(),
                };
                return dto;
            }, TimeSpan.FromHours(24), logger);
            return comunas != null ? Results.Ok(comunas) : Results.NotFound();
        });

        group.MapGet("/{nombre}/barrios", async (GeoDBContext db, string nombre, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var normalizedNombre = nombre.Trim().ToLowerInvariant();
            var cacheKey = $"municipio:nombre:{normalizedNombre}:barrios";
            var barrios = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios
                    .FirstOrDefaultAsync(m =>
                        EF.Functions.ILike(
                            EF.Functions.Unaccent(m.Nombre),
                            EF.Functions.Unaccent(nombre.Trim())
                        ));
                ;
                if (municipio is null) return null;
                var barrios = await db.ComunaBarrios.Where(cm => cm.MunicipioId == municipio.Id).Select(cm =>
                    new BarrioDto
                    {
                        Id = cm.Id,
                        NombreBarrio = cm.NombreBarrio,
                        NumComuna = cm.NumComuna,
                        NombreComuna = cm.NombreComuna,
                    }).ToListAsync();
                return new BarriosPorMunicipioDto
                {
                    MunicipioId = municipio.Id,
                    Municipio = municipio.Nombre,
                    Barrios = barrios
                };
            }, TimeSpan.FromHours(24), logger);
            return barrios != null ? Results.Ok(barrios) : Results.NotFound();
        });

        group.MapGet("/{id:int}/barrios", async (GeoDBContext db, int id, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var cacheKey = $"municipio:{id}:barrios";
            var barrios = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios.FindAsync(id);
                if (municipio is null) return null;
                var barrios = await db.ComunaBarrios.Where(cm => cm.MunicipioId == id).Select(cm => new BarrioDto
                    {
                        Id = cm.Id,
                        NombreBarrio = cm.NombreBarrio,
                        NombreComuna = cm.NombreComuna,
                        NumComuna = cm.NumComuna,
                    }).ToListAsync();
                return new BarriosPorMunicipioDto
                {
                    MunicipioId = municipio.Id,
                    Municipio = municipio.Nombre,
                    Barrios = barrios
                };
            }, TimeSpan.FromHours(24), logger);
            return barrios != null ? Results.Ok(barrios) : Results.NotFound();
        });

        group.MapGet("/{id:int}/corregimientos", async (GeoDBContext db, int id, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var cacheKey = $"municipio:{id}:corregimientos";
            var corregimientos = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios.FindAsync(id);
                if (municipio is null) return null;
                var corregimientos = await db.Corregimientos.Where(c => c.MunicipioId == municipio.Id).Select(c => new CorregimientoDto
                {
                    Id = c.Id,
                    Corregimiento = c.Nombre,
                }).ToListAsync();

                return new CorregimientosPorMunicipioDto
                {
                    MunicipioId = municipio.Id,
                    Municipio = municipio.Nombre,
                    Corregimientos = corregimientos
                };
            }, TimeSpan.FromHours(24), logger);
            return corregimientos != null ? Results.Ok(corregimientos) : Results.NotFound();
        });

        group.MapGet("/{nombre}/corregimientos", async (GeoDBContext db, string nombre, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var normalizedNombre = nombre.Trim().ToLowerInvariant();
            var cacheKey = $"municipio:nombre:{normalizedNombre}:corregimientos";
            var dto = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios
                    .FirstOrDefaultAsync(m =>
                        EF.Functions.ILike(
                            EF.Functions.Unaccent(m.Nombre),
                            EF.Functions.Unaccent(nombre.Trim())
                        ));
                
                if (municipio == null) return null;

                var result = await db.Municipios
                    .Where(m => m.Id == municipio.Id)
                    .Include(m => m.Corregimientos)
                    .Select(m => new CorregimientosPorMunicipioDto
                    {
                        MunicipioId = municipio.Id,
                        Municipio = m.Nombre,
                        Corregimientos = m.Corregimientos.Select(c => new CorregimientoDto
                        {
                            Id = c.Id,
                            Corregimiento = c.Nombre,
                        }).ToList(),
                    })
                    .FirstOrDefaultAsync();

                return result;
            }, TimeSpan.FromHours(12), logger);
            
            return dto != null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapGet("/{id:int}/veredas", async (GeoDBContext db, int id, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var cacheKey = $"municipio:{id}:veredas";
            var dto = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios.FindAsync(id);
                if (municipio == null) return null;

                var result = await db.Municipios
                    .Where(m => m.Id == id)
                    .Include(m => m.Veredas)
                    .Select(m => new VeredasPorMunicipioDto
                    {
                        MunicipioId = id,
                        Municipio = m.Nombre,
                        Veredas = m.Veredas.Select(v => new VeredaDto
                        {
                            Id = v.Id,
                            Vereda = v.Nombre,
                        }).ToList(),
                    })
                    .FirstOrDefaultAsync();

                return result;
            }, TimeSpan.FromHours(12), logger);
            
            return dto != null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapGet("/{nombre}/veredas", async (GeoDBContext db, string nombre, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var normalizedNombre = nombre.Trim().ToLowerInvariant();
            var cacheKey = $"municipio:nombre:{normalizedNombre}:veredas";
            var dto = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios
                    .FirstOrDefaultAsync(m =>
                        EF.Functions.ILike(
                            EF.Functions.Unaccent(m.Nombre),
                            EF.Functions.Unaccent(nombre.Trim())
                        ));
                
                if (municipio == null) return null;

                var result = await db.Municipios
                    .Where(m => m.Id == municipio.Id)
                    .Include(m => m.Veredas)
                    .Select(m => new VeredasPorMunicipioDto
                    {
                        MunicipioId = municipio.Id,
                        Municipio = m.Nombre,
                        Veredas = m.Veredas.Select(v => new VeredaDto
                        {
                            Id = v.Id,
                            Vereda = v.Nombre,
                        }).ToList(),
                    })
                    .FirstOrDefaultAsync();

                return result;
            }, TimeSpan.FromHours(12), logger);
            
            return dto != null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapGet("/{id:int}/centros_poblados", async (GeoDBContext db, int id, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var cacheKey = $"municipio:{id}:centros_poblados";
            var dto = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios.FindAsync(id);
                if (municipio == null) return null;

                var result = await db.Municipios
                    .Where(m => m.Id == id)
                    .Include(m => m.CentrosPoblados)
                    .ThenInclude(cp => cp.TipoCentroPoblado)
                    .Select(m => new CentrosPobladosPorMunicipioDto
                    {
                        MunicipioId = id,
                        Municipio = m.Nombre,
                        CentrosPoblados = m.CentrosPoblados.Select(cp => new CentroPobladoDto
                        {
                            Id = cp.Id,
                            CentroPoblado = cp.Nombre,
                            Tipo = cp.TipoCentroPoblado.Nombre
                        }).ToList(),
                    })
                    .FirstOrDefaultAsync();

                return result;
            }, TimeSpan.FromHours(12), logger);
            
            return dto != null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapGet("/{nombre}/centros_poblados", async (GeoDBContext db, string nombre, IDistributedCache cache, [FromServices] ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("MunicipioEndpoints");
            var normalizedNombre = nombre.Trim().ToLowerInvariant();
            var cacheKey = $"municipio:nombre:{normalizedNombre}:centros_poblados";
            var dto = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var municipio = await db.Municipios
                    .FirstOrDefaultAsync(m =>
                        EF.Functions.ILike(
                            EF.Functions.Unaccent(m.Nombre),
                            EF.Functions.Unaccent(nombre.Trim())
                        ));
                
                if (municipio == null) return null;

                var result = await db.Municipios
                    .Where(m => m.Id == municipio.Id)
                    .Include(m => m.CentrosPoblados)
                    .ThenInclude(cp => cp.TipoCentroPoblado)
                    .Select(m => new CentrosPobladosPorMunicipioDto
                    {
                        MunicipioId = municipio.Id,
                        Municipio = m.Nombre,
                        CentrosPoblados = m.CentrosPoblados.Select(cp => new CentroPobladoDto
                        {
                            Id = cp.Id,
                            CentroPoblado = cp.Nombre,
                            Tipo = cp.TipoCentroPoblado.Nombre
                        }).ToList(),
                    })
                    .FirstOrDefaultAsync();

                return result;
            }, TimeSpan.FromHours(12), logger);
            
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

                var results = await db.Database.SqlQueryRaw<MunicipioDetailDto>(@"
        SELECT 
            m.id as Id,
            m.nombre as Municipio,
            m.departamento_id as DepartamentoId,
            m.link_mapa_municipio as LinkMapaMunicipio,
            d.nombre as Departamento
        FROM ""municipio"" m
        INNER JOIN ""departamento"" d ON m.departamento_id = d.id
        WHERE
            to_tsvector('spanish', m.""nombre_unaccent"") @@ plainto_tsquery('spanish', immutable_unaccent(@p0))
            OR m.""nombre_unaccent"" ILIKE '%' || immutable_unaccent(@p0) || '%'
            OR similarity(m.""nombre_unaccent"", immutable_unaccent(@p0)) > 0.3
        ORDER BY
            (
                CASE 
                    WHEN m.""nombre_unaccent"" ~ ('(^|\s)' || immutable_unaccent(@p0) || '(\s|$)') THEN 10000.0
                    WHEN m.""nombre_unaccent"" = immutable_unaccent(@p0) THEN 5000.0
                    WHEN m.""nombre_unaccent"" ILIKE immutable_unaccent(@p0) || '%' THEN 1000.0
                    ELSE 0
                END
                +
                (1000.0 * similarity(m.""nombre_unaccent"", immutable_unaccent(@p0)))
                +
                (100.0 * ts_rank_cd(to_tsvector('spanish', m.""nombre_unaccent""),
                                   plainto_tsquery('spanish', immutable_unaccent(@p0))))
            ) DESC
        LIMIT 30
    ", query).ToListAsync();

                return results.Any() ? Results.Ok(results) : Results.NotFound();
            })
            .WithName("SearchMunicipiosHybrid")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Hybrid full-text + trigram + accent-insensitive municipio search.";
                operation.Parameters[0].Description = "Municipio name or partial text to search for.";
                return operation;
            });
    }
}