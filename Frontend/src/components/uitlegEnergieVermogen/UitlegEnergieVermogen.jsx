import React from "react";
import "./UitlegEnergieVermogen.css";
import "../General.css";
import uitleg from "../../Images/uitlegEnergieVermogen.png";

const UitlegEnergieVermogen = () => {
  return (
    <>
      <h1 className="title">Energie en vermogen</h1>
      <div className="uitleg-container">
        <img
          className="uitleg-image"
          src={uitleg}
          alt="uitlegEnergieEnVermogen"
        />
        <div className="uitleg-text uitleg-text1">
          Energie = hoeveelheid opgevangen water onderaan de dam
        </div>
        <div className="uitleg-text uitleg-text2">
          Vermogen = stroomsnelheid van het water
        </div>
      </div>
    </>
  );
};

export default UitlegEnergieVermogen;
