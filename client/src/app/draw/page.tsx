'use client';
import { TDrawCondition, TPicture } from '@/types/app';
import { useEffect, useState } from 'react';
import { ZenMaruGothic } from '../../../font/font';
import { DrawCondition } from './components/DrawCondition';
import { DrawEditor } from './components/DrawEditor';

export default function Draw() {
  const [title, setTitle] = useState('');
  const [picture, setPicture] = useState<TPicture>([]);
  const [drawCondition, setDrawCondition] =
    useState<TDrawCondition>('STOPPING');
  const [draftPosition, setDraftPosition] = useState<{
    x: number;
    y: number;
  }>({ x: 1, y: 1 });

  useEffect(() => {
    navigator.geolocation.watchPosition((position) => {
      console.log(position.coords.latitude, position.coords.longitude);
      setDraftPosition({
        x: position.coords.latitude,
        y: position.coords.longitude,
      });
    });
  }, [draftPosition]);

  const onSubmit = () => {
    // TODO: サーバー側に情報を渡す
  };

  return (
    <div className={`${ZenMaruGothic.className} flex flex-col gap-1`}>
      <div className='flex gap-1 items-center flex-col'>
        <div className='flex justify-between'>
          <input
            id='title'
            placeholder='タイトル'
            type='text'
            className='p-1 rounded-sm'
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
          <button onClick={onSubmit} className='border-2 p-1 rounded-sm'>
            保存
          </button>
        </div>

        <div className='flex gap-1 items-center ms-auto'>
          <DrawCondition
            drawConditionState={{
              state: drawCondition,
              setState: setDrawCondition,
            }}
          />
        </div>
      </div>
      <div>経度{draftPosition.x}</div>
      <div>経度{draftPosition.y}</div>
      <DrawEditor pictureState={{ state: picture, setState: setPicture }} />
      <div>
        <p className='text-sm'>
          キャンバス内のクリックで描画/移動が切り替えれます
        </p>
      </div>
    </div>
  );
}
