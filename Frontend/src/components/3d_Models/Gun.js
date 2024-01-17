import { useGLTF } from '@react-three/drei';

export default function Model(props) {
  const { nodes, materials } = useGLTF('/gun.gltf');

  return (
    <group {...props} dispose={null}>
      <group scale={0.005}>
        <mesh
          geometry={nodes['����������_lowpoly_Mat1_0'].geometry}
          material={materials.Mat1}
          position={[0, 132.667, 207.164]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['��������������_lowpoly_Mat1_0'].geometry}
          material={materials.Mat1}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['����������������_lowpoly_Mat1_0'].geometry}
          material={materials.Mat1}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['������������_lowpoly_Mat1_0'].geometry}
          material={materials.Mat1}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['����������_lowpoly_Mat1_0_1'].geometry}
          material={materials.Mat1}
          position={[0, 256.925, 183.079]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['����������_lowpoly_Mat1_0_2'].geometry}
          material={materials.Mat1}
          position={[0, 256.925, -288.25]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['����������_lowpoly_Mat1_0_3'].geometry}
          material={materials.Mat1}
          position={[0, 191.47, 292.238]}
          rotation={[-Math.PI / 2, Math.PI / 2, 0]}
          scale={100}
        />
        <mesh
          geometry={
            nodes['����������������������������_lowpoly_Mat1_0'].geometry
          }
          material={materials.Mat1}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['����������_lowpoly_Mat1_0_4'].geometry}
          material={materials.Mat1}
          position={[87.558, 205.066, 101.961]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['������������_lowpoly_Mat1_0_1'].geometry}
          material={materials.Mat1}
          position={[0, 208.905, -72.105]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['����������_lowpoly_Mat1_0_5'].geometry}
          material={materials.Mat1}
          position={[0, 197.432, -289.469]}
          rotation={[Math.PI, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['������������_lowpoly_Mat1_0_2'].geometry}
          material={materials.Mat1}
          position={[0, 32.221, -224.169]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['������������_lowpoly_Mat1_0_3'].geometry}
          material={materials.Mat1}
          position={[0, 39.745, -22.303]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={
            nodes['������������_����������������_lowpoly_Mat1_0'].geometry
          }
          material={materials.Mat1}
          position={[0, 0.555, 2.22]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={
            nodes['����������_������������������_lowpoly_Mat1_0'].geometry
          }
          material={materials.Mat1}
          position={[-47.944, -36.87, 58.731]}
          rotation={[-Math.PI / 2, -Math.PI / 2, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['����������_lowpoly_Mat1_0_6'].geometry}
          material={materials.Mat1}
          position={[-53.042, -36.87, 58.731]}
          rotation={[-Math.PI / 2, -Math.PI / 2, 0]}
          scale={100}
        />
        <mesh
          geometry={
            nodes['��������������_������������������_lowpoly_Mat1_0'].geometry
          }
          material={materials.Mat1}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
        <mesh
          geometry={nodes['��������������_lowpoly_Mat1_0_1'].geometry}
          material={materials.Mat1}
          position={[0, -116.162, 208.68]}
          rotation={[-Math.PI / 2, 0, 0]}
          scale={100}
        />
      </group>
    </group>
  );
}

useGLTF.preload('/gun.gltf');
