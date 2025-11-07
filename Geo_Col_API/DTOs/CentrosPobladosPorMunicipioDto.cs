namespace Geo_Col_API.DTOs;

public class CentrosPobladosPorMunicipioDto
{
    public int MunicipioId { get; set; }
    public string Municipio { get; set; }
    public IEnumerable<CentroPobladoDto> CentrosPoblados { get; set; }
}