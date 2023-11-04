import { TPoint } from "@/types/app";

export const toXY = (position: GeolocationPosition): TPoint => {
  return [position.coords.longitude, position.coords.latitude];
};

// canvas上で正確に図を書くための緯度と経度の修正
type TToAppropriatePositionProps = { start: TPoint; current: GeolocationPosition };
export const toAppropriatePosition = ({ start, current }: TToAppropriatePositionProps): TPoint => {
  const startLongtitude = start[0];
  const startLatitude = start[1];

  const currentLongtitude = current.coords.longitude;
  const currentLatitude = current.coords.latitude;

  const longtitudeDiff = startLongtitude - currentLongtitude;
  const latitudeDiff = startLatitude - currentLatitude;

  // 1mあたりの緯度経度はだいたい0.00001なので1mを一の位のになるようにする。
  const appropriateLongtitude = longtitudeDiff / 0.00001;
  const appropriateLatitude = latitudeDiff / 0.00001;

  //
  console.log("toAppropriatePosition", [appropriateLongtitude, appropriateLatitude].toString());
  return [appropriateLongtitude, appropriateLatitude];
};

export const toInitialXY = (position: GeolocationPosition): TPoint => {
  const longtitude = position.coords.longitude;
  const latitude = position.coords.latitude;
  return [longtitude - Math.floor(longtitude), latitude - Math.floor(position.coords.latitude)];
};
