export default function Result({ params }: { params: { id: string } }) {
  const { id } = params;
  return (
    <main className='flex min-h-screen flex-col items-center justify-center gap-5 bg-secondary'>
      <h1 className='font-bold text-4xl text-main stroke-black text-stroke'>
        DUCK STREAM
      </h1>
      <div className='w-[95vw] h-[65vh] bg-slate-300 rounded-sm' />
      <a className='text-2xl hover:text-main' href='/'>
        TOPへ
      </a>
    </main>
  );
}
