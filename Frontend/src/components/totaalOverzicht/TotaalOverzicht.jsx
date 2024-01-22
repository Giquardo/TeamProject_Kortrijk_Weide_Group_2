import React from "react";
import "./TotaalOverzicht.css";
import overzicht from "../../Images/TotaalOverzicht.png";

const TotaalOverzicht = () => {
  return (
    <>
      <h1 className="totaalOverzicht-title">Energie Verbruik Kortrijk Weide</h1>
      <div className="totaalOverzicht-container">
        <img className="totaalOverzicht-image" src={overzicht} alt="overzicht" />
      </div>
    </>
  );
};

export default TotaalOverzicht;