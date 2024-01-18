import React from "react";
import "./TotaalOverzicht.css";
import overzicht from "../../Images/TotaalOverzicht.png";
//import HowestCorner from "../howestCorner/HowestCorner";

const TotaalOverzicht = () => {
  return (
    <>
      <div className="overzicht-container">
        <img className="image" src={overzicht} alt="overzicht" />
      </div>
    </>
  );
};

export default TotaalOverzicht;
