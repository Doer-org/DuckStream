"use client";
import { TActionState, TPoints } from "@/types/app";
import Konva from "konva";
import { FC, useRef, useState } from "react";
import { Layer, Line, Path, Stage, type KonvaNodeEvents } from "react-konva";

type TPointer = { x: number; y: number };
type TKonvasEditorProps = {
  pointsState: TActionState<TPoints>;
};

const KonvasEditor: FC<TKonvasEditorProps> = ({ pointsState }) => {
  Konva.hitOnDragEnabled = true;
  const stageRef = useRef<Konva.Stage>(null);
  const layerRef = useRef<Konva.Layer>(null);
  const lineRef = useRef<Konva.Line>(null);
  const [lastDist, setLastDist] = useState(0);
  const [dist, setDist] = useState(0);
  const [lastCenter, setLastCenter] = useState<null | TPointer>(null);
  const [newCenter, setNewCenter] = useState<null | TPointer>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [scale, setScale] = useState(1);

  const getCenter = (p1: TPointer, p2: TPointer): TPointer => ({ x: (p1.x + p2.x) / 2, y: (p1.y + p2.y) / 2 });
  const getDistance = (p1: TPointer, p2: TPointer) => Math.sqrt(Math.pow(p2.x - p1.x, 2) + Math.pow(p2.y - p1.y, 2));

  // 現在はうまく動いていないが、将来的にピンチで拡大縮小ができるようになりたい
  // https://konvajs.org/docs/sandbox/Multi-touch_Scale_Stage.html　を参考にreactでできるように置き換えたい
  const onTouchMove: KonvaNodeEvents["onTouchMove"] = (event) => {
    event.evt.preventDefault();

    const t1 = event.evt.touches[0];
    const t2 = event.evt.touches[1];

    if (!stageRef.current) return;

    if (t1 && !t2 && !stageRef.current.isDragging() && isDragging) {
      stageRef.current.startDrag();
      setIsDragging(false);
    }

    if (t1 && t2) {
      if (stageRef.current.isDragging()) {
        setIsDragging(true);
        stageRef.current.stopDrag();
      }

      const p1 = { x: t1.clientX, y: t2.clientY };
      const p2 = { x: t1.clientX, y: t2.clientY };

      if (!lastCenter) {
        setLastCenter(getCenter(p1, p2));
        return;
      }
      setNewCenter(getCenter(p1, p2));

      setDist(getDistance(p1, p2));

      if (!lastDist) setLastDist(dist);
      if (!newCenter) return;

      const pointTo = {
        x: (newCenter.x - stageRef.current.x()) / stageRef.current.scaleX(),
        y: (newCenter.y - stageRef.current.y()) / stageRef.current.scaleX(),
      };

      // stageRef.current.scaleX()とかがちゃんと取れていない雰囲気がある
      setScale(stageRef.current.scaleX() * (dist / lastDist));

      stageRef.current.scaleX(scale);
      stageRef.current.scaleY(scale);

      setLastCenter(getCenter(p1, p2));
      const dx = newCenter.x - lastCenter.x;
      const dy = newCenter.y - lastCenter.y;

      const newPos = {
        x: newCenter.x - pointTo.x * scale + dx,
        y: newCenter.y - pointTo.y * scale + dy,
      };

      stageRef.current.position(newPos);

      setLastDist(dist);
      setLastCenter(newCenter);
    }

    if (!layerRef.current || !lineRef.current) {
      return;
    }

    layerRef.current.add(lineRef.current);
    stageRef.current.add(layerRef.current);
  };

  const onTouchEnd: KonvaNodeEvents["onTouchEnd"] = (event) => {
    const t1 = event.evt.touches[0];
    const t2 = event.evt.touches[1];
    if (t1 && t2) {
      setLastDist(0);
      setLastCenter(null);
    }
  };

  return (
    <Stage height={320} width={320} draggable onTouchMove={onTouchMove} onTouchEnd={onTouchEnd} ref={stageRef}>
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
