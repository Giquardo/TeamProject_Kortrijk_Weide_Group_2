import React from "react";
import "./hernieuwbareEnergie.css";
import zonneEnergie from "../../Images/zonneEnergie.png";

const HernieuwbareEnergieLayout = ({ info }) => {
  return (
    <>
      <div className="hernieuwbareEnergie-container">
        <h1 className="hernieuwbareEnergie-title">{info.title}</h1>
        <img className="energie-image" src={zonneEnergie} alt="zonneEnergie" />
      </div>
    </>
  );
};

export default HernieuwbareEnergieLayout;