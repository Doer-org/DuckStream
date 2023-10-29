export default function Home() {
  return (
    <main className='flex min-h-screen flex-col items-center justify-center gap-5 bg-secondary'>
      <h1 className='font-bold text-4xl text-main stroke-black text-stroke'>
        DUCK STREAM
      </h1>
      <a className='text-2xl hover:text-main' href='/draw'>
        play
      </a>
    </main>
  );
}
