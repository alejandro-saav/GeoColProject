namespace Geo_Col_API.DTOs;

public class BarriosPorMunicipioDto
{
    public int MunicipioId { get; set; }
    public string Municipio { get; set; }
    public IEnumerable<BarrioDto> Barrios { get; set; }
}