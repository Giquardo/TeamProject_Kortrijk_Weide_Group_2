import React from "react";
import "./hernieuwbareEnergie.css";
import zonneEnergie from "../../Images/zonneEnergie.jpg";

const HernieuwbareEnergieLayout = () => {
  return (
    <>
      <div className="container">
        <h1 className="titel">Zonne energie</h1>
        <img className="image" src={zonneEnergie} alt="zonneEnergie" />
      </div>
    </>
  );
};

export default HernieuwbareEnergieLayout;
