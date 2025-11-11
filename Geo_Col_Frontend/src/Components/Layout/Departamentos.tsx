import {geoApiService} from "../../Services/Api.tsx";
import {useEffect, useState} from "react";
import type {DepartamentosDto} from "../../Services/Types.tsx";

interface DepartamentoProps {
    departamentoId?: number;
}

function toPascalCase(str: string): string {
    if (!str) return str;
    return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
}

export default function Departamentos({departamentoId}: DepartamentoProps) {
    const [departamentos, setDepartamentos] = useState<DepartamentosDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        async function fetchDepartamentos(){
            try {
                setLoading(true);
                const deps = await geoApiService.getDepartamentos();
                setDepartamentos(deps);
            } catch (error) {
                console.error("Error fetching departamentos:", error);
            } finally {
                setLoading(false);
            }
        }
        fetchDepartamentos();
    },[]);
    
    const sortedDepartamentos = departamentos.length > 0 
        ? [...departamentos].sort((a, b) => a.id - b.id)
        : [];
    
    return (
        <div className="w-full max-w-md mx-auto p-4">
            <div className="bg-[#1a1a2e] rounded-lg shadow-2xl overflow-hidden border border-[#2a2a3e]">
                {/* Header */}
                <div className="bg-gradient-to-r from-[#000030] to-[#1a1a3e] px-4 py-3 border-b border-[#2a2a3e]">
                    <h2 className="text-xl font-bold text-[#F8FAFC] font-heading">
                        Departamentos de Colombia
                    </h2>
                    <p className="text-xs text-gray-400 mt-1">
                        {sortedDepartamentos.length} departamentos disponibles
                    </p>
                </div>

                {/* Loading State */}
                {loading ? (
                    <div className="p-8 text-center">
                        <div className="inline-block animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-[#FFFF00]"></div>
                        <p className="text-gray-400 mt-4 text-sm">Cargando departamentos...</p>
                    </div>
                ) : sortedDepartamentos.length === 0 ? (
                    <div className="p-8 text-center">
                        <p className="text-gray-400 text-sm">No hay departamentos disponibles</p>
                    </div>
                ) : (
                    /* Table Content */
                    <div className="overflow-x-auto">
                        <table className="w-full table-auto">
                            <thead>
                                <tr className="bg-[#0f0f1e] border-b border-[#2a2a3e]">
                                    <th className="px-3 py-2 text-left text-xs font-semibold text-[#FFFF00] uppercase tracking-wider w-12">
                                        ID
                                    </th>
                                    <th className="px-3 py-2 text-left text-xs font-semibold text-[#FFFF00] uppercase tracking-wider">
                                        Departamento
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-[#2a2a3e]">
                                {sortedDepartamentos.map((item, index) => (
                                    <tr 
                                        key={item.id} 
                                        className="bg-[#1a1a2e] hover:bg-[#252540] transition-colors duration-150 cursor-pointer"
                                    >
                                        <td className="px-3 py-2 whitespace-nowrap">
                                            <span className="text-sm font-medium text-[#F8FAFC]">
                                                {item.id}
                                            </span>
                                        </td>
                                        <td className="px-3 py-2">
                                            <span className="text-sm text-[#F8FAFC] font-body">
                                                {toPascalCase(item.departamento)}
                                            </span>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
}