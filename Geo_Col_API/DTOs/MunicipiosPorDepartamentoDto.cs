namespace Geo_Col_API.DTOs;

public class MunicipiosPorDepartamentoDto
{
    public int DepartamentoId { get; set; }
    public string Departamento { get; set; }
    public IEnumerable<MunicipioDto> Municipios { get; set; }
}