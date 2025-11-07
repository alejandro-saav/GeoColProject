namespace Geo_Col_API.DTOs;

public class CorregimientoDetailDto
{
    public int Id { get; set; }
    public string Corregimiento { get; set; }
    public int MunicipioId { get; set; }
    public string Municipio { get; set; }
}