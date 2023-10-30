import { TActionState, TDrawCondition } from '@/types/app';
import { FC } from 'react';

type TProps = { drawConditionState: TActionState<TDrawCondition> };

export const DrawCondition: FC<TProps> = ({ drawConditionState }) => {
  const condition: { [K in TDrawCondition]: { text: string; color: string } } =
    {
      DRAWING: { text: '描画中', color: 'bg-green-400' },
      MOVING: { text: '移動中', color: 'bg-yellow-400' },
      STOPPING: { text: '停止中', color: 'bg-red-400' },
    };

  return (
    <>
      {drawConditionState.state === 'STOPPING' ? (
        <>
          <div
            className={`p-1 rounded-sm bg-main ms-auto ${
              condition[drawConditionState.state].color
            }`}
          >
            {condition[drawConditionState.state].text}
          </div>
          <button
            className='border-2 p-1 rounded-sm bg-green-400'
            onClick={() => drawConditionState.setState('DRAWING')}
          >
            ▶️
          </button>
        </>
      ) : (
        <>
          <div
            className={`p-1 rounded-sm bg-main ms-auto ${
              condition[drawConditionState.state].color
            }`}
          >
            {condition[drawConditionState.state].text}
          </div>
          <button
            className='border-2 p-1 rounded-sm bg-red-400'
            onClick={() => drawConditionState.setState('STOPPING')}
          >
            ⏹
          </button>
        </>
      )}
    </>
  );
};
