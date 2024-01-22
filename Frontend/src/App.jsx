import "./App.css";
import { Routes, Route, useNavigate, useLocation } from "react-router-dom";
import React, { useEffect, useState } from 'react';
import ProgressBar from './components/progressBar/ProgressBar.jsx';
import Background from "./components/background/Background.jsx";
import TotaalOverzicht from "./components/totaalOverzicht/TotaalOverzicht.jsx";
import EnergieVerbruikTotaalOverzicht from "./components/energieVerbruikTotaalOverzicht/EnergieVerbruikTotaalOverzicht.jsx";
import UitlegEnergieVermogen from "./components/uitlegEnergieVermogen/UitlegEnergieVermogen.jsx";
import EnergieStroomGebouw from "./components/energieStroomGebouw/EnergieStroomGebouw.jsx";
import energieStroomGebouwInfo from "./data/energieStroomGebouwInfo.js";
import Quiz from "./components/quiz/Quiz.jsx";
import HernieuwbareEnergieLayout from "./components/hernieuwbareEnergie/HernieuwbareEnergieLayout.jsx";
import hernieuwbareEnergieInfo from "./data/hernieuwbareEnergieInfo.js";
import EndScreen from "./components/endScreen/EndScreen.jsx";

import BottomBar from './components/bottomBar/BottomBar.jsx';


const routes = [
  { path: "/totaaloverzicht", element: <TotaalOverzicht /> },
  { path: "/energieverbruiktotaaloverzicht", element: <EnergieVerbruikTotaalOverzicht /> },
  { path: "/uitlegenergievermogen", element: <UitlegEnergieVermogen /> },
  ...energieStroomGebouwInfo.map((info,index) => ({ path: `/gebouw/${index}`, element: <EnergieStroomGebouw info={info}  /> })),
  { path: "/quiz", element: <Quiz /> },
  ...hernieuwbareEnergieInfo.map((info, index) => ({ path: `/hernieuwbareenergie/${index}`, element: <HernieuwbareEnergieLayout info={info} /> })),
  { path: "/endscreen", element: <EndScreen /> }
];

function App() {
  const navigate = useNavigate();
  const location = useLocation();
  const [visitedPages, setVisitedPages] = useState(new Set());
  const [isPlaying, setIsPlaying] = useState(false);
  const totalNumberOfPages = routes.length;

  useEffect(() => {
    if (location.pathname === routes[0].path) {
      setVisitedPages(new Set([location.pathname]));
    } else {
      setVisitedPages(prev => new Set([...prev, location.pathname]));
    }
  }, [location]);

  useEffect(() => {
    let interval;
    if (isPlaying) { 
      interval = setInterval(() => {
        const currentIndex = routes.findIndex(route => route.path === location.pathname);
        const nextIndex = (currentIndex + 1) % totalNumberOfPages;
        navigate(routes[nextIndex].path);
      }, location.pathname === "/quiz" ? 20000 : 10000);
    }
    return () => clearInterval(interval);
  }, [location, navigate, isPlaying]);

  const onPlay = () => {
    setIsPlaying(!isPlaying);
  };

  const progress = (visitedPages.size / totalNumberOfPages) * 100;

  return (
    <div className="App">
      <header className="App-header">
        <Background />
        <ProgressBar progress={progress} />
        <Routes>
          {routes.map((route, index) => (
            <Route key={index} path={route.path} element={route.element} />
          ))}
        </Routes>
      </header>
      <BottomBar onPlay={onPlay} />
    </div>
  );
}

export default App;