import React, { useState, useEffect } from "react";
import "./EnergieVerbruikTotaalOverzicht.css";
import overzicht from "../../Images/TotaalOverzicht.png";

const TotaalOverzicht = () => {

  const [data, setData] = useState({});

  useEffect(() => {
    // Replace with your HTTP trigger
    fetch('http://localhost:5000/api/data/overview/weekly')
      .then(response => response.json())
      .then(data => setData(data.weeklyoverview[0])); // Access the first object in the array
      console.log(data);
  }, []);
  
  return (
    <>
      <h1 className="title">Kortrijk Weide</h1>
      <div className="overzicht-container">
        <img className="image" src={overzicht} alt="overzicht" />
        <div className="textbox textbox1">
          Totaal Verbruik
          <div>{data.consumption} kW</div>
        </div>
        <div className="textbox textbox2">
          Eigen Verbruik
          <div>{data.totalProductieEigenVerbruik} kW</div>
        </div>
        <div className="textbox textbox3">
          Totaal Opbrengst
          <div>{data.totalProductionProfit} kW</div>        
        </div>
      </div>
    </>
  );
};

export default TotaalOverzicht;