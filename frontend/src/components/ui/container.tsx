
export default function Container({ children }: { children: React.ReactNode }) {
  return (
    <div className="container mx-auto my-6">
      {children}
    </div>
  );
}