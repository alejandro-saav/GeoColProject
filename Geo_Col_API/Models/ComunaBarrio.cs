using System.ComponentModel.DataAnnotations.Schema;

namespace Geo_Col_API.Models;

[Table("comuna_barrio")]
public class ComunaBarrio
{
    [Column("id")]
    public int Id { get; set; }
    [Column("num_comuna")]
    public int NumComuna { get; set; }
    [Column("nombre_barrio")]
    public string NombreBarrio { get; set; }
    [Column("municipio_id")]
    public int MunicipioId { get; set; }
    [Column("departamento_id")]
    public int DepartamentoId { get; set; }
    [Column("nombre_comuna")]
    public string? NombreComuna { get; set; }
    
    public Municipio Municipio { get; set; }
    public Departamento Departamento { get; set; }
}