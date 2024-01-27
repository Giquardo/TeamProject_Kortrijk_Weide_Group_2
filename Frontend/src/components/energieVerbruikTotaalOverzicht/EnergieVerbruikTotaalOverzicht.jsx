import React, { useState, useEffect } from "react";
import "./EnergieVerbruikTotaalOverzicht.css";
import overzicht from "../../Images/TotaalOverzicht.png";
import Lottie from "lottie-react";
import powerline from "../../animations/power_line.json";
import "../General.css";
import Background from "../background/Background";

const TotaalOverzicht = () => {
  const [data, setData] = useState({});

  useEffect(() => {
    fetch("http://localhost:5000/api/PeriodData/generaloverview/weekly")
      .then((response) => response.json())
      .then((data) => {
        if (Array.isArray(data.generaloverview) && data.generaloverview.length > 0) {
          setData(data.generaloverview[0]);
        } else {
          console.error('generaloverview is not an array or it\'s empty:', data);
        }
      })
      .catch((error) => console.error("Error:", error));
  }, []);

  return (
    <>
      <Background />
      <h1 className="title title_extra">Kortrijk Weide</h1>
      <div className="overzicht-container">
        <img className="image" src={overzicht} alt="overzicht" />
        <Lottie />
        <div className="textbox textbox1">
          Totaal Verbruik
          <div>
            {data.consumption ? `${data.consumption} kWh` : "Loading..."}
            <Lottie
              animationData={powerline}
              loop={true}
              autoplay={true}
              style={{ transform: "rotate(90deg)", height: "100px" }}
            />
          </div>
        </div>
        <div className="textbox textbox2">
          <Lottie
            animationData={powerline}
            loop={true}
            autoplay={true}
            direction={-1}
            style={{
              transform: "rotate(90deg)",
              height: "100px",
              paddingBottom: "10px",
              paddingTop: "10px",
            }}
          />
          Injectie
          <div>{data.injection ? `${data.injection} kWh` : "Loading..."}</div>
        </div>
        <div className="textbox textbox3">
          <Lottie
            animationData={powerline}
            loop={true}
            autoplay={true}
            direction={-1}
            style={{
              transform: "rotate(90deg)",
              height: "100px",
              paddingBottom: "10px",
              paddingTop: "10px",
            }}
          />
          Totale productie
          <div>{data.production ? `${data.production} kWh` : "Loading..."}</div>
        </div>
      </div>
    </>
  );
};

export default TotaalOverzicht;
