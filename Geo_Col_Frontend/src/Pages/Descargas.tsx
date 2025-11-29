import { BsFiletypeCsv, BsFiletypeJson } from "react-icons/bs";
import {geoApiService} from "../Services/Api.tsx";
import { DownloadOptions } from "../Constants/DescargasOptions.tsx";
import type {DownloadOptionsType} from "../Constants/DescargasOptions.tsx";
import {useState} from "react";

export default function Descargas() {

    const [loadingJsonIcon, setLoadingJsonIcon] = useState<number[]>([]);
    const [loadingCsvIcon, setLoadingCsvIcon] = useState<number[]>([]);
    
    async function fetchData(endpointRelativeUrl: string, dataExportExtension: string, itemIndex: number):Promise<void> {
        if (dataExportExtension === ".csv") {
            setLoadingCsvIcon(prev => [...prev, itemIndex]);
        } else if (dataExportExtension === ".json") {
            setLoadingJsonIcon(prev => [...prev, itemIndex]);
        }
        const blob: Blob | null = await geoApiService.getExportedData(endpointRelativeUrl+dataExportExtension);
      
        // if null i need to show an error messsage here
        if (blob == null) return;
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        if (dataExportExtension === ".csv") {
            setLoadingCsvIcon(prev => prev.filter(item => item !== itemIndex));
        } else if (dataExportExtension === ".json") {
            setLoadingJsonIcon(prev => prev.filter(item => item !== itemIndex));
        }
        link.download = `${endpointRelativeUrl}_${new Date().toISOString().slice(0,10)}${dataExportExtension}`;
        link.click();
        window.URL.revokeObjectURL(url);
    }
    return (
        <div>
            <div className="p-4">
                <h1 className="text-4xl font-bold text-[#F8FAFC] font-heading text-center">Descarga de datos en formatos CSV y JSON</h1>
            </div>
            {DownloadOptions.map((item:DownloadOptionsType, index:number) => (
            <div className="p-2 pl-12" key={index}>
                <div className="grid grid-cols-[1fr_0.2fr] p-2 w-1/2 bg-gradient-to-r from-[#000030] to-[#1a1a3e]  text-2xl rounded-xl">
                    <h1>Descarga de {item.header}</h1>
                    <div className="flex gap-2 text-end justify-end">
                        {loadingCsvIcon.includes(index) ?
                            <div className="flex justify-center items-center w-7 h-7"><div className="loader"></div></div> :
                        <BsFiletypeCsv size={28} className={`cursor-pointer hover:text-[#FFFF00]`} onClick={() => fetchData(item.relativeUrl, ".csv", index)}/>
                        }
                        {loadingJsonIcon.includes(index) ?  <div className="flex justify-center items-center w-7 h-7"><div className="loader"></div></div>:
                        <BsFiletypeJson size={28} className="cursor-pointer hover:text-[#FFFF00]" onClick={() => fetchData(item.relativeUrl, ".json", index)}/>
                        }
                    </div>
                </div>
            </div>
                ))}
                    <div className="dot-pulse"></div>
        </div>
    )
}