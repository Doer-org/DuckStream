"use clients";
import { TActionState, TDrawCondition, TPicture, TPoints } from "@/types/app";
import { toInitialXY, toXY } from "@/utils/position";
import { FC, useEffect, useState } from "react";
import KonvasEditor from "./KonvasEditor";
type TPictureEditorProps = {
  pictureState: TActionState<TPicture>;
  drawConditionState: TActionState<TDrawCondition>;
};

// https://github.com/konvajs/react-konva#usage-with-nextjs の例通りでもできるが、フォントが合わなくなるので普通にimportしてる
// const KonvasEditor = dynamic(() => import("./KonvasEditor"), { ssr: false });

export const DrawEditor: FC<TPictureEditorProps> = ({ pictureState, drawConditionState }) => {
  const [points, setPoints] = useState<TPoints>([]);
  // 別に取らなくても良さそう。
  const [initialPoints, setInitialPoints] = useState<TPoints>([]);

  const onSwitchCondition = () => {
    if (drawConditionState.state === "DRAWING") {
      drawConditionState.setState("MOVING");
      // MOVINGになる段階で一度線画を保存して、現在の状態を初期化する
      pictureState.setState([...pictureState.state, points]);
      setPoints([]);
    } else {
      drawConditionState.setState("DRAWING");
      // 現在地を初期値にする
      navigator.geolocation.getCurrentPosition((position) => setPoints([toXY(position)]));
    }
  };

  // 初期値を入れる
  useEffect(() => {
    navigator.geolocation.getCurrentPosition((position) => {
      // viewに表す際に何かしらの尺度に置き換えておきたい
      // というのも今いるところだと [135.123456... , 35.123456...] とかで移動しても小数点のところがちょっと動くだけで線画としてはうまく表示されない
      // なので、もうちょっと1~300くらいで移動できるように調整したい
      setPoints([toXY(position)]);
      setInitialPoints([toInitialXY(position)]);
    });
  }, []);

  useEffect(() => {
    // DRAWING以外の状態では線画を作らないようにしている
    if (drawConditionState.state === "DRAWING" && points.length > 1) {
      navigator.geolocation.watchPosition((position) => setPoints([...points, toXY(position)]));
    }
  }, [points, drawConditionState]);

  return (
    <>
      <div>
        <div className="break-all">
          <p>points</p>
          {points.map((point, i) => {
            return <div key={i}>{point.toString()}</div>;
          })}
        </div>
        <div className="break-all">
          <p>pictures</p>
          {pictureState.state.map((picture, i) => {
            return <div key={i}>{picture.toString()}</div>;
          })}
        </div>
      </div>
      <div className="bg-white max-w-[95vw] mx-auto rounded-md overflow-hidden" onClick={onSwitchCondition}>
        {/*  */}
        <KonvasEditor
          pictureState={{ state: pictureState.state, setState: pictureState.setState }}
          pointsState={{ state: points, setState: setPoints }}
        />
      </div>
    </>
  );
};
