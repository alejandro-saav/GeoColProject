import {useEffect, useState} from "react";
import type {DepartamentosDto, MunicipioDto, MunicipiosPorDepartamentoDto} from "../Services/Types.tsx";
import {geoApiService} from "../Services/Api.tsx";
import DataTable from "../Components/Common/DataTable.tsx";
import {useLocation} from "react-router-dom";
import SkeletonLoadingDataTable from "../Components/Common/SkeletonLoadingDataTable.tsx";

export default function HomePage(){
    const [departamentos, setDepartamentos] = useState<DepartamentosDto[]>([]);
    const [departamentoSeleccionado, setDepartamentoSeleccionado] = useState<DepartamentosDto | null>(null);
    const [municipios, setMunicipios] = useState<MunicipioDto[]>([]);
    const [municipiosSeleccionado, setMunicipioSeleccionado] = useState<MunicipioDto | null>(null);
    // const [loading, setLoading] = useState<boolean>(true);
    const loc = useLocation();
    const path :string[] = loc.pathname.split("/").filter(Boolean);
    console.log(path);
    
    useEffect(() => {
        async function fetchDepartamentos(){
            try {
                // setLoading(true);
                if (path.length > 1 && (isNaN(Number(path[1])) || Number(path[1]) > 50)) throw new Error("departamento id invalido");
                const depId : number | undefined = path.length > 1 ? Number(path[1]) : undefined;
                const deps: DepartamentosDto | DepartamentosDto[] = await geoApiService.getDepartamentos(depId);
                if (!Array.isArray(deps)) {
                    setDepartamentoSeleccionado(deps);
                }
                setDepartamentos(Array.isArray(deps) ? deps : [deps]);
            } catch (error) {
                console.error("Error fetching departamentos:", error);
            } finally {
                // setLoading(false);
            }
        }
        fetchDepartamentos();
    },[]);
    
    useEffect(() => {
        async function fetchMunicipios(){
            try {
                // setLoading(true);
                if (!departamentoSeleccionado) return;
                if (path.length > 3 && (isNaN(Number(path[3])))) throw new Error("municipio id invalido");
                const munId : number | undefined = path.length > 3 ? Number(path[3]) : undefined;
                if (munId != undefined) {
                    const mun: MunicipioDto = await geoApiService.getMunicipio(munId);
                    setMunicipioSeleccionado(mun);
                } else {
                const muns: MunicipioDto | MunicipiosPorDepartamentoDto = await geoApiService.getMunicipiosPorDepartamento(departamentoSeleccionado.id);
                    setMunicipios(muns.municipios);
                }
            } catch (error) {
                console.error("Error fetching municipios:", error);
            } finally {
                // setLoading(false);
            }
        }
        fetchMunicipios();
    }, [departamentoSeleccionado])
    console.log(departamentos);
    console.log(municipios);
    return(
    <div className="flex">
        <DataTable header={"Departamentos de Colombia"} entityName={"Departamentos"} data={departamentos.sort((a, b) => a.id - b.id)} onClickHandler={setDepartamentoSeleccionado}/>
        {departamentoSeleccionado != null ? 
        <DataTable header={`Municipios de ${departamentoSeleccionado?.departamento}`} entityName={"Municipios"} data={municipios.sort((a, b) => a.id - b.id)} onClickHandler={setMunicipioSeleccionado}/>
        : null}
        <SkeletonLoadingDataTable />
    </div>
        )
}