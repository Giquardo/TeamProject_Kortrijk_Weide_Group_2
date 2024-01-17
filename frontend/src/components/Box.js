import React, { useRef } from 'react';
import { useFrame, useLoader } from '@react-three/fiber';
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader';

export default function ModelComponent() {
  const meshRef = useRef();
  const gltf = useLoader(GLTFLoader, process.env.PUBLIC_URL + '/scene.gltf');

  useFrame(() => {
    if (meshRef.current) {
      meshRef.current.rotation.x += 0.01;
      meshRef.current.rotation.y += 0.01;
    }
  });

  return (
    <>
      <ambientLight intensity={0.5} />
      <directionalLight position={[0, 10, 0]} intensity={1.5} />
      <primitive object={gltf.scene} ref={meshRef} />
    </>
  );
}