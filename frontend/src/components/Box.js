import React from 'react';
import { Box, useTexture } from '@react-three/drei';
import boxTexture from '../resources/boxTexture.jpg'; // import the texture file

export default function BoxComponent() {
  const meshRef = React.useRef();
  const texture = useTexture(boxTexture); // use the imported texture

  return (
    <>
      <Box args={[2, 2, 2]} ref={meshRef}>
        <meshBasicMaterial attach="material" map={texture} />
      </Box>
    </>
  );
}