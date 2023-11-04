"use clients";
import { TActionState, TPoint, TPoints } from "@/types/app";
import { toAppropriatePosition, toXY } from "@/utils/position";
import { Stage } from "konva/lib/Stage";
import dynamic from "next/dynamic";
import { FC, RefObject, useEffect, useState } from "react";

type TDrawEditorProps = { pointsState: TActionState<TPoints>; stageRef: RefObject<Stage> };

// https://github.com/konvajs/react-konva#usage-with-nextjs の例通りでもできるが、フォントが合わなくなるので普通にimportしてる
const KonvasEditor = dynamic(() => import("./KonvasEditor"), { ssr: false });

export const DrawEditor: FC<TDrawEditorProps> = ({ pointsState, stageRef }) => {
  const [start, setStart] = useState<TPoint>([0, 0]);

  // 初期値を入れる
  useEffect(() => {
    navigator.geolocation.getCurrentPosition((position) => {
      setStart(toXY(position));
    });
  }, []);

  // クリックしたときに現在位置を入れている
  const pushPoint = async () => {
    console.log("onPushPoint");
    await new Promise((resolve, reject) => {
      navigator.geolocation.getCurrentPosition(resolve, reject);
    }).then((data) => {
      pointsState.setState([
        ...pointsState.state,
        toAppropriatePosition({ start, current: data as GeolocationPosition }),
      ]);
    });
  };

  return (
    <>
      {/* 初期値が入るまではviewを表示しない。TODO: 別コンポーネントに分けたい */}
      {!start[0] && (
        <div className="w-screen h-screen bg-white absolute top-0 flex items-center justify-center">
          現在位置の取得中...
        </div>
      )}
      <div className="bg-white max-w-[95vw] mx-auto rounded-md overflow-hidden" onClick={pushPoint}>
        <KonvasEditor pointsState={{ state: pointsState.state, setState: pointsState.setState }} stageRef={stageRef} />
      </div>
    </>
  );
};
