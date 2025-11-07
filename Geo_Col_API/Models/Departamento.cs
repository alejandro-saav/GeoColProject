using System.ComponentModel.DataAnnotations.Schema;

namespace Geo_Col_API.Models;

[Table("departamento")]
public class Departamento
{
    [Column("id")]
    public int Id { get; set; }
    [Column("nombre")]
    public string Nombre { get; set; }
    
    public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
    public ICollection<ComunaBarrio> ComunasBarrios { get; set; } = new List<ComunaBarrio>();
}