import React from 'react';
import { Box } from '@react-three/drei';
import { useFrame } from '@react-three/fiber';

export default function BoxComponent() {
  const meshRef = React.useRef();

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
      <Box args={[2, 2, 2]} ref={meshRef}>
        <meshLambertMaterial attach="material" color="hotpink" />
      </Box>
    </>
  );
}
