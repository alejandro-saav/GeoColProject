export interface DownloadOptionsType {
    header: string;
    relativeUrl: string;
}

export const DownloadOptions : DownloadOptionsType[] = [{
    header: "Todo el conjunto de datos",
    relativeUrl: "/all"
}, {
    header: "los Departamentos de Colombia",
    relativeUrl: "departamentos"
}, {
    header: "los Municipios de Colombia",
    relativeUrl: "municipios"
}, {
    header: "las Comunas y Barrios de Colombia",
    relativeUrl: "comunas_barrios"
}, {
    header: "los Corregimientos de Colombia",
    relativeUrl: "corregimientos"
}, {
    header: "los Centros Poblados de Colombia",
    relativeUrl: "centros_poblados"
}, {
    header: "las Veredas de Colombia",
    relativeUrl: "veredas"
}]