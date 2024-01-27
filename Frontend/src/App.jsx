import "./App.css";
import {
  Routes,
  Route,
  useNavigate,
  useLocation,
  Navigate,
} from "react-router-dom";
import React, { useEffect, useState } from "react";
import ProgressBar from "./components/progressBar/ProgressBar.jsx";
import EnergieVerbruikTotaalOverzicht from "./components/energieVerbruikTotaalOverzicht/EnergieVerbruikTotaalOverzicht.jsx";
import UitlegEnergieVermogen from "./components/uitlegEnergieVermogen/UitlegEnergieVermogen.jsx";
import EnergieStroomGebouw from "./components/energieStroomGebouw/EnergieStroomGebouw.jsx";
import energieStroomGebouwInfo from "./data/energieStroomGebouwInfo.js";
import Quiz from "./components/quiz/Quiz.jsx";
import HernieuwbareEnergie from "./components/hernieuwbareEnergie/HernieuwbareEnergie.jsx";
import hernieuwbareEnergieInfo from "./data/hernieuwbareEnergieInfo.js";
import EndScreen from "./components/endScreen/EndScreen.jsx";
import BottomBar from "./components/bottomBar/BottomBar.jsx";
import { CSSTransition, SwitchTransition } from "react-transition-group";

const routes = [
  { path: "/totaaloverzicht", element: <EnergieVerbruikTotaalOverzicht /> },
  { path: "/uitlegenergievermogen", element: <UitlegEnergieVermogen /> },
  // Assuming energieStroomGebouwInfo is an array of objects
  ...energieStroomGebouwInfo.map((info, index) => ({
    path: `/gebouw/${index}`,
    element: <EnergieStroomGebouw info={info} />,
  })),
  { path: "/quiz", element: <Quiz /> },
  // Assuming hernieuwbareEnergieInfo is an array of objects
  ...hernieuwbareEnergieInfo.map((info, index) => ({
    path: `/hernieuwbareenergie/${index}`,
    element: <HernieuwbareEnergie info={info} />,
  })),
  { path: "/endscreen", element: <EndScreen /> },
];

function App() {
  const navigate = useNavigate();
  const location = useLocation();
  const [visitedPages, setVisitedPages] = useState(new Set());
  const [isPlaying, setIsPlaying] = useState(false);
  const totalNumberOfPages = routes.length;
  const [isHovered, setIsHovered] = useState(false);

  useEffect(() => {
    if (location.pathname === routes[0].path) {
      setVisitedPages(new Set([location.pathname]));
    } else {
      setVisitedPages((prev) => new Set([...prev, location.pathname]));
    }
  }, [location]);

  useEffect(() => {
    let interval = null;
    if (isPlaying) {
      interval = setInterval(
        () => {
          const currentIndex = routes.findIndex(
            (route) => route.path === location.pathname
          );
          const nextIndex = (currentIndex + 1) % totalNumberOfPages;
          navigate(routes[nextIndex].path);
        },
        location.pathname === "/quiz" ? 25000 : 10000
      );
    }
    return () => clearInterval(interval);
  }, [location, navigate, isPlaying, totalNumberOfPages]);

  const onPlay = () => {
    setIsPlaying(!isPlaying);
  };

  const progress = (visitedPages.size / totalNumberOfPages) * 100;
  const currentIndex = routes.findIndex(
    (route) => route.path === location.pathname
  );

  return (
    <div
      className="App"
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      <header className="App-header">
        <ProgressBar progress={progress} />
        <Routes>
          <Route
            path="/"
            element={<Navigate to="/totaaloverzicht" replace />}
          />
          {routes.map((route, index) => (
            <Route
              key={index}
              path={route.path}
              element={
                <SwitchTransition>
                  <CSSTransition
                    key={location.key}
                    classNames="fade"
                    timeout={300}
                  >
                    {route.element}
                  </CSSTransition>
                </SwitchTransition>
              }
            />
          ))}
        </Routes>
      </header>
      <BottomBar
        onPlay={onPlay}
        isHovered={isHovered}
        navigate={navigate}
        currentIndex={currentIndex}
        totalNumberOfPages={totalNumberOfPages}
        routes={routes}
      />
    </div>
  );
}

export default App;
