using Geo_Col_API.Data;
using Geo_Col_API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;
using System.Text.Json;

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
            await writer.WriteLineAsync("Id,Municipio,DepartamentoId");
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
                    .Select(m => new { m.Id, m.Nombre, m.DepartamentoId })
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    var municipio = EscapeCsvValue(item.Nombre);
                    await writer.WriteLineAsync($"{item.Id},{municipio},{item.DepartamentoId}");
                }

                await writer.FlushAsync();
                skip += batchSize;
            }
        });

        group.MapGet("/comunas_barrios.csv", async (GeoDBContext db, HttpResponse response, int? municipioId = null) =>
        {
            response.ContentType = "text/csv; charset=utf-8";
            var filename = municipioId.HasValue
                ? $"comunas_barrios_mun_{municipioId}_{DateTime.UtcNow:yyyyMMdd}.csv"
                : $"comunas_barrios_{DateTime.UtcNow:yyyyMMdd}.csv";
            response.Headers.Append("Content-Disposition", $"attachment; filename={filename}");

            // UTF-8 with BOM for Excel compatibility with accents
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(true));
            await writer.WriteLineAsync("Id,NumComuna,NombreComuna,NombreBarrio");
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
                    .Select(cb => new { cb.Id, cb.NumComuna, cb.NombreComuna, cb.NombreBarrio })
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    var barrio = EscapeCsvValue(item.NombreBarrio);
                    var comuna = EscapeCsvValue(item.NombreComuna);
                    await writer.WriteLineAsync($"{item.Id},{item.NumComuna},{comuna},{barrio}");
                }

                await writer.FlushAsync();
                skip += batchSize;
            }
        });
        
        group.MapGet("/centros_poblados.csv", async (GeoDBContext db, HttpResponse response, int? municipioId = null) =>
        {
            response.ContentType = "text/csv; charset=utf-8";
            var filename = municipioId.HasValue
                ? $"centros_poblados_mun_{municipioId}_{DateTime.UtcNow:yyyyMMdd}.csv"
                : $"centros_poblados_{DateTime.UtcNow:yyyyMMdd}.csv";
            response.Headers.Append("Content-Disposition", $"attachment; filename={filename}");

            // UTF-8 with BOM for Excel compatibility with accents
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(true));
            await writer.WriteLineAsync("Id,Nombre,Tipo");
            await writer.FlushAsync();

            var query = db.CentrosPoblados.AsQueryable();
            if (municipioId.HasValue)
            {
                query = query.Where(cp => cp.MunicipioId == municipioId.Value);
            }

            const int batchSize = 1000;
            int skip = 0;

            while (true)
            {
                var batch = await query
                    .OrderBy(cb => cb.Id)
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(cp => new { cp.Id, cp.Nombre, cp.TipoCentroPoblado })
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    var centroPoblado = EscapeCsvValue(item.Nombre);
                    await writer.WriteLineAsync($"{item.Id},{centroPoblado},{item.TipoCentroPoblado}");
                }

                await writer.FlushAsync();
                skip += batchSize;
            }
        });
        
        group.MapGet("/corregimientos.csv", async (GeoDBContext db, HttpResponse response, int? municipioId = null) =>
        {
            response.ContentType = "text/csv; charset=utf-8";
            var filename = municipioId.HasValue
                ? $"corregimientos_mun_{municipioId}_{DateTime.UtcNow:yyyyMMdd}.csv"
                : $"corregimientos_{DateTime.UtcNow:yyyyMMdd}.csv";
            response.Headers.Append("Content-Disposition", $"attachment; filename={filename}");

            // UTF-8 with BOM for Excel compatibility with accents
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(true));
            await writer.WriteLineAsync("Id,Nombre");
            await writer.FlushAsync();

            var query = db.Corregimientos.AsQueryable();
            if (municipioId.HasValue)
            {
                query = query.Where(c => c.MunicipioId == municipioId.Value);
            }

            const int batchSize = 1000;
            int skip = 0;

            while (true)
            {
                var batch = await query
                    .OrderBy(cb => cb.Id)
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(c => new { c.Id, c.Nombre })
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    var corregimiento = EscapeCsvValue(item.Nombre);
                    await writer.WriteLineAsync($"{item.Id},{corregimiento}");
                }

                await writer.FlushAsync();
                skip += batchSize;
            }
        });
        
        group.MapGet("/veredas.csv", async (GeoDBContext db, HttpResponse response, int? municipioId = null) =>
        {
            response.ContentType = "text/csv; charset=utf-8";
            var filename = municipioId.HasValue
                ? $"veredas_mun_{municipioId}_{DateTime.UtcNow:yyyyMMdd}.csv"
                : $"veredas_{DateTime.UtcNow:yyyyMMdd}.csv";
            response.Headers.Append("Content-Disposition", $"attachment; filename={filename}");

            // UTF-8 with BOM for Excel compatibility with accents
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(true));
            await writer.WriteLineAsync("Id,Nombre");
            await writer.FlushAsync();

            var query = db.Veredas.AsQueryable();
            if (municipioId.HasValue)
            {
                query = query.Where(v => v.MunicipioId == municipioId.Value);
            }

            const int batchSize = 1000;
            int skip = 0;

            while (true)
            {
                var batch = await query
                    .OrderBy(cb => cb.Id)
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(v => new { v.Id, v.Nombre })
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var item in batch)
                {
                    var vereda = EscapeCsvValue(item.Nombre);
                    await writer.WriteLineAsync($"{item.Id},{vereda}");
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
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Append("Content-Disposition", $"attachment; filename=departamentos_{DateTime.UtcNow:yyyyMMdd}.json");

            // UTF-8 encoding
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(false));
            
            await writer.WriteLineAsync("[");
            
            var departamentos = await db.Departamentos
                .OrderBy(d => d.Id)
                .Select(d => new DepartamentosDto { Id = d.Id, Departamento = d.Nombre })
                .ToListAsync();

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            for (int i = 0; i < departamentos.Count; i++)
            {
                var json = JsonSerializer.Serialize(departamentos[i], options);
                await writer.WriteAsync("  " + json);
                if (i < departamentos.Count - 1) await writer.WriteAsync(",");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync("]");
            await writer.FlushAsync();
        });
        
        group.MapGet("/municipios.json", async (GeoDBContext db, HttpResponse response) =>
        {
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Append("Content-Disposition", $"attachment; filename=municipios_{DateTime.UtcNow:yyyyMMdd}.json");

            // UTF-8 encoding
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(false));
            
            await writer.WriteLineAsync("[");
            
            var municipios = await db.Municipios
                .OrderBy(d => d.Id)
                .Select(m => new MunicipioDto { Id = m.Id, Municipio = m.Nombre })
                .ToListAsync();

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            for (int i = 0; i < municipios.Count; i++)
            {
                var json = JsonSerializer.Serialize(municipios[i], options);
                await writer.WriteAsync("  " + json);
                if (i < municipios.Count - 1) await writer.WriteAsync(",");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync("]");
            await writer.FlushAsync();
        });
        
        group.MapGet("/comunas_barrios.json", async (GeoDBContext db, HttpResponse response) =>
        {
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Append("Content-Disposition", $"attachment; filename=comunas_barrios_{DateTime.UtcNow:yyyyMMdd}.json");

            // UTF-8 encoding
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(false));
            
            await writer.WriteLineAsync("[");
            
            var comunas_barrios = await db.ComunaBarrios
                .OrderBy(cb => cb.Id)
                .Select(cb => new BarrioDto { Id = cb.Id, NumComuna = cb.NumComuna, NombreComuna = cb.NombreComuna, NombreBarrio = cb.NombreBarrio})
                .ToListAsync();

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            for (int i = 0; i < comunas_barrios.Count; i++)
            {
                var json = JsonSerializer.Serialize(comunas_barrios[i], options);
                await writer.WriteAsync("  " + json);
                if (i < comunas_barrios.Count - 1) await writer.WriteAsync(",");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync("]");
            await writer.FlushAsync();
        });
        
        group.MapGet("/corregimientos.json", async (GeoDBContext db, HttpResponse response) =>
        {
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Append("Content-Disposition", $"attachment; filename=corregimientos_{DateTime.UtcNow:yyyyMMdd}.json");

            // UTF-8 encoding
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(false));
            
            await writer.WriteLineAsync("[");
            
            var corregimientos = await db.Corregimientos
                .OrderBy(c => c.Id)
                .Select(c => new CorregimientoDto { Id = c.Id, Corregimiento = c.Nombre })
                .ToListAsync();

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            for (int i = 0; i < corregimientos.Count; i++)
            {
                var json = JsonSerializer.Serialize(corregimientos[i], options);
                await writer.WriteAsync("  " + json);
                if (i < corregimientos.Count - 1) await writer.WriteAsync(",");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync("]");
            await writer.FlushAsync();
        });
        
        group.MapGet("/centros_poblados.json", async (GeoDBContext db, HttpResponse response) =>
        {
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Append("Content-Disposition", $"attachment; filename=centros_poblados_{DateTime.UtcNow:yyyyMMdd}.json");

            // UTF-8 encoding
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(false));
            
            await writer.WriteLineAsync("[");
            
            var centros_poblados = await db.CentrosPoblados
                .OrderBy(cp => cp.Id)
                .Select(cp => new CentroPobladoDto { Id = cp.Id, CentroPoblado = cp.Nombre, Tipo = cp.TipoCentroPoblado.Nombre })
                .ToListAsync();

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            for (int i = 0; i < centros_poblados.Count; i++)
            {
                var json = JsonSerializer.Serialize(centros_poblados[i], options);
                await writer.WriteAsync("  " + json);
                if (i < centros_poblados.Count - 1) await writer.WriteAsync(",");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync("]");
            await writer.FlushAsync();
        });
        
        group.MapGet("/veredas.json", async (GeoDBContext db, HttpResponse response) =>
        {
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Append("Content-Disposition", $"attachment; filename=veredas_{DateTime.UtcNow:yyyyMMdd}.json");

            // UTF-8 encoding
            await using var writer = new StreamWriter(response.Body, new UTF8Encoding(false));
            
            await writer.WriteLineAsync("[");
            
            var veredas = await db.Veredas
                .OrderBy(v => v.Id)
                .Select(v => new VeredaDto { Id = v.Id, Vereda = v.Nombre })
                .ToListAsync();

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            for (int i = 0; i < veredas.Count; i++)
            {
                var json = JsonSerializer.Serialize(veredas[i], options);
                await writer.WriteAsync("  " + json);
                if (i < veredas.Count - 1) await writer.WriteAsync(",");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync("]");
            await writer.FlushAsync();
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


