import { Dispatch, SetStateAction } from 'react';

export type TLine = [x: number, y: number];
export type TLines = TLine[];
export type TPicture = TLines[];
export type TActionState<T> = {
  state: T;
  setState: Dispatch<SetStateAction<T>>;
};
export type TDrawCondition = 'DRAWING' | 'MOVING' | 'STOPPING';
