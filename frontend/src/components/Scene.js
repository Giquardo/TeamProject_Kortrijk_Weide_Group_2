import React from 'react';
import { Canvas } from '@react-three/fiber';
import { OrbitControls } from '@react-three/drei';
// import BoxComponent from './Box';
import Model from './Gun';

export default function Scene() {
  return (
    <Canvas>
      <ambientLight intensity={1} />
      <directionalLight position={[0, 10, 5]} intensity={5} castShadow />
      <Model />
      <OrbitControls />
    </Canvas>
  );
}