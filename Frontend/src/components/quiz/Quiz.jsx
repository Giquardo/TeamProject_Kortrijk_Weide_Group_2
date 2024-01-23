import React, { useState, useEffect, useRef } from "react";
import "./Quiz.css";
import vragen from "../../data/vragen.js";
import Lottie from "lottie-react";
import timer from "../../animations/timer.json";
import timerTeken from "../../animations/timerTeken.json";

const Quiz = () => {
  const ref = useRef(null);
  const [randomQuestion, setRandomQuestion] = useState(null);
  const [showCorrectAnswer, setShowCorrectAnswer] = useState(false);
  const [timerFinished, setTimerFinished] = useState(false);

  useEffect(() => {
    setTimeout(() => {
      ref?.current?.play();
    }, 500);

    // Select a random question
    const randomIndex = Math.floor(Math.random() * vragen.length);
    setRandomQuestion(vragen[randomIndex]);

    // After 10 seconds, show the correct answer
    const timeoutId = setTimeout(() => {
      setShowCorrectAnswer(true);
    }, 15000);

    // Clean up the timeout when the component is unmounted or the question changes
    return () => clearTimeout(timeoutId);
  }, []);

  const handleAnimationComplete = () => {
    setTimerFinished(true);
  };

  return (
    <>
      <h1 className="title title_extra">Quiz</h1>
      {randomQuestion && (
        <div className="container">
          <div className="text-content">
            <h1 className="vraag">{randomQuestion.vraag}</h1>
            <div className="antwoord">
              {randomQuestion.antwoorden.map((antwoord) => (
                <div key={antwoord.key}>
                  <p
                    className={
                      showCorrectAnswer &&
                      antwoord.key !== randomQuestion.correctAntwoord
                        ? "wrong-answer"
                        : ""
                    }
                  >
                    {antwoord.key}. {antwoord.text}
                  </p>
                </div>
              ))}
            </div>
            {showCorrectAnswer && (
              <p className="verklaring">
                Verklaring: {randomQuestion.verklaring}
              </p>
            )}
          </div>
          <div className="timer-container">
            {!timerFinished && (
              <div className="timer">
                <Lottie
                  animationData={timer}
                  loop={false}
                  autoplay={true}
                  onComplete={handleAnimationComplete}
                />
              </div>
            )}
            {!timerFinished && (
              <div className="timerTeken">
                <Lottie
                  animationData={timerTeken}
                  loop={true}
                  autoplay={true}
                  onComplete={handleAnimationComplete}
                />
              </div>
            )}
          </div>
        </div>
      )}
    </>
  );
};

export default Quiz;
