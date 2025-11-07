using Geo_Col_API.Endpoints;

namespace Geo_Col_API.Extensions;

public static class EndpointExtensions
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapDepartamentoEndpoints();
        app.MapMunicipioEndpoints();
        app.MapComunaBarrioEndpoints();
        app.MapVeredaEndpoints();
        app.MapCentroPobladoEndpoints();
        app.MapReferenciaEndpoints();
        app.MapCorregimientoEndpoints();
        app.MapExportEndpoints();
    }
}