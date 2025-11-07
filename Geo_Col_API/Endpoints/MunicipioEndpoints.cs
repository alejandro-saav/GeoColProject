using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Microsoft.EntityFrameworkCore;


namespace Geo_Col_API.Endpoints;

public static class MunicipioEndpoints
{
    public static void MapMunicipioEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/municipios");

        group.MapGet("/", async (GeoDBContext db) => await db.Municipios.Select(m => new MunicipioDto {
            Id = m.Id,
            Municipio = m.Nombre,
            LinkMapaMunicipio = m.LinkMapaMunicipio
            }).ToListAsync());

        group.MapGet("/{id:int}",
            async (GeoDBContext db, int id) => await db.Municipios.FindAsync(id) is { } municipio
                ? Results.Ok(new MunicipioDto
                {
                    Id = municipio.Id, Municipio = municipio.Nombre, LinkMapaMunicipio = municipio.LinkMapaMunicipio
                })
                : Results.NotFound());
        
        group.MapGet("/{id:int}/comunas", async (GeoDBContext db, int id) =>
        {
            var municipio = await db.Municipios.FindAsync(id);
            if (municipio is null) return Results.NotFound();
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
            return Results.Ok(dto);
        });

        group.MapGet("/{nombre}/barrios", async (GeoDBContext db, string nombre) =>
        {
            var municipio = await db.Municipios
                .FirstOrDefaultAsync(m =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(m.Nombre),
                        EF.Functions.Unaccent(nombre.Trim())
                    ));
            ;
            if (municipio is null) return Results.NotFound();
            var dto = new ComunasPorMunicipio
            {
                MunicipioId = municipio.Id,
                Municipio = municipio.Nombre,
                Comunas = await db.ComunaBarrios.Where(cm => cm.MunicipioId == municipio.Id).Select(cm => new ComunasDto
                {
                    Id = cm.Id,
                    NumeroComuna = cm.NumComuna,
                    NombreComuna = cm.NombreComuna,
                }).ToListAsync(),
            };
            return Results.Ok(dto);
        });

        group.MapGet("/{id:int}/barrios", async (GeoDBContext db, int id) =>
        {
            var municipio = await db.Municipios.FindAsync(id);
            if (municipio is null) return Results.NotFound();
            var dto = new BarriosPorMunicipioDto
            {
                MunicipioId = id,
                Municipio = municipio.Nombre,
                Barrios = await db.ComunaBarrios.Where(cm => cm.MunicipioId == id).Select(cm => new BarrioDto
                {
                    Id = cm.Id,
                    NombreBarrio = cm.NombreBarrio,
                    NombreComuna = cm.NombreComuna,
                    NumComuna = cm.NumComuna,
                }).ToListAsync(),
            };
            return Results.Ok(dto);
        });

        group.MapGet("/{nombre}/barrios", async (GeoDBContext db, string nombre) =>
        {
            var municipio = await db.Municipios
                .FirstOrDefaultAsync(m =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(m.Nombre),
                        EF.Functions.Unaccent(nombre.Trim())
                    ));
            ;
            if (municipio is null) return Results.NotFound();
            var dto = new BarriosPorMunicipioDto
            {
                MunicipioId = municipio.Id,
                Municipio = municipio.Nombre,
                Barrios = await db.ComunaBarrios.Where(cm => cm.MunicipioId == municipio.Id).Select(cm => new BarrioDto
                {
                    Id = cm.Id,
                    NombreBarrio = cm.NombreBarrio,
                    NombreComuna = cm.NombreComuna,
                    NumComuna = cm.NumComuna,
                }).ToListAsync(),
            };
            return Results.Ok(dto);
        });

        group.MapGet("/{id:int}/corregimientos", async (GeoDBContext db, int id) =>
        {
            var municipio = await db.Municipios.FindAsync(id);
            if (municipio is null) return Results.NotFound();
            var dto = await db.Municipios.Where(m => m.Id == id).Include(m => m.Corregimientos).Select(m =>
                new CorregimientosPorMunicipioDto
                {
                    MunicipioId = id,
                    Municipio = m.Nombre,
                    Corregimientos = m.Corregimientos.Select(c => new CorregimientoDto
                    {
                        Id = c.Id,
                        Corregimiento = c.Nombre,
                    }).ToList(),
                }).ToListAsync();
            return Results.Ok(dto);
        });

        group.MapGet("/{nombre}/corregimientos", async (GeoDBContext db, string nombre) =>
        {
            var municipio = await db.Municipios
                .FirstOrDefaultAsync(m =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(m.Nombre),
                        EF.Functions.Unaccent(nombre.Trim())
                    ));
            ;
            ;
            if (municipio is null) return Results.NotFound();
            var dto = await db.Municipios.Where(m => m.Id == municipio.Id).Include(m => m.Corregimientos).Select(m =>
                new CorregimientosPorMunicipioDto
                {
                    MunicipioId = municipio.Id,
                    Municipio = m.Nombre,
                    Corregimientos = m.Corregimientos.Select(c => new CorregimientoDto
                    {
                        Id = c.Id,
                        Corregimiento = c.Nombre,
                    }).ToList(),
                }).ToListAsync();
            return Results.Ok(dto);
        });

        group.MapGet("/{id:int}/veredas", async (GeoDBContext db, int id) =>
        {
            var municipio = await db.Municipios.FindAsync(id);
            if (municipio is null) return Results.NotFound();
            var dto = await db.Municipios.Where(m => m.Id == id).Include(m => m.Veredas).Select(m =>
                new VeredasPorMunicipioDto
                {
                    MunicipioId = id,
                    Municipio = m.Nombre,
                    Veredas = m.Veredas.Select(v => new VeredaDto
                    {
                        Id = v.Id,
                        Vereda = v.Nombre,
                    }).ToList(),
                }).ToListAsync();
            return Results.Ok(dto);
        });

        group.MapGet("/{nombre}/veredas", async (GeoDBContext db, string nombre) =>
        {
            var municipio = await db.Municipios
                .FirstOrDefaultAsync(m =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(m.Nombre),
                        EF.Functions.Unaccent(nombre.Trim())
                    ));
            ;
            ;
            if (municipio is null) return Results.NotFound();
            var dto = await db.Municipios.Where(m => m.Id == municipio.Id).Include(m => m.Veredas).Select(m =>
                new VeredasPorMunicipioDto
                {
                    MunicipioId = municipio.Id,
                    Municipio = m.Nombre,
                    Veredas = m.Veredas.Select(v => new VeredaDto
                    {
                        Id = v.Id,
                        Vereda = v.Nombre,
                    }).ToList(),
                }).ToListAsync();
            return Results.Ok(dto);
        });

        group.MapGet("/{id:int}/centros_poblados", async (GeoDBContext db, int id) =>
        {
            var municipio = await db.Municipios.FindAsync(id);
            if (municipio is null) return Results.NotFound();
            var dto = await db.Municipios.Where(m => m.Id == id).Include(m => m.CentrosPoblados).Select(m =>
                new CentrosPobladosPorMunicipioDto
                {
                    MunicipioId = id,
                    Municipio = m.Nombre,
                    CentrosPoblados = m.CentrosPoblados.Select(cp => new CentroPobladoDto
                    {
                        Id = cp.Id,
                        CentroPoblado = cp.Nombre,
                        Tipo = cp.TipoCentroPoblado.Nombre
                    }).ToList(),
                }).ToListAsync();
            return Results.Ok(dto);
        });

        group.MapGet("/{nombre}/centros_poblados", async (GeoDBContext db, string nombre) =>
        {
            var municipio = await db.Municipios
                .FirstOrDefaultAsync(m =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(m.Nombre),
                        EF.Functions.Unaccent(nombre.Trim())
                    ));
            ;
            ;
            if (municipio is null) return Results.NotFound();
            var dto = await db.Municipios.Where(m => m.Id == municipio.Id).Include(m => m.CentrosPoblados).Select(m =>
                new CentrosPobladosPorMunicipioDto
                {
                    MunicipioId = municipio.Id,
                    Municipio = m.Nombre,
                    CentrosPoblados = m.CentrosPoblados.Select(cp => new CentroPobladoDto
                    {
                        Id = cp.Id,
                        CentroPoblado = cp.Nombre,
                        Tipo = cp.TipoCentroPoblado.Nombre
                    }).ToList(),
                }).ToListAsync();
            return Results.Ok(dto);
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