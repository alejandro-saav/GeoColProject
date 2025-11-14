import type {TerritorialLevel} from "../Constants/TerritorialLevels.tsx";
import {geoApiService} from "./Api.tsx";

export const fetchers: Record<TerritorialLevel, (id: number) => Promise<any>> = {
    comunas_barrios: geoApiService.getComunasBarriosPorMunicipio,
    centros_poblados: geoApiService.getCentrosPobladosPorMunicipio,
    corregimientos: geoApiService.getCorregimientosPorMunicipio,
    veredas: geoApiService.getVeredasPorMunicipio,
};