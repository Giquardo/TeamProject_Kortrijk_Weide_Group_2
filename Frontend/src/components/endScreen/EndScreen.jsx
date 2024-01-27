import React from "react";
import "./EndScreen.css";
import logo from "../../Images/Logo/Label-Powered-by-Howest_WIT+LIJNTJES.png";
import logo1 from "../../Images/Logo/Opleidingslogo_Howest_Multimedia_en_Creative_Technologies_liggend_WIT.png";
import logo2 from "../../Images/Logo/Opleidingslogo_Howest_Energiemanagement_liggend_WIT.png";
import lago from "../../Images/Logo/lago.png";
import lago2 from "../../Images/Logo/lago logo 2.png";
import lago3 from "../../Images/Logo/lago logo 3.png";
import kortrijk from "../../Images/Logo/02_Kortrijk-logo-web_WIT-pos.png";
import westvlaanderen from "../../Images/Logo/West-Vlaanderen.png";
import ugent from "../../Images/Logo/logo_UGent_CK_NL_RGB_2400_wit.png";
import Background from "../background/Background";

const EndScreen = () => {
  return (
    <>
      <Background />
      <div className="end-screen-container">
        <img src={logo} alt="Logo" className="howest-logo" />
        <p className="collaboration-text">In samenwerking met</p>
        <div className="middle-logos">
          <img src={logo1} alt="Logo 1" className="middle-logo" />
          <p className="and-text">en</p>
          <img src={logo2} alt="Logo 2" className="middle-logo" />
        </div>
        <p className="bottom-text">Onze partners</p>
        <div className="bottom-logos">
          <img src={kortrijk} alt="Kortrijk" className="bottom-logo" />
          <img
            src={westvlaanderen}
            alt="West-Vlaanderen"
            className="bottom-logo"
          />
          <img src={lago2} alt="Lago" className="bottom-logo" />
          <img src={ugent} alt="UGent" className="bottom-logo" />
        </div>
      </div>
    </>
  );
};

export default EndScreen;
