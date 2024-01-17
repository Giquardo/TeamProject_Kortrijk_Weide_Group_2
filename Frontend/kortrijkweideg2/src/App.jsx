import "./App.css";
import Quiz from "./components/quiz/Quiz.jsx";
import UitlegEnergieVermogen from "./components/uitlegEnergieVermogen/UitlegEnergieVermogen.jsx";
import TotaalOverzicht from "./components/totaalOverzicht/TotaalOverzicht.jsx";
import HernieuwbareEnergieLayout from "./components/hernieuwbareEnergie/hernieuwbareEnergieLayout.jsx";
import Navbar from "./components/navigator/Navbar.jsx";
import { Routes, Route } from "react-router-dom";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        {/* <TotaalOverzicht /> */}
        {/* <UitlegEnergieVermogen /> */}
        {/* <Quiz /> */}
        {/* <HernieuwbareEnergieLayout /> */}

        <Routes>
          <Route path="/" element={<TotaalOverzicht />} />
        </Routes>
        <Navbar />
      </header>
    </div>
  );
}

export default App;
