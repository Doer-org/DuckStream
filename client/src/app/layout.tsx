import type { Metadata } from 'next';
import { DelaGothic } from '../../font/font';
import './globals.css';

export const metadata: Metadata = { title: 'Duck Stream' };

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang='en'>
      <body className={DelaGothic.className}>{children}</body>
    </html>
  );
}
