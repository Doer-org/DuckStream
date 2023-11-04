"use client";
import { TActionState, TPoints } from "@/types/app";
import Konva from "konva";
import { FC, RefObject, useRef } from "react";
import { KonvaNodeEvents, Layer, Line, Path, Stage } from "react-konva";

type TPointer = { x: number; y: number };
type TKonvasEditorProps = {
  pointsState: TActionState<TPoints>;
  stageRef: RefObject<Konva.Stage>;
};

const KonvasEditor: FC<TKonvasEditorProps> = ({ pointsState, stageRef }) => {
  Konva.hitOnDragEnabled = true;
  const layerRef = useRef<Konva.Layer>(null);
  const lineRef = useRef<Konva.Line>(null);

  const getCenter = (p1: TPointer, p2: TPointer): TPointer => ({ x: (p1.x + p2.x) / 2, y: (p1.y + p2.y) / 2 });
  const getDistance = (p1: TPointer, p2: TPointer) => Math.sqrt(Math.pow(p2.x - p1.x, 2) + Math.pow(p2.y - p1.y, 2));

  let lastCenter: TPointer | null = null;
  let lastDist = 0;

  const onTouchMove: KonvaNodeEvents["onTouchMove"] = (e) => {
    e.evt.preventDefault();
    let touch1 = e.evt.touches[0];
    let touch2 = e.evt.touches[1];

    const stage = stageRef.current;

    if (stage !== null) {
      if (touch1 && touch2) {
        if (stage.isDragging()) stage.stopDrag();

        let p1 = { x: touch1.clientX, y: touch1.clientY };
        let p2 = { x: touch2.clientX, y: touch2.clientY };

        if (!lastCenter) {
          lastCenter = getCenter(p1, p2);
          return;
        }

        let newCenter = getCenter(p1, p2);
        let dist = getDistance(p1, p2);

        if (!lastDist) lastDist = dist;

        let pointTo = { x: (newCenter.x - stage.x()) / stage.scaleX(), y: (newCenter.y - stage.y()) / stage.scaleX() };

        let scale = stage.scaleX() * (dist / lastDist);

        stage.scaleX(scale);
        stage.scaleY(scale);

        let dx = newCenter.x - lastCenter.x;
        let dy = newCenter.y - lastCenter.y;

        let newPos = { x: newCenter.x - pointTo.x * scale + dx, y: newCenter.y - pointTo.y * scale + dy };

        stage.position(newPos);
        stage.batchDraw();

        lastDist = dist;
        lastCenter = newCenter;
      }
    }
  };

  const onTouchEnd = () => {
    lastCenter = null;
    lastDist = 0;
  };

  return (
    <Stage height={500} width={500} draggable onTouchMove={onTouchMove} onTouchEnd={onTouchEnd} ref={stageRef}>
      <Layer ref={layerRef}>
        {/* 今書いている線を描画する */}
        <Line
          strokeWidth={4}
          lineCap="round"
          lineJoin="round"
          stroke="black"
          points={[...(pointsState.state.flat(Infinity) as number[])]}
          ref={lineRef}
        />
        <Path />
      </Layer>
    </Stage>
  );
};

export default KonvasEditor;
