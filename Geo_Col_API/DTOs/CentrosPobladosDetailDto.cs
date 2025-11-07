namespace Geo_Col_API.DTOs;

public class CentrosPobladosDetailDto
{
    public int Id { get; set; }
    public string CentroPoblado { get; set; }
    public string TipoCentroPoblado { get; set; }
    public int MunicipioId { get; set; }
    public string Municipio { get; set; }
}