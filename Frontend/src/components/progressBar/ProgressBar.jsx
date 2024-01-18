import React from 'react';
import './ProgressBar.css';

function ProgressBar({ progress }) {
  return (
    <div className="progress-bar-container">
      <div className="progress-bar" style={{ height: `${progress}%` }} />
    </div>
  );
}

export default ProgressBar;