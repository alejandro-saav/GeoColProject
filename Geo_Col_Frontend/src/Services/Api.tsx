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

    async getDepartamentos(): Promise<DepartamentosDto[]> {
        const response = await fetch(`${this.baseUrl}/departamentos`);
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
        const response = await fetch(`${this.baseUrl}/municipios/${municipioId}/centros_poblados`);
        return response.json();
    }
    
    async getDepartamentosExcel(): Promise<Blob | null> {
        try {
            const response: Response = await fetch(`${this.baseUrl}/export/departamentos.csv`);
            if (!response.ok) throw new Error(response.statusText);
            return response.blob();
        } catch (error) {
            console.log(error);
            return null;
        }
    }
    
}

export const geoApiService = new GeographicApiService(import.meta.env.VITE_API_URL);