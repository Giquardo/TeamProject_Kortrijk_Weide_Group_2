import React, { useState, useEffect } from 'react';
import { IoExpand, IoPlayCircleOutline, IoPauseCircleOutline, IoArrowBack, IoArrowForward } from "react-icons/io5";
import './BottomBar.css';

const BottomBar = ({  onPlay, navigate, currentIndex, totalNumberOfPages, routes  }) => {
  const [isVisible, setIsVisible] = useState(true);
  const [isPlaying, setIsPlaying] = useState(false); // New state variable

  useEffect(() => {
    const timeout = setTimeout(() => {
      setIsVisible(false);
    }, 5000);
    return () => clearTimeout(timeout);
  }, []);

  const handlePlayPause = () => {
    setIsPlaying(!isPlaying); // Toggle play/pause state
    onPlay(); // Call the onPlay prop function
  };

  const goFullScreen = () => {
    if (document.fullscreenElement) {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      } else if (document.mozCancelFullScreen) { /* Firefox */
        document.mozCancelFullScreen();
      } else if (document.webkitExitFullscreen) { /* Chrome, Safari and Opera */
        document.webkitExitFullscreen();
      } else if (document.msExitFullscreen) { /* IE/Edge */
        document.msExitFullscreen();
      }
    } else {
      if (document.documentElement.requestFullscreen) {
        document.documentElement.requestFullscreen();
      } else if (document.documentElement.mozRequestFullScreen) { /* Firefox */
        document.documentElement.mozRequestFullScreen();
      } else if (document.documentElement.webkitRequestFullscreen) { /* Chrome, Safari and Opera */
        document.documentElement.webkitRequestFullscreen();
      } else if (document.documentElement.msRequestFullscreen) { /* IE/Edge */
        document.documentElement.msRequestFullscreen();
      }
    }
  }

  const goBack = () => {
    const prevIndex = (currentIndex - 1 + totalNumberOfPages) % totalNumberOfPages;
    navigate(routes[prevIndex].path);
  };

  const goForward = () => {
    const nextIndex = (currentIndex + 1) % totalNumberOfPages;
    navigate(routes[nextIndex].path);
  };

  return (
    <div className={`bottom-bar ${isVisible ? 'visible' : ''}`} onMouseEnter={() => setIsVisible(true)} onMouseLeave={() => setIsVisible(false)}>
      {isPlaying ? 
        <IoPauseCircleOutline className="play-icon" onClick={handlePlayPause} /> : 
        <IoPlayCircleOutline className="play-icon" onClick={handlePlayPause} />
      }
      <div>
        <IoArrowBack className="arrow-icon" onClick={goBack} />
        <IoArrowForward className="arrow-icon" onClick={goForward} />
      </div>
      <IoExpand className="fullscreen-icon" onClick={goFullScreen} />
    </div>
  );
};

export default BottomBar;