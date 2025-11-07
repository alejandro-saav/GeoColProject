namespace Geo_Col_API.DTOs;

public class ComunasPorMunicipio
{
    public int MunicipioId { get; set; }
    public string Municipio { get; set; }
    public List<ComunasDto> Comunas { get; set; }
}