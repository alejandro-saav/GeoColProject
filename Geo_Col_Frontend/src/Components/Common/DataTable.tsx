import {useMemo} from "react";
import {toPascalCase} from "../../Utils/Formatters.tsx";
interface DataTableProps<T> {
    header: string;
    entityName: string;
    data: T[],
    onClickHandler?: (item: T) => void;
    entidadSeleccionada?: Record<string, any> | null;
}
export default function DataTable<T extends object>({header, entityName, data, onClickHandler, entidadSeleccionada} : DataTableProps<T>) {
    if (data == null) {
        return null;
    }
    // Memoize headers to avoid recalculating on every render
    const contentHeaders: string[] = useMemo(() => {
        return data.length > 0 ? Object.keys(data[0]) : [];
    }, [data]);
    return (
        <div className="w-full max-w-md mx-auto p-2">
            <div className="bg-[#1a1a2e] rounded-lg shadow-2xl overflow-hidden border border-[#2a2a3e]">
        {/*   HEADER     */}
        <div className="bg-gradient-to-r from-[#000030] to-[#1a1a3e] px-4 py-3 border-b border-[#2a2a3e]">
            <h2 className="text-xl font-bold text-[#F8FAFC] font-heading">
                {header}
            </h2>
            <p className="text-xs text-gray-400 mt-1">
                {data.length} {entityName} disponibles
            </p>
        </div>
                {data.length === 0 ? (
                    <div className="p-8 text-center">
                        <p className="text-gray-400 text-sm">No hay {entityName} disponibles</p>
                    </div>
                ) : (
                    /* Table Content */
                    <div className="overflow-x-auto">
                        <table className="w-full table-auto">
                            <thead>
                            <tr className="bg-[#0f0f1e] border-b border-[#2a2a3e]">
                                {contentHeaders.map((dataHeader:string, index:number) => (
                                    <th key={index} className="px-3 py-2 text-left text-xs font-semibold text-[#FFFF00] uppercase tracking-wider">
                                        {dataHeader}
                                    </th>
                                ))}
                            </tr>
                            </thead>
                            <tbody className="divide-y divide-[#2a2a3e]">
                            {data.map((item:any) => (
                                <tr
                                    key={item.id}
                                    className={`${entidadSeleccionada === item ? "bg-[#0A0A12]" : "bg-[#1a1a2e]"} hover:bg-[#252540] transition-colors duration-150 cursor-pointer`}  onClick={() => {
                                    if (onClickHandler != undefined) {
                                        onClickHandler(item)
                                    }
                                }
                                }
                                >
                                    {Object.values(item as Record<string, any>).map((value, j) => (
                                        <td key={j} className="px-3 py-2">
                                            <span className="text-sm font-medium text-[#F8FAFC]">
                                                {toPascalCase(String(value))}
                                            </span>
                                        </td>
                                    ))}
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    )
}