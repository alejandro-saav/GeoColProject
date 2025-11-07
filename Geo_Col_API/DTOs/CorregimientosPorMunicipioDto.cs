namespace Geo_Col_API.DTOs;

public class CorregimientosPorMunicipioDto
{
    public int MunicipioId { get; set; }
    public string Municipio { get; set; }
    public List<CorregimientoDto> Corregimientos { get; set; }
}