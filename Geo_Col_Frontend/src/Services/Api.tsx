// services/api.ts
import type {
    BarriosPorMunicipioDto, CentrosPobladosPorMunicipioDto,
    CorregimientosPorMunicipioDto, DepartamentosDto, MunicipioDto, MunicipiosPorDepartamentoDto, VeredasPorMunicipioDto
} from './Types';

class GeographicApiService {
    private baseUrl: string;

    constructor(baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    async getDepartamentos(departamentoId?: number): Promise<DepartamentosDto | DepartamentosDto[]> {
        const depsUrl: string = `${departamentoId ? `/departamentos/${departamentoId}` : '/departamentos'}`;
        const response = await fetch(`${this.baseUrl}${depsUrl}`);
        console.log("RESPONSE:", response);
        return response.json();
    }

    async getMunicipiosPorDepartamento(departamentoId: number): Promise<MunicipiosPorDepartamentoDto> {
        const response = await fetch(`${this.baseUrl}/departamentos/${departamentoId}/municipios`);
        return response.json();
    }
    
    async getMunicipio(municipioId: number): Promise<MunicipioDto> {
        const response = await fetch(`${this.baseUrl}/municipios/${municipioId}`);
        return response.json();
    }
    
    async getComunasBarriosPorMunicipio(municipioId: number): Promise<BarriosPorMunicipioDto> {
        const response = await fetch(`${this.baseUrl}/municipios/${municipioId}/barrios`);
        return response.json();
    }
    
    async getCorregimientosPorMunicipio(municipioId:number): Promise<CorregimientosPorMunicipioDto> {
        const response = await fetch(`${this.baseUrl}/municipios/${municipioId}/corregimientos`);
        return response.json();
    }

    async getVeredasPorMunicipio(municipioId:number): Promise<VeredasPorMunicipioDto> {
        const response = await fetch(`${this.baseUrl}/municipios/${municipioId}/veredas`);
        return response.json();
    }

    async getCentrosPobladosPorMunicipio(municipioId:number): Promise<CentrosPobladosPorMunicipioDto> {
        console.log("WE HERE:)");
        const response = await fetch(`${this.baseUrl}/municipios/${municipioId}/centros_poblados`);
        console.log("RESPONSE:", response);
        return response.json();
    }
    
}

export const geoApiService = new GeographicApiService(import.meta.env.VITE_API_URL);