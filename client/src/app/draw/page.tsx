"use client";
import { TPoints } from "@/types/app";
import { useState } from "react";
import { ZenMaruGothic } from "../../../font/font";
import { DrawEditor } from "./components/DrawEditor";

export default function Draw() {
  const [title, setTitle] = useState("");
  const [points, setPoints] = useState<TPoints>([]);

  const onSubmit = () => {
    // TODO: サーバー側に情報を渡す
  };

  return (
    <div className={`${ZenMaruGothic.className} flex flex-col gap-1`}>
      <div className="flex gap-1 items-center flex-col">
        <div className="flex justify-between">
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
      <DrawEditor pointsState={{ state: points, setState: setPoints }} />
      <div>
        <p className="text-sm">キャンバス内のクリックで描画/移動が切り替えれます</p>
      </div>
    </div>
  );
}
