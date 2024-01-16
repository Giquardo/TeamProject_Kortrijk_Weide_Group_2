import "./App.css";
import UitlegEnergieVermogen from "./components/uitlegEnergieVermogen/UitlegEnergieVermogen.jsx";
//import TotaalOverzicht from "./components/totaalOverzicht/TotaalOverzicht.jsx";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        {/* <TotaalOverzicht /> */}
        <UitlegEnergieVermogen />
      </header>
    </div>
  );
}

export default App;
