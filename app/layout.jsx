export const metadata = {
  title: "Nhà Giả Kim",
  description: "Vercel reverse proxy for the Nhà Giả Kim ASP.NET application.",
};

export default function RootLayout({ children }) {
  return (
    <html lang="vi">
      <body>{children}</body>
    </html>
  );
}
