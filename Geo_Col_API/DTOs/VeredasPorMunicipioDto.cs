namespace Geo_Col_API.DTOs;

public class VeredasPorMunicipioDto
{
    public int MunicipioId { get; set; }
    public string Municipio { get; set; }
    public IEnumerable<VeredaDto> Veredas { get; set; }
}