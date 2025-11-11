export interface BarrioDto {
    Id: number;
    NombreBarrio: string;
    NombreComuna: string | null;
    NumComuna: number | null;
}

export interface BarrioDetailDto {
    Id: number;
    NombreBarrio: string;
    NombreComuna: string;
    NumComuna: number;
    MunicipioId: number;
    DepartamentoId: number;
}

export interface BarriosPorMunicipioDto {
    MunicipioId: number;
    Municipio: string;
    Barrios: BarrioDto[];
}

export interface CentroPobladoDto {
    Id: number;
    CentroPoblado: string;
    Tipo: string;
}

export interface CentrosPobladosDetailDto {
    Id: number;
    CentroPoblado: string;
    TipoCentroPoblado: string;
    MunicipioId: number;
    Municipio: string;
}

export interface CentrosPobladosPorMunicipioDto {
    MunicipioId: number;
    Municipio: string;
    CentrosPoblados: CentroPobladoDto[];
}

export interface ComunasDto {
    Id: number;
    NumeroComuna: number;
    NombreComuna: string;
}

export interface ComunasPorMunicipio {
    MunicipioId: number;
    Municipio: string;
    Comunas: ComunasDto[];
}

export interface CorregimientoDto {
    Id: number;
    Corregimiento: string;
}

export interface CorregimientoDetailDto {
    Id: number;
    Corregimiento: string;
    MunicipioId: number;
    Municipio: string;
}

export interface CorregimientosPorMunicipioDto {
    MunicipioId: number;
    Municipio: string;
    Corregimientos: CorregimientoDto[];
}

export interface DepartamentosDto {
    id: number;
    departamento: string;
}

export interface MunicipioDto {
    Id: number;
    Municipio: string;
    LinkMapaMunicipio: string;
}

export interface MunicipioDetailDto {
    Id: number;
    Municipio: string;
    LinkMapaMunicipio: string | null;
    DepartamentoId: number;
    Departamento: string;
}

export interface MunicipioSearchResult {
    Id: number;
    Municipio: string;
    DepartamentoId: number;
    LinkMapaMunicipio: string | null;
    Departamento: string;
}

export interface MunicipiosPorDepartamentoDto {
    DepartamentoId: number;
    Departamento: string;
    Municipios: MunicipioDto[];
}

export interface VeredaDto {
    Id: number;
    Vereda: string;
}

export interface VeredaDetailDto {
    Id: number;
    Vereda: string;
    MunicipioId: number;
    Municipio: string;
    CorregimientoId: number;
    Corregimiento: string;
}

export interface VeredasPorMunicipioDto {
    MunicipioId: number;
    Municipio: string;
    Veredas: VeredaDto[];
}