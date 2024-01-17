import "./App.css";
import Quiz from "./components/quiz/Quiz.jsx";
import UitlegEnergieVermogen from "./components/uitlegEnergieVermogen/UitlegEnergieVermogen.jsx";
import TotaalOverzicht from "./components/totaalOverzicht/TotaalOverzicht.jsx";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        {/* <TotaalOverzicht /> */}
        <UitlegEnergieVermogen />
        {/* <Quiz /> */}
      </header>
    </div>
  );
}

export default App;
