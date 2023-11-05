import Image from "next/image";
export default async function Result({ params }: { params: { id: string } }) {
  const { id } = params;
  const image: { data: { url: string } } = await (await fetch(`${process.env.SERVER_URL}/image/${id}`)).json();

  return (
    <main className="flex min-h-screen flex-col items-center justify-center gap-5 bg-secondary">
      <h1 className="font-bold text-4xl text-main stroke-black text-stroke">DUCK STREAM</h1>
      <div className="p-5">
        <Image src={image.data.url} width={300} height={300} alt="result-image" />
      </div>
      <a className="text-2xl hover:text-main" href="/">
        TOPへ
      </a>
    </main>
  );
}
