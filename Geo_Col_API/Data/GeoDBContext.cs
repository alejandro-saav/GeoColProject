using Geo_Col_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Geo_Col_API.Data;

public class GeoDBContext : DbContext
{
    public GeoDBContext(DbContextOptions<GeoDBContext> options) :base(options) {}
    
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Municipio> Municipios => Set<Municipio>();
    public DbSet<ComunaBarrio> ComunaBarrios => Set<ComunaBarrio>();
    public DbSet<Corregimiento> Corregimientos => Set<Corregimiento>();
    public DbSet<Vereda> Veredas => Set<Vereda>();
    public DbSet<CentrosPoblados> CentrosPoblados => Set<CentrosPoblados>();
    public DbSet<TipoCentroPoblado> TiposCentroPoblado => Set<TipoCentroPoblado>();
    public DbSet<Referencia> Referencias => Set<Referencia>();
}