import { TActionState, TLine, TPicture } from '@/types/app';
import { FC, useState } from 'react';

type TPictureEditorProps = {
  pictureState: TActionState<TPicture>;
};

export const DrawEditor: FC<TPictureEditorProps> = ({ pictureState }) => {
  // TODO: Editor内のタップとかで切り替えられるようにしたい
  // TODO: 初期値は選択できるようにしておきたい
  const [line, setLine] = useState<TLine>(['0', '0']);

  // TODO:　pixelsに線の描画が終わったらpushしていく
  return (
    <div className='bg-white w-[95vw] h-[70vh] flex items-center justify-center rounded-sm'>
      <div className='w-2 h-2 rounded-full bg-secondary' />
    </div>
  );
};
