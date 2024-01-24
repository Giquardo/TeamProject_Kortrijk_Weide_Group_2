import React from "react";
import "./TotaalOverzicht.css";
import overzicht from "../../Images/TotaalOverzicht.png";
import "../General.css";

const TotaalOverzicht = () => {
  return (
    <>
      <h1 className="title">Energie Verbruik Kortrijk Weide</h1>
      <div className="totaalOverzicht-container">
        <img
          className="totaalOverzicht-image"
          src={overzicht}
          alt="overzicht"
        />
      </div>
    </>
  );
};

export default TotaalOverzicht;
