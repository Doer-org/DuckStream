import { TActionState, TDrawCondition } from "@/types/app";
import { FC } from "react";

type TProps = { drawConditionState: TActionState<TDrawCondition> };

export const DrawCondition: FC<TProps> = ({ drawConditionState }) => {
  const condition: { [K in TDrawCondition]: { text: string; color: string } } = {
    DRAWING: { text: "描画中", color: "bg-green-400" },
    MOVING: { text: "移動中", color: "bg-yellow-400" },
  };

  return (
    <div className={`p-1 rounded-sm bg-main ms-auto ${condition[drawConditionState.state].color}`}>
      {condition[drawConditionState.state].text}
    </div>
  );
};
