import React from "react";
import "./hernieuwbareEnergie.css";
import zonneEnergie from "../../Images/zonneEnergie.jpg";
import HowestCorner from "../howestCorner/HowestCorner";

const HernieuwbareEnergieLayout = () => {
  return (
    <>
      <div className="container">
        <h1 className="titel">Zonne energie</h1>
        <img className="image" src={zonneEnergie} alt="zonneEnergie" />
        <HowestCorner />
      </div>
    </>
  );
};

export default HernieuwbareEnergieLayout;
