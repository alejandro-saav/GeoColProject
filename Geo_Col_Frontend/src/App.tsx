import './App.css'
import HomePage from "./Pages/HomePage.tsx";
import Header from "./Components/Common/Header.tsx"
import {BrowserRouter, Routes, Route} from "react-router-dom";

function App() {

  return (
    <BrowserRouter>
        <Header />
        <Routes>
            <Route path="/" element={<HomePage />} />
        </Routes>
    </BrowserRouter>
  )
}

export default App
