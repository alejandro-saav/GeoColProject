using System.ComponentModel.DataAnnotations.Schema;

namespace Geo_Col_API.Models;

[Table("tipo_centro_poblado")]
public class TipoCentroPoblado
{
    [Column("id")]
    public int Id { get; set; }
    [Column("nombre")]
    public string Nombre { get; set; }
    
}