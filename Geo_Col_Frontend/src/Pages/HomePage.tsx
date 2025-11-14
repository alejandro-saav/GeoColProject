import {useEffect, useState} from "react";
import type {DataFor, DepartamentosDto, MunicipioDto, MunicipiosPorDepartamentoDto} from "../Services/Types.tsx";
import {geoApiService} from "../Services/Api.tsx";
import DataTable from "../Components/Common/DataTable.tsx";
import {useLocation} from "react-router-dom";
import SkeletonLoadingDataTable from "../Components/Common/SkeletonLoadingDataTable.tsx";
import SeleccionUltimoNivelTerritorial from "../Components/Visualization/SeleccionUltimoNivelTerritorial.tsx";
import {fetchers} from "../Services/Fetchers.tsx";
import type {TerritorialLevel} from "../Constants/TerritorialLevels.tsx";
import {formatTerritorialLabel} from "../Utils/Formatters.tsx";

export default function HomePage(){
    // DATA STATES
    const [departamentos, setDepartamentos] = useState<DepartamentosDto[]>([]);
    const [municipios, setMunicipios] = useState<MunicipioDto[]>([]);
    const [ultimaDivisionTerritorialData, setUltimaDivisionTerritorialData] = useState<DataFor<TerritorialLevel>>();
    // SELECT ENTITY STATES
    const [departamentoSeleccionado, setDepartamentoSeleccionado] = useState<DepartamentosDto | null>(null);
    const [municipioSeleccionado, setMunicipioSeleccionado] = useState<MunicipioDto | null>(null);
    const [subdivisionSeleccionada, setSubdivisionSeleccionada] = useState<TerritorialLevel | null>(null);
    // LOADING STATES
    const [loadingDepartamentos, setLoadingDepartamentos] = useState<boolean>(true);
    const [loadingMunicipios, setLoadingMunicipios] = useState<boolean>(true);
    const [loadingSubdivision, setLoadingSubdivision] = useState<boolean>(true);
    
    const loc = useLocation();
    const path :string[] = loc.pathname.split("/").filter(Boolean);
    console.log(path);
    
    useEffect(() => {
        async function fetchDepartamentos(){
            try {
                setLoadingDepartamentos(true);
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
                setLoadingDepartamentos(false);
            }
        }
        fetchDepartamentos();
    },[]);
    
    useEffect(() => {
        async function fetchMunicipios(){
            try {
                setLoadingMunicipios(true);
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
                setLoadingMunicipios(false);
            }
        }
        fetchMunicipios();
    }, [departamentoSeleccionado])
    
    useEffect(() => {
        setLoadingSubdivision(true);
        async function fetchSubdivision(){
            try {
                if (subdivisionSeleccionada == null || municipioSeleccionado == null) throw new Error("subdivisionSeleccionada o municipioSeleccionado es null");
                console.log("SUB:", subdivisionSeleccionada);
                const data = await fetchers[subdivisionSeleccionada](municipioSeleccionado?.id);
                console.log("DATA:", data);
                setUltimaDivisionTerritorialData(data);
            } catch (error) {
                console.log("Error fetching ultima subdivision:", error);
            } finally {
                setLoadingSubdivision(false);
            }
        }
        fetchSubdivision();
    }, [subdivisionSeleccionada])
    console.log(departamentos);
    console.log(municipios);
    return(
    <div className="flex">
        {loadingDepartamentos ? <SkeletonLoadingDataTable /> : 
        <DataTable header={"Departamentos de Colombia"} entityName={"Departamentos"} data={departamentos.sort((a, b) => a.id - b.id)} onClickHandler={setDepartamentoSeleccionado} />
        }
        {departamentoSeleccionado != null ? loadingMunicipios ? <SkeletonLoadingDataTable /> :
        <DataTable header={`Municipios de ${departamentoSeleccionado?.departamento}`} entityName={"Municipios"} data={municipios.sort((a, b) => a.id - b.id)} onClickHandler={setMunicipioSeleccionado} />
        : null }
        {municipioSeleccionado != null ? <SeleccionUltimoNivelTerritorial onClickHandler={setSubdivisionSeleccionada}/> : null}
        {subdivisionSeleccionada != null ? loadingSubdivision ? <SkeletonLoadingDataTable /> : <DataTable header={formatTerritorialLabel(subdivisionSeleccionada)} entityName={formatTerritorialLabel(subdivisionSeleccionada)} data={ultimaDivisionTerritorialData} /> : null}
    </div>
        )
}