namespace Geo_Col_API.DTOs;

public class MunicipioDetailDto
{
    public int Id { get; set; }
    public string Municipio { get; set; }
    public string? LinkMapaMunicipio { get; set; }
    public int DepartamentoId { get; set; }
    public string Departamento { get; set; }
}