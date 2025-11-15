import type {TerritorialLevel} from "../Constants/TerritorialLevels.tsx";
import {geoApiService} from "./Api.tsx";

export const fetchers: Record<TerritorialLevel, (id: number) => Promise<any>> = {
    comunas_barrios: (id) => geoApiService.getComunasBarriosPorMunicipio(id),
    centros_poblados: (id) => geoApiService.getCentrosPobladosPorMunicipio(id),
    corregimientos: (id) => geoApiService.getCorregimientosPorMunicipio(id),
    veredas: (id) => geoApiService.getVeredasPorMunicipio(id),
};