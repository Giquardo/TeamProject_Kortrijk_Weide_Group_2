import "./App.css";
import TotaalOverzicht from "./components/totaalOverzicht/TotaalOverzicht.jsx";
import Quiz from "./components/quiz/Quiz.jsx";
import UitlegEnergieVermogen from "./components/uitlegEnergieVermogen/UitlegEnergieVermogen.jsx";
import HernieuwbareEnergieLayout from "./components/hernieuwbareEnergie/hernieuwbareEnergieLayout.jsx";
import Navbar from "./components/navigator/Navbar.jsx";
import { Routes, Route, useNavigate, useLocation } from "react-router-dom";
import ProgressBar from './components/progressBar/ProgressBar.jsx';
import React, { useEffect, useState } from 'react';

const routes = [
  { path: "/totaaloverzicht", element: <TotaalOverzicht /> },
  { path: "/uitlegenergievermogen", element: <UitlegEnergieVermogen /> },
  { path: "/quiz", element: <Quiz /> },
  { path: "/hernieuwbareenergielayout", element: <HernieuwbareEnergieLayout /> },
];

function App() {
  const navigate = useNavigate();
  const location = useLocation();
  const [visitedPages, setVisitedPages] = useState(new Set());
  const totalNumberOfPages = routes.length;

  useEffect(() => {
    if (location.pathname === routes[0].path) {
      setVisitedPages(new Set([location.pathname]));
    } else {
      setVisitedPages(prev => new Set([...prev, location.pathname]));
    }
  }, [location]);

  useEffect(() => {
    const interval = setInterval(() => {
      const currentIndex = routes.findIndex(route => route.path === location.pathname);
      const nextIndex = (currentIndex + 1) % totalNumberOfPages;
      navigate(routes[nextIndex].path);
    }, 10000);
    return () => clearInterval(interval);
  }, [location, navigate]);

  const progress = (visitedPages.size / totalNumberOfPages) * 100;

  return (
    <div className="App">
      <header className="App-header">
        <ProgressBar progress={progress} />
        <Navbar />
        <Routes>
          {routes.map((route, index) => (
            <Route key={index} path={route.path} element={route.element} />
          ))}
        </Routes>
      </header>
    </div>
  );
}

export default App;