using System.ComponentModel.DataAnnotations.Schema;

namespace Geo_Col_API.Models;

[Table("vereda")]
public class Vereda
{
    [Column("id")]
    public int Id { get; set; }
    [Column("nombre")]
    public string Nombre { get; set; }
    [Column("municipio_id")]
    public int MunicipioId { get; set; }
    [Column("corregimiento_id")]
    public int CorregimientoId { get; set; }
    
    public Municipio Municipio { get; set; }
    public Corregimiento Corregimiento { get; set; }
}