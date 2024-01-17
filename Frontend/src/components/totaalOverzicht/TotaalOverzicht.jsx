import React from "react";
import "./TotaalOverzicht.css";
import overzicht from "../../Images/Screenshot 2024-01-12 083305.png";
import HowestCorner from "../howestCorner/HowestCorner";

const TotaalOverzicht = () => {
  return (
    <>
      <div className="overzicht-container">
        <h1 className="overzicht-titel">Kortrijk Weide</h1>
        <img className="image" src={overzicht} alt="overzicht" />
      </div>
      <HowestCorner className="corner" />
    </>
  );
};

export default TotaalOverzicht;
