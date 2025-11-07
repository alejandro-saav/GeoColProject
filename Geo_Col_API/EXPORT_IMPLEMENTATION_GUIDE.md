# Data Export/Download Implementation Guide

## ✅ **What I've Implemented**

I've created `ExportEndpoints.cs` with **streaming CSV exports** - the best approach for your use case because:

1. **Memory Efficient**: Processes data in batches, doesn't load everything into memory
2. **Fast**: Starts streaming immediately, no waiting for full dataset
3. **Scalable**: Can handle millions of records
4. **Simple**: No external libraries needed for CSV

## 📋 **Available Export Endpoints**

### CSV Exports (Implemented)
- `GET /export/departamentos.csv` - All departments
- `GET /export/municipios.csv?departamentoId={id}` - Municipalities (optionally filtered)
- `GET /export/barrios.csv?municipioId={id}` - Barrios (optionally filtered)
- `GET /export/custom.csv?entity={name}&departamentoId={id}&municipioId={id}` - Flexible export

### JSON Exports (Basic Implementation)
- `GET /export/departamentos.json` - All departments as JSON

## 🚀 **Additional Options You Can Add**

### 1. **Excel/XLSX Exports** (For Business Users)

**Package Required:**
```xml
<PackageReference Include="EPPlus" Version="7.0.0" />
<!-- OR -->
<PackageReference Include="ClosedXML" Version="0.102.1" />
```

**Example Implementation:**
```csharp
group.MapGet("/municipios.xlsx", async (GeoDBContext db, HttpResponse response, int? departamentoId = null) =>
{
    using var package = new OfficeOpenXml.ExcelPackage();
    var worksheet = package.Workbook.Worksheets.Add("Municipios");
    
    // Headers
    worksheet.Cells[1, 1].Value = "Id";
    worksheet.Cells[1, 2].Value = "Municipio";
    worksheet.Cells[1, 3].Value = "DepartamentoId";
    
    var query = db.Municipios.AsQueryable();
    if (departamentoId.HasValue)
        query = query.Where(m => m.DepartamentoId == departamentoId.Value);
    
    var municipios = await query.ToListAsync();
    
    for (int i = 0; i < municipios.Count; i++)
    {
        var row = i + 2;
        worksheet.Cells[row, 1].Value = municipios[i].Id;
        worksheet.Cells[row, 2].Value = municipios[i].Nombre;
        worksheet.Cells[row, 3].Value = municipios[i].DepartamentoId;
    }
    
    response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    response.Headers.Append("Content-Disposition", 
        $"attachment; filename=municipios_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    
    await response.Body.WriteAsync(package.GetAsByteArray());
});
```

**Pros:**
- Business-friendly format
- Can include multiple sheets
- Supports formatting, formulas

**Cons:**
- Requires additional package
- More memory usage
- Slower for very large datasets

### 2. **ZIP Exports** (Multiple Files)

**Package Required:**
```xml
<!-- Built-in, no package needed for basic ZIP -->
```

**Example Implementation:**
```csharp
using System.IO.Compression;

group.MapGet("/all.zip", async (GeoDBContext db, HttpResponse response) =>
{
    response.ContentType = "application/zip";
    response.Headers.Append("Content-Disposition", 
        $"attachment; filename=geo_col_export_{DateTime.UtcNow:yyyyMMdd}.zip");
    
    using var archive = new ZipArchive(response.Body, ZipArchiveMode.Create);
    
    // Add departamentos CSV
    var deptEntry = archive.CreateEntry("departamentos.csv");
    await using var deptStream = deptEntry.Open();
    await using var deptWriter = new StreamWriter(deptStream, Encoding.UTF8);
    await deptWriter.WriteLineAsync("Id,Departamento");
    
    var departamentos = await db.Departamentos.ToListAsync();
    foreach (var d in departamentos)
    {
        await deptWriter.WriteLineAsync($"{d.Id},{d.Nombre}");
    }
    
    // Add municipios CSV
    var munEntry = archive.CreateEntry("municipios.csv");
    // ... similar code
    
    await response.Body.FlushAsync();
});
```

### 3. **Async/BACKGROUND EXPORTS** (For Very Large Datasets)

**When to Use:**
- Datasets > 100,000 records
- Complex queries that take > 30 seconds
- User wants to receive export via email

**Approach:**
1. User requests export → Returns job ID
2. Background job processes export
3. Store file temporarily (Azure Blob, S3, local storage)
4. Email user with download link (expires in 24 hours)

**Implementation:**
- Use Hangfire or Quartz.NET for background jobs
- Store job status in database
- Use cloud storage (Azure Blob, AWS S3) or temp directory

### 4. **Filtered/Search Exports**

**Example:**
```csharp
group.MapGet("/search/export.csv", async (
    GeoDBContext db, 
    HttpResponse response,
    string query,
    string entity = "municipios") =>
{
    // Use your existing search logic
    // Export search results instead of all data
    // Great for "Export these 30 search results"
});
```

## 📊 **Recommendations by Use Case**

| Use Case | Format | Implementation | Priority |
|----------|--------|----------------|----------|
| **Developer/API Users** | CSV, JSON | ✅ Already done | High |
| **Business Users/Excel** | XLSX | Add EPPlus/ClosedXML | Medium |
| **Complete Database Backup** | ZIP with multiple CSVs | Add ZIP support | Medium |
| **Large Datasets (>100K)** | Background job + email | Add async processing | Low |
| **Filtered Exports** | CSV/JSON with query params | ✅ Already done | High |

## 🎯 **Best Practices**

### ✅ **DO:**
1. **Stream responses** - Don't load all data into memory
2. **Use batching** - Process in chunks (1000-5000 records)
3. **Add timestamps** to filenames (`departamentos_20241220.csv`)
4. **Include metadata** - Add header comments or separate metadata file
5. **Handle encoding** - Use UTF-8 with BOM for Excel compatibility
6. **Add query parameters** - Let users filter what they export
7. **Set proper headers** - `Content-Disposition`, `Content-Type`

### ❌ **DON'T:**
1. Load entire dataset into memory at once
2. Generate files on disk if not needed (stream directly)
3. Forget to escape CSV values (commas, quotes, newlines)
4. Expose exports without authentication (if charging)
5. Allow unbounded exports (add limits if needed)

## 💰 **Monetization Considerations**

### Free Tier:
- CSV exports only
- Max 10,000 records per export
- No background exports

### Paid Tier:
- CSV, JSON, Excel exports
- Unlimited records
- Background/async exports
- ZIP downloads
- Scheduled exports

### Enterprise:
- All formats
- Custom field selection
- API for programmatic exports
- Webhook notifications
- Dedicated support

## 🔧 **Quick Add-Ons**

### Add Export Links to Your Existing Endpoints:
```csharp
// In your response DTOs, add:
public class MunicipiosPorDepartamentoDto
{
    // ... existing properties
    public string? ExportCsvUrl => $"/export/municipios.csv?departamentoId={DepartamentoId}";
    public string? ExportJsonUrl => $"/export/municipios.json?departamentoId={DepartamentoId}";
}
```

### Add Export Format Query Parameter:
```csharp
group.MapGet("/departamentos.{format}", async (
    GeoDBContext db, 
    HttpResponse response,
    string format = "csv") =>
{
    format = format.ToLower();
    switch (format)
    {
        case "csv":
            // CSV export logic
            break;
        case "json":
            // JSON export logic
            break;
        default:
            return Results.BadRequest($"Unsupported format: {format}");
    }
});
```

## 📦 **Next Steps**

1. ✅ **Test the implemented CSV exports** - Make sure they work with your data
2. ⚠️ **Add Excel support** - If you want business users (add EPPlus)
3. ⚠️ **Add ZIP support** - For complete database exports
4. ⚠️ **Add authentication** - Protect exports if monetizing
5. ⚠️ **Add rate limiting** - Prevent abuse of export endpoints
6. ⚠️ **Add usage tracking** - Monitor export usage for billing

## 🧪 **Testing Your Exports**

```http
### Test CSV export
GET http://localhost:5149/export/departamentos.csv
Accept: */*

### Test filtered export
GET http://localhost:5149/export/municipios.csv?departamentoId=1
Accept: */*

### Test custom export
GET http://localhost:5149/export/custom.csv?entity=barrios&municipioId=1
Accept: */*
```

---

**Need help implementing any of these?** Let me know which feature you'd like to add first!

