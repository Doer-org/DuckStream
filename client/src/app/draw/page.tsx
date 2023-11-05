"use client";
import { TPoints } from "@/types/app";
import Konva from "konva";
import { useRouter } from "next/navigation";
import { useRef, useState } from "react";
import { ZenMaruGothic } from "../../../font/font";
import { DrawEditor } from "./components/DrawEditor";

export default function Draw() {
  const [title, setTitle] = useState("");
  const [points, setPoints] = useState<TPoints>([]);
  const [isPending, setIsPending] = useState(false);
  const stageRef = useRef<Konva.Stage>(null);
  const router = useRouter();

  const onSubmit = async () => {
    setIsPending(true);
    await stageRef.current?.toImage().then(async (data) => {
      const imgElment = data as HTMLImageElement;
      await fetch(`${process.env.NEXT_PUBLIC_SERVER_URL}/image`, {
        method: "POST",
        body: JSON.stringify({ base64: imgElment.src }),
      })
        .then(async (data) => {
          const imageResultData: { data: { id: string } } = await data.json();
          await fetch(`${process.env.NEXT_PUBLIC_SERVER_URL}/inference`, {
            method: "POST",
            body: JSON.stringify({ prompt: title, id: imageResultData.data.id }),
          }).then(async (data) => {
            const inferenceResult: { data: { id: string } } = await data.json();
            router.push(`${process.env.NEXT_PUBLIC_CLIENT_URL}/result/${inferenceResult.data.id}`);
          });
        })
        .catch(() => {
          setIsPending(false);
        });
    });
  };

  return (
    <>
      {isPending && (
        <div className="w-screen h-screen absolute top-0 left-0 flex items-center justify-center bg-white">
          結果を推論中です...
        </div>
      )}

      <div className={`${ZenMaruGothic.className} flex flex-col gap-1`}>
        <div className="flex gap-1 max-w-[95vw] m-auto">
          <input
            id="title"
            placeholder="タイトル"
            type="text"
            className="p-1 rounded-sm max-w-[70%]"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
          <button onClick={onSubmit} className="border-2 p-1 rounded-sm">
            保存
          </button>
        </div>
        <div className={`${isPending && "hidden"}`}>
          <DrawEditor pointsState={{ state: points, setState: setPoints }} stageRef={stageRef} />
        </div>

        <ul className="max-w-[95vw] m-auto">
          <li className="text-sm">キャンバス内のタップで現在位置に点を打てます</li>
          <li className="text-sm">周りに気をつけて広い場所で遊んでください</li>
        </ul>
      </div>
    </>
  );
}
