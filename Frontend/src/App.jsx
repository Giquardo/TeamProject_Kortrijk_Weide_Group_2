import "./App.css";
import Quiz from "./components/quiz/Quiz.jsx";
import UitlegEnergieVermogen from "./components/uitlegEnergieVermogen/UitlegEnergieVermogen.jsx";
import TotaalOverzicht from "./components/totaalOverzicht/TotaalOverzicht.jsx";
import HernieuwbareEnergieLayout from "./components/hernieuwbareEnergie/hernieuwbareEnergieLayout.jsx";
import Navbar from "./components/navigator/Navbar.jsx";
import React, { useEffect, useState } from 'react';
import { Routes, Route } from "react-router-dom";

function App() {
  const [data, setData] = useState(null);
  const [buildingSpecificDataKWE_A, setBuildingSpecificDataKWE_A] = useState(null);
  const [buildingSpecificDataLAGO, setBuildingSpecificDataLAGO] = useState(null);

  useEffect(() => {
    // Fetch general data
    fetch('http://localhost:5000/api/data/overview')
      .then(response => response.json())
      .then(data => {
        console.log('Received data:', data);
        setData(data);
      })
      .catch(error => console.error('Error:', error));
  }, []);

  useEffect(() => {
    // Fetch building-specific data for KWE_A
    fetch('http://localhost:5000/api/data/buildingspecific/KWE_A')
      .then(response => response.json())
      .then(data => {
        console.log('Received building-specific data for KWE_A:', data);
        setBuildingSpecificDataKWE_A(data);
      })
      .catch(error => console.error('Error fetching building-specific data for KWE_A:', error));
  }, []);

  useEffect(() => {
    // Fetch building-specific data for LAGO
    fetch('http://localhost:5000/api/data/buildingspecific/LAGO')
      .then(response => response.json())
      .then(data => {
        console.log('Received building-specific data for LAGO:', data);
        setBuildingSpecificDataLAGO(data);
      })
      .catch(error => console.error('Error fetching building-specific data for LAGO:', error));
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <TotaalOverzicht />
        {/* <UitlegEnergieVermogen /> */}
        {/* <Quiz /> */}
        {/* <HernieuwbareEnergieLayout /> */}
        {/*<Routes>
          <Route path="/" element={<TotaalOverzicht />} />
        </Routes>
        <Navbar />*/}
      </header>
    </div>
  );
}

export default App;
