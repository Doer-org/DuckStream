import type { Metadata } from "next";

export const metadata: Metadata = { title: "Duck Stream - draw -" };

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <>
      <header className="h-[8vh] flex items-center bg-secondary">
        <h1 className="text-sm text-main w-[95vw] m-auto">Duck Stream</h1>
      </header>
      <main className="flex min-h-[92vh] flex-col gap-5 bg-secondary">{children}</main>
    </>
  );
}
