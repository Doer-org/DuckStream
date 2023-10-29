import { Dispatch, SetStateAction } from 'react';

export type TLine = [x: string, y: string];
export type TPicture = TLine[];
export type TActionState<T> = {
  state: T;
  setState: Dispatch<SetStateAction<T>>;
};
export type TDrawCondition = 'DRAWING' | 'MOVING' | 'STOPPING';
