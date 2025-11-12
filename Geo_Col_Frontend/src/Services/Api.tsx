// services/api.ts
import type {DepartamentosDto, MunicipiosPorDepartamentoDto} from './Types';

class GeographicApiService {
    private baseUrl: string;

    constructor(baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    async getDepartamentos(): Promise<DepartamentosDto[]> {
        const response = await fetch(`${this.baseUrl}/departamentos`);
        return response.json();
    }

    async getMunicipiosPorDepartamento(departamentoId?: number): Promise<MunicipiosPorDepartamentoDto> {
        const response = await fetch(`${this.baseUrl}/departamentos/${departamentoId}/municipios`);
        return response.json();
    }

    // private async fetchData(url: string): Promise<any> {
    //     const response = await fetch(url);
    //     if (!response.ok) {
    //         throw new Error(`API call failed: ${response.statusText}`);
    //     }
    //     return response.json();
    // }
}

export const geoApiService = new GeographicApiService(import.meta.env.VITE_API_URL);