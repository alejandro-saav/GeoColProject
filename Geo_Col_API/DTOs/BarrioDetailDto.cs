namespace Geo_Col_API.DTOs;

public class BarrioDetailDto
{
    public int Id { get; set; }
    public string NombreBarrio { get; set; }
    public string? NombreComuna { get; set; }
    public int? NumComuna { get; set; }
    public int MunicipioId { get; set; }
    public int DepartamentoId { get; set; }
}