import React from "react";
import "./HowestCorner.css";
import logo from "../../Images/Howest_hogeschool-logo_RGB-WIT.png";

const HowestCorner = () => {
  return (
    <div className="slide-container">
      <div className="triangle">
        <img src={logo} alt="Logo" className="logo" />
      </div>
    </div>
  );
};

export default HowestCorner;
