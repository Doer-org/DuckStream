import { Dispatch, SetStateAction } from "react";

export type TPoint = [x: number, y: number];
export type TPoints = TPoint[];
export type TPicture = TPoints[];
export type TActionState<T> = {
  state: T;
  setState: Dispatch<SetStateAction<T>>;
};
export type TDrawCondition = "DRAWING" | "MOVING";
