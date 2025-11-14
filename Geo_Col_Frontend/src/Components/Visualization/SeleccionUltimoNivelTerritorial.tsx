import type {Dispatch, SetStateAction} from "react";
import {TERRITORIAL_LEVELS, type TerritorialLevel} from "../../Constants/TerritorialLevels.tsx";
import {formatTerritorialLabel} from "../../Utils/Formatters.tsx";

interface componentProps{
    onClickHandler: Dispatch<SetStateAction<TerritorialLevel | null>>;
}
export default function SeleccionUltimoNivelTerritorial ({onClickHandler}: componentProps) {
    return (
        <div className="w-full max-w-md mx-auto p-4">
            <div className="bg-[#1a1a2e] rounded-lg shadow-2xl overflow-hidden border border-[#2a2a3e]">
                {/*   HEADER     */}
                <div className="bg-gradient-to-r from-[#000030] to-[#1a1a3e] px-4 py-3 border-b border-[#2a2a3e]">
                    <h2 className="text-xl font-bold text-[#F8FAFC] font-heading">
                        Seleccione una subdivisión
                    </h2>
                    <p className="text-xs text-gray-400 mt-1">
                        4 subdivisiónes disponibles
                    </p>
                </div>
                {/* BODY */}
                <div className="overflow-x-auto">
                    <table className="w-full table-auto">
                        <thead>
                        <tr className="bg-[#0f0f1e] border-b border-[#2a2a3e]">
                                <th className="px-3 py-2 text-left text-xs font-semibold text-[#FFFF00] uppercase tracking-wider">
                                    Subdivision Territorial
                                </th>
                         
                        </tr>
                        </thead>
                        <tbody className="divide-y divide-[#2a2a3e]">
                        {TERRITORIAL_LEVELS.map((item:TerritorialLevel, index: number) => (
                            <tr
                                key={index}
                                className="bg-[#1a1a2e] hover:bg-[#252540] transition-colors duration-150 cursor-pointer" 
                                onClick={() => onClickHandler(item)}
                            >
                                    <td key={index} className="px-3 py-2">
                                            <span className="text-sm font-medium text-[#F8FAFC]">
                                                {formatTerritorialLabel(item)}
                                            </span>
                                    </td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    )
}