export interface BarrioDto {
    id: number;
    nombreBarrio: string;
    nombreComuna: string | null;
    numComuna: number | null;
}

export interface BarrioDetailDto {
    id: number;
    nombreBarrio: string;
    nombreComuna: string;
    numComuna: number;
    municipioId: number;
    departamentoId: number;
}

export interface BarriosPorMunicipioDto {
    municipioId: number;
    municipio: string;
    barrios: BarrioDto[];
}

export interface CentroPobladoDto {
    id: number;
    centroPoblado: string;
    tipo: string;
}

export interface CentrosPobladosDetailDto {
    id: number;
    centroPoblado: string;
    tipoCentroPoblado: string;
    municipioId: number;
    municipio: string;
}

export interface CentrosPobladosPorMunicipioDto {
    municipioId: number;
    municipio: string;
    centrosPoblados: CentroPobladoDto[];
}

export interface ComunasDto {
    id: number;
    numeroComuna: number;
    nombreComuna: string;
}

export interface ComunasPorMunicipio {
    municipioId: number;
    municipio: string;
    comunas: ComunasDto[];
}

export interface CorregimientoDto {
    id: number;
    corregimiento: string;
}

export interface CorregimientoDetailDto {
    id: number;
    corregimiento: string;
    municipioId: number;
    municipio: string;
}

export interface CorregimientosPorMunicipioDto {
    municipioId: number;
    municipio: string;
    corregimientos: CorregimientoDto[];
}

export interface DepartamentosDto {
    id: number;
    departamento: string;
}

export interface MunicipioDto {
    id: number;
    municipio: string;
    linkMapaMunicipio: string;
}

export interface MunicipioDetailDto {
    id: number;
    municipio: string;
    linkMapaMunicipio: string | null;
    departamentoId: number;
    departamento: string;
}

export interface MunicipioSearchResult {
    id: number;
    municipio: string;
    departamentoId: number;
    linkMapaMunicipio: string | null;
    departamento: string;
}

export interface MunicipiosPorDepartamentoDto {
    departamentoId: number;
    departamento: string;
    municipios: MunicipioDto[];
}

export interface VeredaDto {
    id: number;
    vereda: string;
}

export interface VeredaDetailDto {
    id: number;
    vereda: string;
    municipioId: number;
    municipio: string;
    corregimientoId: number;
    corregimiento: string;
}

export interface VeredasPorMunicipioDto {
    municipioId: number;
    municipio: string;
    veredas: VeredaDto[];
}