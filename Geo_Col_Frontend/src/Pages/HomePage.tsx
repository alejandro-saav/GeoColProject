import {useEffect, useState, useMemo, useCallback} from "react";
import type {DataFor, DepartamentosDto, MunicipioDto, MunicipiosPorDepartamentoDto} from "../Services/Types.tsx";
import {geoApiService} from "../Services/Api.tsx";
import DataTable from "../Components/Common/DataTable.tsx";
import { useNavigate, useParams} from "react-router-dom";
import SkeletonLoadingDataTable from "../Components/Common/SkeletonLoadingDataTable.tsx";
import SeleccionUltimoNivelTerritorial from "../Components/Visualization/SeleccionUltimoNivelTerritorial.tsx";
import {fetchers} from "../Services/Fetchers.tsx";
import {TERRITORIAL_LEVELS, type TerritorialLevel} from "../Constants/TerritorialLevels.tsx";
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
    
    const { depId, munId, subdivision } = useParams();
    
    const navigate = useNavigate();
    
    const handleDepartamentoSeleccionado = useCallback((departamento: DepartamentosDto):void => {
        setDepartamentoSeleccionado(departamento);
        setMunicipioSeleccionado(null);
        setMunicipios([]);
        setSubdivisionSeleccionada(null);
        setUltimaDivisionTerritorialData([]);
        navigate(`/departamentos/${departamento.id}`, { replace: true });
    }, [navigate]);
    
    const handleMunicipioSeleccionado = useCallback((municipio: MunicipioDto):void => {
        setSubdivisionSeleccionada(null);
        setMunicipioSeleccionado(municipio);
        setUltimaDivisionTerritorialData([]);
        navigate(`/departamentos/${departamentoSeleccionado?.id}/municipios/${municipio.id}`);
    }, [departamentoSeleccionado, navigate]);
    
    const handleSubdivisionSeleccionada = useCallback((sub:TerritorialLevel):void => {
        setSubdivisionSeleccionada(sub);
        navigate(`/departamentos/${departamentoSeleccionado?.id}/municipios/${municipioSeleccionado?.id}/${sub}`);
    }, [departamentoSeleccionado, municipioSeleccionado, navigate]);
    
    // DEPARTAMENTOS FETCHING LOGIC
    useEffect(() => {
        async function fetchDepartamentos(){
            try {
                setLoadingDepartamentos(true);
                
                const deps: DepartamentosDto[] = await geoApiService.getDepartamentos();
                setDepartamentos(deps);
                if (depId != undefined && !Number.isNaN(Number(depId))) {
                    const departamentoEncontrado = deps.find(dp => dp.id === Number(depId));
                    if (departamentoEncontrado != undefined) {
                        setDepartamentoSeleccionado(departamentoEncontrado);
                    }
                }
            } catch (error) {
                console.error("Error fetching departamentos:", error);
            } finally {
                setLoadingDepartamentos(false);
            }
        }
        fetchDepartamentos();
    },[]);

    // MUNICIPIOS FETCHING LOGIC
    useEffect(() => {
        async function fetchMunicipios(){
            try {
                if (!departamentoSeleccionado) return;
                setLoadingMunicipios(true);
                const muns: MunicipiosPorDepartamentoDto = await geoApiService.getMunicipiosPorDepartamento(departamentoSeleccionado.id);
                    setMunicipios(muns.municipios);
                    
                //     CHECK MUNICIPIO ID PARAM
                if (munId != undefined && !Number.isNaN(Number(munId))) {
                    const municipioEncontrado = muns.municipios.find(mun => mun.id === Number(munId));
                    if (municipioEncontrado != undefined) {
                        setMunicipioSeleccionado(municipioEncontrado);
                    }
                }
            } catch (error) {
                console.error("Error fetching municipios:", error);
            } finally {
                setLoadingMunicipios(false);
            }
        }
        fetchMunicipios();
    }, [departamentoSeleccionado])

    // SUBDIVISION FETCHING LOGIC
    useEffect(() => {
        setLoadingSubdivision(true);

        // Clear subdivision if URL param is missing or invalid
        if (subdivision == undefined || !TERRITORIAL_LEVELS.includes(subdivision as any)) {
            setSubdivisionSeleccionada(null);
            setUltimaDivisionTerritorialData(undefined);
            setLoadingSubdivision(false);
            return;
        }

        // Set state from URL param
        const sub = TERRITORIAL_LEVELS.find(s => s == subdivision);
        if (sub != undefined) {
            setSubdivisionSeleccionada(sub);
        }

        async function fetchSubdivision(){
            try {
                if (sub == null || municipioSeleccionado == null) {
                    setLoadingSubdivision(false);
                    return;
                }
                const data = await fetchers[sub](municipioSeleccionado.id);
                const objectKeys: string[] = Object.keys(data);
                setUltimaDivisionTerritorialData(data[objectKeys[2]]);
            } catch (error) {
                console.log("Error fetching ultima subdivision:", error);
            } finally {
                setLoadingSubdivision(false);
            }
        }
        fetchSubdivision();
    }, [municipioSeleccionado, subdivision]);
    
    // Memoize sorted arrays to avoid sorting on every render
    const sortedDepartamentos = useMemo(() => {
        return [...departamentos].sort((a, b) => a.id - b.id);
    }, [departamentos]);
    
    const sortedMunicipios = useMemo(() => {
        return [...municipios].sort((a, b) => a.id - b.id);
    }, [municipios]);
    
    // Memoize formatted label to avoid calling function twice
    const subdivisionLabel = useMemo(() => {
        return subdivisionSeleccionada ? formatTerritorialLabel(subdivisionSeleccionada) : "";
    }, [subdivisionSeleccionada]);
    return(
    <div className="flex">
        {/* SHOW DEPARTAMENTOS */}
        {loadingDepartamentos ? <SkeletonLoadingDataTable /> : 
        <DataTable header={"Departamentos de Colombia"} entityName={"Departamentos"} data={sortedDepartamentos}  onClickHandler={handleDepartamentoSeleccionado} entidadSeleccionada={departamentoSeleccionado}/>
        }
        {/* SHOW MUNICIPIOS */}
        {departamentoSeleccionado != null ? loadingMunicipios ? <SkeletonLoadingDataTable /> :
        <DataTable header={`Municipios de ${departamentoSeleccionado?.departamento}`} entityName={"Municipios"} data={sortedMunicipios} onClickHandler={handleMunicipioSeleccionado} entidadSeleccionada={municipioSeleccionado}/>
        : null }
        {/* SHOW SUBDIVISIONES OPCIONES */}
        {municipioSeleccionado != null && !loadingMunicipios? <SeleccionUltimoNivelTerritorial onClickHandler={handleSubdivisionSeleccionada} entidadSeleccionada={subdivisionSeleccionada}/> : null}
        {/* SHOW SUBDIVISION DATA */}
        {subdivisionSeleccionada != null ? loadingSubdivision ? <SkeletonLoadingDataTable /> : <DataTable header={subdivisionLabel} entityName={subdivisionLabel} data={ultimaDivisionTerritorialData} /> : null}
    </div>
        )
}