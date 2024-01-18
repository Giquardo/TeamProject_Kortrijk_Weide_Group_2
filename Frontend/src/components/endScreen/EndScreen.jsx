import React from 'react';
import './EndScreen.css';
import logo from '../../Images/Label-Powered-by-Howest_WIT+LIJNTJES.png';
import logo1 from '../../Images/Opleidingslogo_Howest_Multimedia_en_Communicatietechnologie_liggend_WIT.png';
import logo2 from '../../Images/Opleidingslogo_Howest_Energiemanagement_liggend_WIT.png';

const EndScreen = () => {
  return (
    <div className="end-screen-container">
      <img src={logo} alt="Logo" className="howest-logo" />
      <p className="collaboration-text">In samenwerking met</p>
      <div className="middle-logos">
        <img src={logo1} alt="Logo 1" className="middle-logo" />
        <p className="and-text">En</p>
        <img src={logo2} alt="Logo 2" className="middle-logo" />
      </div>
    </div>
  );
};

export default EndScreen;