import React from "react";
import "./UitlegEnergieVermogen.css";
import uitleg from "../../Images/uitlegEnergieVermogen.png";

const UitlegEnergieVermogen = () => {
  return (
    <>
      <h1 className="title">Energie en vermogen</h1>
      <div className="uitleg-container">
        <img className="uitleg-image" src={uitleg} alt="uitlegEnergieEnVermogen" />
        <div className="uitleg-text uitleg-text1">Energie = volume van het water</div>
        <div className="uitleg-text uitleg-text2">Vermogen = snelheid van het water</div>
      </div>
    </>
  );
};

export default UitlegEnergieVermogen;