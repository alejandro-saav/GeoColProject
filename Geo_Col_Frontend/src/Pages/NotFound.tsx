import { useNavigate } from "react-router-dom";

export default function NotFound() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen flex items-center justify-center p-6">
            <div className="w-full max-w-xl mx-auto">
                <div className="bg-[#1a1a2e] rounded-lg shadow-2xl overflow-hidden border border-[#2a2a3e]">
                    {/* Header */}
                    <div className="bg-gradient-to-r from-[#000030] to-[#1a1a3e] px-6 py-4 border-b border-[#2a2a3e]">
                        <h1 className="text-4xl font-bold text-[#F8FAFC] font-heading text-center">
                            Página No Encontrada
                        </h1>
                    </div>

                    {/* Content */}
                    <div className="p-12 text-center">
                        {/* 404 Number */}
                        <div className="mb-8">
                            <span className="text-9xl font-bold text-[#FFFF00] font-heading">
                                404
                            </span>
                        </div>

                        {/* Error Message */}
                        <div className="mb-8">
                            <h2 className="text-2xl font-semibold text-[#F8FAFC] font-heading mb-4">
                                Lo sentimos, esta página no existe
                            </h2>
                            <p className="text-gray-400 font-body text-lg">
                                La página que estás buscando no se encuentra disponible o ha sido movida.
                            </p>
                        </div>

                        {/* Action Buttons */}
                        <div className="flex flex-col sm:flex-row gap-4 justify-center items-center">
                            <button
                                onClick={() => navigate("/")}
                                className="px-6 py-3 bg-gradient-to-r from-[#000030] to-[#1a1a3e] text-[#FFFF00] font-semibold rounded-lg border border-[#FFFF00] hover:!bg-[#FFFF00] hover:!text-[#000000] hover:border-[#FFFF00] transition-all duration-300 shadow-lg hover:shadow-[#FFFF00]/50"
                            >
                                Volver al Inicio
                            </button>
                            <button
                                onClick={() => navigate(-1)}
                                className="px-6 py-3 bg-[#1a1a2e] text-[#F8FAFC] font-semibold rounded-lg border border-[#2a2a3e] hover:bg-[#252540] hover:border-[#FFFF00] transition-all duration-300"
                            >
                                Página Anterior
                            </button>
                        </div>

                        {/* Additional Info */}
                        <div className="mt-12 pt-8 border-t border-[#2a2a3e]">
                            <p className="text-sm text-gray-500 font-body">
                                Si crees que esto es un error, por favor contacta al administrador.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

