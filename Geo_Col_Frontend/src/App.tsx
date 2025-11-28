import './App.css'
import HomePage from "./Pages/HomePage.tsx";
import NotFound from "./Pages/NotFound.tsx";
import AboutPage from "./Pages/AboutPage.tsx";
import Header from "./Components/Common/Header.tsx"
import {BrowserRouter, Routes, Route} from "react-router-dom";

function App() {

  return (
    <BrowserRouter>
        <Header />
        <Routes>
            <Route path="/" element={<HomePage />} />

            <Route path="/departamentos" element={<HomePage />} />
            <Route path="/departamentos/:depId" element={<HomePage />} />

            <Route path="/departamentos/:depId/municipios" element={<HomePage />} />
            <Route path="/departamentos/:depId/municipios/:munId" element={<HomePage />} />

            <Route
                path="/departamentos/:depId/municipios/:munId/:subdivision"
                element={<HomePage />}
            />

            <Route path="/sobre" element={<AboutPage />} />
            <Route path="/about" element={<AboutPage />} />

            <Route path="*" element={<NotFound />} />
        </Routes>

    </BrowserRouter>
  )
}

export default App
