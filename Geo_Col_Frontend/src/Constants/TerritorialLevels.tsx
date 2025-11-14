export const TERRITORIAL_LEVELS = [
    "comunas_barrios",
    "centros_poblados",
    "veredas",
    "corregimientos",
] as const;

export type TerritorialLevel = typeof TERRITORIAL_LEVELS[number];