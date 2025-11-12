import type {MunicipiosPorDepartamentoDto} from "../../Services/Types.tsx";
import {useState, useEffect} from "react";
import {geoApiService} from "../../Services/Api.tsx";
import DataTable from "../Common/DataTable.tsx";

interface MunicipiosProps {
    departamentoId?: number;
}
export default function Municipios({departamentoId} : MunicipiosProps) {
    const [municipios, setMunicipios] = useState<MunicipiosPorDepartamentoDto>();
    const [loading, setLoading] = useState<boolean>(true);
    
    useEffect(() => {
        async function fetchMunicipiosAsync(): Promise<void> {
            try {
                setLoading(true);
                var muns : MunicipiosPorDepartamentoDto = await geoApiService.getMunicipiosPorDepartamento(departamentoId);
                setMunicipios(muns);
            } catch (error) {
                console.log(error);
            } finally {
                setLoading(false);
            }
        }
        fetchMunicipiosAsync();
    }, []);
    
    if (municipios != undefined && municipios.municipios.length > 1) municipios.municipios.sort((a,b) => {
        const nameA = a.municipio.toUpperCase(); // ignore upper and lowercase
        const nameB = b.municipio.toUpperCase(); // ignore upper and lowercase
        if (nameA < nameB) {
            return -1;
        }
        if (nameA > nameB) {
            return 1;
        }
        // names must be equal
        return 0;
    });
    return (
        <div className="w-full max-w-md mx-auto p-4">
            <div className="bg-[#1a1a2e] rounded-lg shadow-2xl overflow-hidden border border-[#2a2a3e]">
                {/* Header */}
                <div className="bg-gradient-to-r from-[#000030] to-[#1a1a3e] px-4 py-3 border-b border-[#2a2a3e]">
                    <h2 className="text-xl font-bold text-[#F8FAFC] font-heading">
                        Municipios de {municipios?.departamento}
                    </h2>
                    <p className="text-xs text-gray-400 mt-1">
                        {municipios?.municipios.length} municipios disponibles
                    </p>
                </div>

                {/* Loading State */}
                {loading ? (
                    <div className="p-8 text-center">
                        <div className="inline-block animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-[#FFFF00]"></div>
                        <p className="text-gray-400 mt-4 text-sm">Cargando departamentos...</p>
                    </div>
                ) : municipios?.municipios.length === 0  || municipios == undefined? (
                    <div className="p-8 text-center">
                        <p className="text-gray-400 text-sm">No hay departamentos disponibles</p>
                    </div>
                ) : (
                    /* Table Content */
                    <DataTable headers={["ID", "Departamento"]} data={municipios?.municipios}/>
                )}
            </div>
        </div>
    )
}