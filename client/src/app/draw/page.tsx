"use client";
import { TPoints } from "@/types/app";
import Konva from "konva";
import { useRef, useState } from "react";
import { ZenMaruGothic } from "../../../font/font";
import { DrawEditor } from "./components/DrawEditor";

export default function Draw() {
  const [title, setTitle] = useState("");
  const [points, setPoints] = useState<TPoints>([]);
  const stageRef = useRef<Konva.Stage>(null);

  const onSubmit = async () => {
    await stageRef.current
      ?.toImage()
      .then((data) => {
        const imgElment = data as HTMLImageElement;
        console.log("title", title);
        console.log("imgElement", imgElment.src);
      })
      .then(() => {
        // POST処理した返り値でrouter.pushする
      });
  };

  return (
    <div className={`${ZenMaruGothic.className} flex flex-col gap-1`}>
      <div className="flex gap-1 items-center flex-col">
        <div className="flex justify-between gap-2">
          <input
            id="title"
            placeholder="タイトル"
            type="text"
            className="p-1 rounded-sm"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
          <button onClick={onSubmit} className="border-2 p-1 rounded-sm">
            保存
          </button>
        </div>
      </div>
      <DrawEditor pointsState={{ state: points, setState: setPoints }} stageRef={stageRef} />
      <div>
        <p className="text-sm">キャンバス内のタップで現在位置に点を打てます</p>
      </div>
    </div>
  );
}
