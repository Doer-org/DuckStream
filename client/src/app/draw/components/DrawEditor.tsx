import { TActionState, TDrawCondition, TLines, TPicture } from '@/types/app';
import { FC, useEffect, useState } from 'react';

type TPictureEditorProps = {
  pictureState: TActionState<TPicture>;
  drawConditionState: TActionState<TDrawCondition>;
};

export const DrawEditor: FC<TPictureEditorProps> = ({
  pictureState,
  drawConditionState,
}) => {
  const [lines, setLines] = useState<TLines>([[0, 0]]);
  // TODO: Editor内のタップとかで切り替えられるようにしたい
  const onSwitchCondition = () => {
    if (drawConditionState.state === 'DRAWING') {
      drawConditionState.setState('MOVING');
      pictureState.setState([...pictureState.state, lines]);
      setLines([]);
    } else if (drawConditionState.state === 'MOVING') {
      drawConditionState.setState('DRAWING');
    }
  };
  useEffect(() => {
    if (drawConditionState.state === 'DRAWING') {
      navigator.geolocation.watchPosition((position) => {
        setLines([
          ...lines,
          [position.coords.latitude, position.coords.longitude],
        ]);
      });
    }
  }, [lines, drawConditionState.state]);
  console.log(lines);
  console.log(drawConditionState.state);
  // TODO: 初期値は選択できるようにしておきたい

  // TODO:　pixelsに線の描画が終わったらpushしていく
  return (
    <>
      <div>
        <div>lines:{lines.toString()}</div>
        <div>pictures:{pictureState.state.toString()}</div>
      </div>
      <div
        className='bg-white w-[95vw] h-[70vh] flex items-center justify-center rounded-sm'
        onClick={onSwitchCondition}
      >
        <div className='w-2 h-2 rounded-full bg-secondary' />
      </div>
    </>
  );
};
