import { TPoint } from "@/types/app";

export const toXY = (position: GeolocationPosition): TPoint => {
  return [position.coords.longitude, position.coords.latitude];
};

// 座標で初期値はどんな数になるかわからないので一旦小数点以下で管理する
// 1mあたり緯度 : 0.000008983148616
// 1mあたり経度 : 0.000010966382364
// なのでこの数で割って四捨五入した数をクライアント側では描画する？
export const toInitialXY = (position: GeolocationPosition): TPoint => {
  const longtitude = position.coords.longitude;
  const latitude = position.coords.latitude;
  return [longtitude - Math.floor(longtitude), latitude - Math.floor(position.coords.latitude)];
};
