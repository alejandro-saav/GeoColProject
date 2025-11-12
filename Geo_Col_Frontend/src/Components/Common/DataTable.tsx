import {toPascalCase} from "../../Utils/Formatters.tsx";

interface DataTableProps<T extends {id: number}> {
    headers: string[],
    data: T[],
    onClickHandler: (municipioId:number) => void
}
export default function DataTable<T extends {id:number}>({headers, data, onClickHandler} : DataTableProps<T>) {
    if (headers.length == 0 || data == null || data.length == 0) {
        return null;
    }
    return (
        <div className="overflow-x-auto">
            <table className="w-full table-auto">
                <thead>
                <tr className="bg-[#0f0f1e] border-b border-[#2a2a3e]">
                {headers.map((header:string) => (
                    <th className="px-3 py-2 text-left text-xs font-semibold text-[#FFFF00] uppercase tracking-wider">
                        {header}
                    </th>
                    ))}
                    {/*<th className="px-3 py-2 text-left text-xs font-semibold text-[#FFFF00] uppercase tracking-wider w-12">*/}
                    {/*    ID*/}
                    {/*</th>*/}
                    {/*<th className="px-3 py-2 text-left text-xs font-semibold text-[#FFFF00] uppercase tracking-wider">*/}
                    {/*    Departamento*/}
                    {/*</th>*/}
                </tr>
                </thead>
                <tbody className="divide-y divide-[#2a2a3e]">
                {data.map((item:any) => (
                    <tr
                        key={item.id}
                        className="bg-[#1a1a2e] hover:bg-[#252540] transition-colors duration-150 cursor-pointer" onClick={():void => onClickHandler(item.id)}
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
    )
}