export default function Header() {
    return (
        <div className="flex justify-around w-full p-4 shadow-2xs shadow-blue-300">
            <div className="flex-1"></div>
            <div className="flex-1 text-center">
                <h1>Geo Col Explorer</h1>
            </div>
            <div className="flex-1">
                <ul className="flex justify-end gap-5">
                <li>Explorar</li>
                <li>Sobre esta pagina</li>
            </ul>
            </div>
        </div>
    )
}