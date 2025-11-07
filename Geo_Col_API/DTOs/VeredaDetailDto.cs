namespace Geo_Col_API.DTOs;

public class VeredaDetailDto
{
    public int Id { get; set; }
    public string Vereda { get; set; }
    public int MunicipioId { get; set; }
    public string Municipio { get; set; }
    public int CorregimientoId { get; set; }
    public string Corregimiento { get; set; }
}