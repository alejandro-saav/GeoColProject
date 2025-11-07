using System.ComponentModel.DataAnnotations.Schema;

namespace Geo_Col_API.Models;

[Table("referencia")]
public class Referencia
{
    [Column("id")]
    public int Id { get; set; }
    [Column("link")]
    public string Link { get; set; }
    [Column("nombre")]
    public string Nombre { get; set; }
    [Column("fecha_ultima_modificacion")]
    public DateOnly FechaUltimaModificacion { get; set; }
    [Column("descripcion")]
    public string Descripcion { get; set; }
}