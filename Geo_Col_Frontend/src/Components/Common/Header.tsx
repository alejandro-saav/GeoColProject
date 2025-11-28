import { useNavigate, useLocation } from "react-router-dom";

export default function Header() {
    const navigate = useNavigate();
    const location = useLocation();

    return (
        <div className="flex justify-around items-center w-full p-3 shadow-2xs bg-[#1a1a2e] border-b border-[#2a2a3e]">
            <div className="flex-1"></div>
            <div className="flex-1 text-center">
                <h1 
                    className="text-2xl text-yellow-500 cursor-pointer hover:text-[#FFFF00] transition-colors"
                    onClick={() => navigate("/")}
                >
                    Geo Col Explorer
                </h1>
            </div>
            <div className="flex-1 pr-5">
                <ul className="flex justify-end gap-5 [*]:cursor-pointer [&>li]:hover:text-white">
                    <li 
                        onClick={() => navigate("/")}
                        className={location.pathname === "/" ? "text-[#FFFF00]" : "text-gray-400"}
                    >
                        Explorar
                    </li>
                    <li className="text-gray-400 opacity-50">
                        Descargas
                    </li>
                    <li 
                        onClick={() => navigate("/sobre")}
                        className={location.pathname === "/sobre" || location.pathname === "/about" ? "text-[#FFFF00]" : "text-gray-400"}
                    >
                        Sobre esta pagina
                    </li>
                </ul>
            </div>
        </div>
    )
}