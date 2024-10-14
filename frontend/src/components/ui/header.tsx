import { Button } from "rizzui";
import { ArrowUpTrayIcon } from '@heroicons/react/24/solid'

export default function Header() {
  return (
    <div className="my-6 flex items-center justify-between">
      <h1 className="text-3xl font-bold">Document Management System</h1>
      <Button variant="outline" className="ml-2">
        <ArrowUpTrayIcon className="w-4 h-4 mr-1" /> 
        Upload
      </Button>
    </div>
  );
}