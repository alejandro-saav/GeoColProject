// import {geoApiService} from "../../Services/Api.tsx";
// import {useEffect, useState} from "react";
// import type {DepartamentosDto} from "../../Services/Types.tsx";
// import DataTable from "../Common/DataTable.tsx";
//
// interface DepartamentoProps {
//     departamentoId?: number;
// }
//
// export default function Departamentos({departamentoId}: DepartamentoProps) {
//    
//     const [departamentos, setDepartamentos] = useState<DepartamentosDto[]>([]);
//     const [municipioId, setMunicipioId] = useState<number | null>(null);
//     const [loading, setLoading] = useState<boolean>(true);
//    
//     function fetchMunicipios(municipioId:number): void {
//         setMunicipioId(municipioId);
//     }
//
//     useEffect(() => {
//         async function fetchDepartamentos(){
//             try {
//                 setLoading(true);
//                 const deps = await geoApiService.getDepartamentos();
//                 setDepartamentos(deps);
//             } catch (error) {
//                 console.error("Error fetching departamentos:", error);
//             } finally {
//                 setLoading(false);
//             }
//         }
//         fetchDepartamentos();
//     },[]);
//    
//     const sortedDepartamentos = departamentos.length > 0 
//         ? [...departamentos].sort((a, b) => a.id - b.id)
//         : [];
//    
//     return (
//        
//                 {/* Header */}
//               
//
//                 {/* Loading State */}
//               
//             </div>
//         </div>
//     );
// }