import { useNavigate } from "react-router-dom";

export default function AboutPage() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen p-6 pt-0">
            <div className="w-full mx-auto">
                {/* Main Card */}
                <div className="bg-[#1a1a2e] rounded-lg shadow-2xl overflow-hidden border border-[#2a2a3e] mb-6">
                    {/* Header */}
                    <div className="bg-gradient-to-r from-[#000030] to-[#1a1a3e] px-6 py-4 border-b border-[#2a2a3e]">
                        <h1 className="text-4xl font-bold text-[#F8FAFC] font-heading text-center">
                            Sobre Geo Col Explorer
                        </h1>
                    </div>

                    {/* Content */}
                    <div className="p-8 space-y-6">
                        {/* Introduction */}
                        <section>
                            <h2 className="text-2xl font-semibold text-[#FFFF00] font-heading mb-4">
                                ¿Qué es Geo Col Explorer?
                            </h2>
                            <p className="text-[#F8FAFC] font-body text-lg leading-relaxed">
                                Geo Col Explorer es una herramienta interactiva diseñada para explorar y visualizar 
                                la estructura territorial de Colombia de manera jerárquica y organizada. Esta aplicación 
                                te permite navegar desde los departamentos hasta las subdivisiones más específicas, 
                                proporcionando una visión completa de la organización geográfica del país.
                            </p>
                        </section>

                        {/* How it works */}
                        <section>
                            <h2 className="text-2xl font-semibold text-[#FFFF00] font-heading mb-4">
                                ¿Cómo Funciona?
                            </h2>
                            <div className="space-y-4">
                                <div className="bg-[#0f0f1e] rounded-lg p-4 border border-[#2a2a3e]">
                                    <h3 className="text-xl font-semibold text-[#F8FAFC] font-heading mb-2">
                                        1. Departamentos
                                    </h3>
                                    <p className="text-gray-400 font-body">
                                        Comienza seleccionando un departamento de Colombia. Colombia cuenta con 32 departamentos 
                                        y un Distrito Capital, cada uno con características únicas y su propia organización territorial.
                                    </p>
                                </div>

                                <div className="bg-[#0f0f1e] rounded-lg p-4 border border-[#2a2a3e]">
                                    <h3 className="text-xl font-semibold text-[#F8FAFC] font-heading mb-2">
                                        2. Municipios
                                    </h3>
                                    <p className="text-gray-400 font-body">
                                        Una vez seleccionado un departamento, podrás explorar todos los municipios que lo conforman. 
                                        Cada municipio representa una unidad administrativa fundamental en la estructura 
                                        territorial colombiana.
                                    </p>
                                </div>

                                <div className="bg-[#0f0f1e] rounded-lg p-4 border border-[#2a2a3e]">
                                    <h3 className="text-xl font-semibold text-[#F8FAFC] font-heading mb-2">
                                        3. Subdivisiones Territoriales
                                    </h3>
                                    <p className="text-gray-400 font-body mb-3">
                                        Al seleccionar un municipio, tendrás acceso a diferentes tipos de subdivisiones territoriales:
                                    </p>
                                    <ul className="list-disc list-inside space-y-2 text-gray-400 font-body ml-4">
                                        <li><span className="text-[#FFFF00] font-semibold">Comunas y Barrios:</span> Divisiones urbanas dentro de los municipios</li>
                                        <li><span className="text-[#FFFF00] font-semibold">Centros Poblados:</span> Asentamientos humanos con características específicas</li>
                                        <li><span className="text-[#FFFF00] font-semibold">Veredas:</span> Unidades territoriales rurales tradicionales</li>
                                        <li><span className="text-[#FFFF00] font-semibold">Corregimientos:</span> Divisiones administrativas menores en áreas rurales</li>
                                    </ul>
                                </div>
                            </div>
                        </section>

                        {/* Features */}
                        <section>
                            <h2 className="text-2xl font-semibold text-[#FFFF00] font-heading mb-4">
                                Características
                            </h2>
                            <div className="grid md:grid-cols-2 gap-4">
                                <div className="bg-[#0f0f1e] rounded-lg p-4 border border-[#2a2a3e]">
                                    <h3 className="text-lg font-semibold text-[#F8FAFC] font-heading mb-2">
                                        🗺️ Navegación Intuitiva
                                    </h3>
                                    <p className="text-gray-400 font-body text-sm">
                                        Interfaz clara y fácil de usar que permite explorar la geografía colombiana de forma natural.
                                    </p>
                                </div>
                                <div className="bg-[#0f0f1e] rounded-lg p-4 border border-[#2a2a3e]">
                                    <h3 className="text-lg font-semibold text-[#F8FAFC] font-heading mb-2">
                                        ⚡ Rendimiento Optimizado
                                    </h3>
                                    <p className="text-gray-400 font-body text-sm">
                                        Carga rápida de datos con sistema de caché para una experiencia fluida.
                                    </p>
                                </div>
                                <div className="bg-[#0f0f1e] rounded-lg p-4 border border-[#2a2a3e]">
                                    <h3 className="text-lg font-semibold text-[#F8FAFC] font-heading mb-2">
                                        📊 Datos Actualizados
                                    </h3>
                                    <p className="text-gray-400 font-body text-sm">
                                        Información geográfica precisa y actualizada sobre la estructura territorial de Colombia.
                                    </p>
                                </div>
                                <div className="bg-[#0f0f1e] rounded-lg p-4 border border-[#2a2a3e]">
                                    <h3 className="text-lg font-semibold text-[#F8FAFC] font-heading mb-2">
                                        🔍 Búsqueda Avanzada
                                    </h3>
                                    <p className="text-gray-400 font-body text-sm">
                                        Capacidades de búsqueda que permiten encontrar rápidamente cualquier entidad territorial.
                                    </p>
                                </div>
                            </div>
                        </section>

                        {/* Technology */}
                        <section>
                            <h2 className="text-2xl font-semibold text-[#FFFF00] font-heading mb-4">
                                Tecnología
                            </h2>
                            <div className="bg-[#0f0f1e] rounded-lg p-4 border border-[#2a2a3e]">
                                <p className="text-gray-400 font-body">
                                    Geo Col Explorer está construido con tecnologías modernas incluyendo React, TypeScript, 
                                    y una API REST robusta. La aplicación utiliza PostgreSQL para el almacenamiento de datos 
                                    geográficos y Redis para optimizar el rendimiento mediante caché distribuido.
                                </p>
                            </div>
                        </section>

                        {/* Action Buttons */}
                        <div className="flex flex-col sm:flex-row gap-4 justify-center items-center pt-6 border-t border-[#2a2a3e]">
                            <button
                                onClick={() => navigate("/")}
                                className="px-6 py-3 bg-gradient-to-r from-[#000030] to-[#1a1a3e] text-[#FFFF00] font-semibold rounded-lg border border-[#FFFF00] transition-all duration-300 shadow-lg hover:shadow-[#FFFF00]/10 cursor-pointer"
                            >
                                Comenzar a Explorar
                            </button>
                            <button
                                onClick={() => navigate(-1)}
                                className="px-6 py-3 bg-[#1a1a2e] text-[#F8FAFC] font-semibold rounded-lg border border-[#2a2a3e] hover:bg-[#252540] hover:border-[#FFFF00] transition-all duration-300 cursor-pointer"
                            >
                                Volver
                            </button>
                        </div>
                    </div>
                </div>

                {/* Footer Info */}
                <div className="text-center">
                    <p className="text-sm text-gray-500 font-body">
                        Geo Col Explorer - Explorando la geografía de Colombia
                    </p>
                </div>
            </div>
        </div>
    );
}

