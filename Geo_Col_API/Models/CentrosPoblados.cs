using System.ComponentModel.DataAnnotations.Schema;

namespace Geo_Col_API.Models;

[Table("centros_poblados")]
public class CentrosPoblados
{
    [Column("id")]
    public int Id { get; set; }
    [Column("nombre")]
    public string Nombre { get; set; }
    [Column("tipo")]
    public int TipoCentroPobladoId { get; set; }
    [Column("municipio_id")]
    public int MunicipioId { get; set; }
    
    public Municipio Municipio { get; set; }
    public TipoCentroPoblado TipoCentroPoblado { get; set; }
}