export default function Header() {
    return (
        <div className="flex justify-around items-center w-full p-3 shadow-2xs">
            <div className="flex-1"></div>
            <div className="flex-1 text-center">
                <h1 className="text-2xl text-yellow-500">Geo Col Explorer</h1>
            </div>
            <div className="flex-1 pr-5">
                <ul className="flex justify-end gap-5 [*]:cursor-pointer [&>li]:hover:text-white">
                <li>Explorar</li>
                    <li>Descargas</li>
                <li>Sobre esta pagina</li>
            </ul>
            </div>
        </div>
    )
}