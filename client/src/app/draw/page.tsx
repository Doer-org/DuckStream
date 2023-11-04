"use client";
import { TDrawCondition, TPicture } from "@/types/app";
import { useState } from "react";
import { ZenMaruGothic } from "../../../font/font";
import { DrawCondition } from "./components/DrawCondition";
import { DrawEditor } from "./components/DrawEditor";

export default function Draw() {
  const [title, setTitle] = useState("");
  const [picture, setPicture] = useState<TPicture>([]);
  const [drawCondition, setDrawCondition] = useState<TDrawCondition>("DRAWING");

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

        <div className="flex gap-1 items-center ms-auto">
          <DrawCondition
            drawConditionState={{
              state: drawCondition,
              setState: setDrawCondition,
            }}
          />
        </div>
      </div>
      <DrawEditor
        pictureState={{ state: picture, setState: setPicture }}
        drawConditionState={{
          state: drawCondition,
          setState: setDrawCondition,
        }}
      />
      <div>
        <p className="text-sm">キャンバス内のクリックで描画/移動が切り替えれます</p>
      </div>
    </div>
  );
}
