import React, { useState, useEffect, useRef } from "react";
import "./Quiz.css";
import HowestCorner from "../howestCorner/HowestCorner.jsx";
import vragen from "../../data/vragen.js";
import Lottie from "lottie-react";
import timer from "../../animations/timer.json";

const Quiz = () => {
  const ref = useRef(null);

  useEffect(() => {
    setTimeout(() => {
      ref?.current?.play();
    }, 500);
  }, []);

  return (
    <>
      <div className="container">
        <h1 className="vraag">{vragen[0].vraag}</h1>
        <p className="antwoord">
          {vragen[0].antwoorden.map((key, antwoord) => (
            <button key={key}>{antwoord}</button>
          ))}
        </p>
        <div className="timer">
          <Lottie animationData={timer} loop={false} autoplay={true} />
        </div>
      </div>
      <HowestCorner />
    </>
  );
};

export default Quiz;
