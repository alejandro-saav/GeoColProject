import { BsFiletypeCsv, BsFiletypeJson } from "react-icons/bs";
import {geoApiService} from "../Services/Api.tsx";

export default function Descargas() {
    const headers: string[] = ["Todo el conjunto de datos","los Departamentos de Colombia", "los Municipios de Colombia", "las Comunas y Barrios de Colombia", " los Corregimientos de Colombia", "los Centros Poblados de Colombia","las Veredas de Colombia"];
    
    async function fetchData():Promise<any> {
        const blob = await geoApiService.getDepartamentosExcel();
        if (blob == null) return;
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = `departamentos_${new Date().toISOString().slice(0,10)}.csv`;
        link.click();
        window.URL.revokeObjectURL(url);
    }
    return (
        <div>
            <div className="p-4">
                <h1 className="text-4xl font-bold text-[#F8FAFC] font-heading text-center">Descarga de datos en formatos CSV y JSON</h1>
            </div>
            {headers.map((item:string, index:number) => (
            <div className="p-2 pl-12" key={index}>
                <div className="grid grid-cols-[1fr_0.2fr] p-2 w-1/2 bg-gradient-to-r from-[#000030] to-[#1a1a3e]  text-2xl rounded-xl">
                    <h1>Descarga de {item}</h1>
                    <div className="flex gap-2 text-end justify-end">
                        <BsFiletypeCsv size={28} className={`cursor-pointer hover:text-[#FFFF00]`} onClick={() => fetchData()}/>
                        <BsFiletypeJson size={28} className="cursor-pointer hover:text-[#FFFF00]"/>
                    </div>
                </div>
            </div>
                ))}
        </div>
    )
}