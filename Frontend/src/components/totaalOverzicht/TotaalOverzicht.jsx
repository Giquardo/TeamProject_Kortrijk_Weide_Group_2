import React from "react";
import "./TotaalOverzicht.css";
import overzicht from "../../Images/TotaalOverzicht.png";

const TotaalOverzicht = () => {
  return (
    <>
      <h1 className="title">Energie Verbruik Kortrijk Weide</h1>
      <div className="overzicht-container">
        <img className="image" src={overzicht} alt="overzicht" />
      </div>
    </>
  );
};

export default TotaalOverzicht;