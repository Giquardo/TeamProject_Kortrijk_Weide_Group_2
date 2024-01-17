import React from "react";
import "./UitlegEnergieVermogen.css";
import HowestCorner from "../howestCorner/HowestCorner";
import uitleg from "../../Images/uitlegEnergieVermogen.jpg";
import Lottie from "lottie-react";
import arrowWater from "../../animations/arrowWater.json";

const UitlegEnergieVermogen = () => {
  return (
    <div className="container">
      <img src={uitleg} alt="uitlegEnergieEnVermogen" />

      <div className="pijl2">
        <Lottie animationData={arrowWater} loop={true} autoplay={true} />
      </div>
      <div className="pijl1">
        <Lottie animationData={arrowWater} loop={true} autoplay={true} />
      </div>
      <HowestCorner />
    </div>
  );
};

export default UitlegEnergieVermogen;
