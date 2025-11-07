using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;

namespace Geo_Col_API.Endpoints;

public static class ExportEndpoints
{
    public static void MapExportEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/export");

        // ====================================================================
        // CSV Exports - Streaming (Best for large datasets)
        // ====================================================================
        
        group.MapGet("/departamentos.csv", async (GeoDBContext db, HttpResponse response) =>
        {
            response.ContentType = "text/csv; charset=utf-8";
            response.Headers.Append("Content-Disposition", $"attachment; filename=departamentos_{DateTime.UtcNow:yyyyMMdd}.csv");

            // UTF-8 with BOM for Excel compatibility with accents
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(true));
            
            // Write CSV header
            await writer.WriteLineAsync("Id,Departamento");
            await writer.FlushAsync();

            // Stream data in batches to avoid loading everything into memory
            const int batchSize = 1000;
            int skip = 0;
            
            while (true)
            {
                var batch = await db.Departamentos
                    .OrderBy(d => d.Id)
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(d => new { d.Id, d.Nombre })
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    // Escape CSV values (handle commas, quotes, newlines)
                    var nombre = EscapeCsvValue(item.Nombre);
                    await writer.WriteLineAsync($"{item.Id},{nombre}");
                }
                
                await writer.FlushAsync();
                skip += batchSize;
            }
        });

        group.MapGet("/municipios.csv", async (GeoDBContext db, HttpResponse response, int? departamentoId = null) =>
        {
            response.ContentType = "text/csv; charset=utf-8";
            var filename = departamentoId.HasValue 
                ? $"municipios_dep_{departamentoId}_{DateTime.UtcNow:yyyyMMdd}.csv"
                : $"municipios_{DateTime.UtcNow:yyyyMMdd}.csv";
            response.Headers.Append("Content-Disposition", $"attachment; filename={filename}");

            // UTF-8 with BOM for Excel compatibility with accents
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(true));
            await writer.WriteLineAsync("Id,Municipio,DepartamentoId,LinkMapaMunicipio");
            await writer.FlushAsync();

            var query = db.Municipios.Include(m => m.Departamento).AsQueryable();
            if (departamentoId.HasValue)
            {
                query = query.Where(m => m.DepartamentoId == departamentoId.Value);
            }

            const int batchSize = 1000;
            int skip = 0;

            while (true)
            {
                var batch = await query
                    .OrderBy(m => m.Id)
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(m => new { m.Id, m.Nombre, m.DepartamentoId, m.LinkMapaMunicipio })
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    var municipio = EscapeCsvValue(item.Nombre);
                    var linkMapa = EscapeCsvValue(item.LinkMapaMunicipio ?? "");
                    await writer.WriteLineAsync($"{item.Id},{municipio},{item.DepartamentoId},{linkMapa}");
                }

                await writer.FlushAsync();
                skip += batchSize;
            }
        });

        group.MapGet("/barrios.csv", async (GeoDBContext db, HttpResponse response, int? municipioId = null) =>
        {
            response.ContentType = "text/csv; charset=utf-8";
            var filename = municipioId.HasValue
                ? $"barrios_mun_{municipioId}_{DateTime.UtcNow:yyyyMMdd}.csv"
                : $"barrios_{DateTime.UtcNow:yyyyMMdd}.csv";
            response.Headers.Append("Content-Disposition", $"attachment; filename={filename}");

            // UTF-8 with BOM for Excel compatibility with accents
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(true));
            await writer.WriteLineAsync("Id,NombreBarrio,NumComuna,NombreComuna,MunicipioId,DepartamentoId");
            await writer.FlushAsync();

            var query = db.ComunaBarrios.AsQueryable();
            if (municipioId.HasValue)
            {
                query = query.Where(cb => cb.MunicipioId == municipioId.Value);
            }

            const int batchSize = 1000;
            int skip = 0;

            while (true)
            {
                var batch = await query
                    .OrderBy(cb => cb.Id)
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(cb => new { cb.Id, cb.NombreBarrio, cb.NumComuna, cb.NombreComuna, cb.MunicipioId, cb.DepartamentoId })
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    var barrio = EscapeCsvValue(item.NombreBarrio);
                    var comuna = EscapeCsvValue(item.NombreComuna);
                    await writer.WriteLineAsync($"{item.Id},{barrio},{item.NumComuna},{comuna},{item.MunicipioId},{item.DepartamentoId}");
                }

                await writer.FlushAsync();
                skip += batchSize;
            }
        });

        // ====================================================================
        // JSON Exports - Streaming (Best for large datasets)
        // ====================================================================

        group.MapGet("/departamentos.json", async (GeoDBContext db, HttpResponse response) =>
        {
            response.ContentType = "application/json";
            response.Headers.Append("Content-Disposition", $"attachment; filename=departamentos_{DateTime.UtcNow:yyyyMMdd}.json");

            await response.WriteAsync("[\n");
            
            var departamentos = await db.Departamentos
                .OrderBy(d => d.Id)
                .Select(d => new DepartamentosDto { Id = d.Id, Departamento = d.Nombre })
                .ToListAsync();

            for (int i = 0; i < departamentos.Count; i++)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(departamentos[i]);
                await response.WriteAsync($"  {json}");
                if (i < departamentos.Count - 1) await response.WriteAsync(",");
                await response.WriteAsync("\n");
            }

            await response.WriteAsync("]\n");
        });

        // ====================================================================
        // Full Database Export (All tables combined)
        // ====================================================================

        group.MapGet("/all.csv", async (GeoDBContext db, HttpResponse response) =>
        {
            response.ContentType = "application/zip";
            response.Headers.Append("Content-Disposition", $"attachment; filename=geo_col_export_{DateTime.UtcNow:yyyyMMdd}.zip");
            
            // Note: For ZIP files, you'll need to use a library like SharpCompress or System.IO.Compression
            // This is a simplified example - you'd need to create a ZIP with multiple CSV files
            
            return Results.BadRequest("ZIP export requires additional library. Use individual CSV exports for now.");
        });

        // ====================================================================
        // Filtered Export by Query Parameter
        // ====================================================================

        group.MapGet("/custom.csv", async (
            GeoDBContext db, 
            HttpResponse response,
            string? entity = null,
            int? departamentoId = null,
            int? municipioId = null) =>
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Results.BadRequest("Entity parameter is required (departamentos, municipios, barrios, veredas, corregimientos, centros_poblados)");
            }

            response.ContentType = "text/csv; charset=utf-8";
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
            var filename = $"{entity}_{timestamp}.csv";
            response.Headers.Append("Content-Disposition", $"attachment; filename={filename}");

            // UTF-8 with BOM for Excel compatibility with accents
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(true));

            switch (entity.ToLower())
            {
                case "departamentos":
                    await writer.WriteLineAsync("Id,Departamento");
                    var deps = await db.Departamentos.Select(d => new { d.Id, d.Nombre }).ToListAsync();
                    foreach (var d in deps)
                    {
                        await writer.WriteLineAsync($"{d.Id},{EscapeCsvValue(d.Nombre)}");
                    }
                    break;

                case "municipios":
                    await writer.WriteLineAsync("Id,Municipio,DepartamentoId,LinkMapaMunicipio");
                    var query = db.Municipios.AsQueryable();
                    if (departamentoId.HasValue)
                        query = query.Where(m => m.DepartamentoId == departamentoId.Value);
                    
                    var muns = await query.Select(m => new { m.Id, m.Nombre, m.DepartamentoId, m.LinkMapaMunicipio }).ToListAsync();
                    foreach (var m in muns)
                    {
                        await writer.WriteLineAsync($"{m.Id},{EscapeCsvValue(m.Nombre)},{m.DepartamentoId},{EscapeCsvValue(m.LinkMapaMunicipio ?? "")}");
                    }
                    break;

                // Add more cases as needed
                default:
                    return Results.BadRequest($"Unknown entity: {entity}");
            }

            await writer.FlushAsync();
            return null;
        });
    }

    private static string EscapeCsvValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        // If value contains comma, quote, or newline, wrap in quotes and escape quotes
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}


