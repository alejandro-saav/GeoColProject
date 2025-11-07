using System.ComponentModel.DataAnnotations.Schema;

namespace Geo_Col_API.Models;

[Table("municipio")]
public class Municipio
{
    [Column("id")]
    public int Id { get; set; }
    [Column("nombre")]
    public string Nombre { get; set; }
    [Column("departamento_id")]
    public int DepartamentoId { get; set; }
    [Column("link_mapa_municipio")]
    public string? LinkMapaMunicipio { get; set; }
    
    public Departamento Departamento { get; set; }
    public ICollection<ComunaBarrio> ComunasBarrios { get; set; } = new List<ComunaBarrio>();
    public ICollection<CentrosPoblados> CentrosPoblados { get; set; } = new List<CentrosPoblados>();
    public ICollection<Corregimiento> Corregimientos { get; set; } = new List<Corregimiento>();
    public ICollection<Vereda> Veredas { get; set; } = new List<Vereda>();
}