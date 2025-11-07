using System.ComponentModel.DataAnnotations.Schema;

namespace Geo_Col_API.Models;

[Table("corregimiento")]
public class Corregimiento
{
    [Column("id")]
    public int Id { get; set; }
    [Column("nombre")]
    public string Nombre { get; set; }
    [Column("municipio_id")]
    public int MunicipioId { get; set; }
    
    public Municipio  Municipio { get; set; }
    public ICollection<Vereda> Veredas { get; set; } = new List<Vereda>();
}