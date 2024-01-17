import React from 'react';
import { Canvas } from '@react-three/fiber';
import BoxComponent from './components/Box';
import './App.css';

export default function App() {
  return (
    <div className="App">
      <Canvas>
        <BoxComponent />
      </Canvas>
    </div>
  );
}
