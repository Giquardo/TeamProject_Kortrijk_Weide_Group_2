import React, { useState, useEffect } from "react";
import "./EnergieVerbruikTotaalOverzicht.css";
import overzicht from "../../Images/TotaalOverzicht.png";
import Lottie from "lottie-react";
import powerline from "../../animations/power_line.json";
import "../General.css";
import Background from "../backgroundTotaalOVerzicht/Background";

const TotaalOverzicht = () => {
  const [data, setData] = useState({});

  useEffect(() => {
    // Replace with your HTTP trigger
    fetch("http://localhost:5000/api/weeklydata/generaloverview")
      .then((response) => response.json())
      .then((data) => setData(data.productionoverview[0])); // Access the first object in the array
    console.log(data);
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
