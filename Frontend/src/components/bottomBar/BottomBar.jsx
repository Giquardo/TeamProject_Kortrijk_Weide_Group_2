import React from 'react';
import { IoExpand, IoPlayCircleOutline } from "react-icons/io5";

import './BottomBar.css';

const BottomBar = ({ onPlay }) => {
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

  return (
    <div className="bottom-bar">
      <IoPlayCircleOutline className="play-icon" onClick={onPlay} />
      <IoExpand className="fullscreen-icon" onClick={goFullScreen} />
    </div>
  );
};

export default BottomBar;