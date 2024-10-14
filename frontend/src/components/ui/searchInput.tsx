import { Button, Input } from "rizzui";
import { MagnifyingGlassIcon } from '@heroicons/react/24/solid'


export default function SearchInput({handleSearch}: {handleSearch: () => void}) {
  return (
    <div className="my-6 flex">
      <Input
        placeholder="Search documents"
        className="flex-grow"
      />
      <Button className="ml-2" onClick={handleSearch}>
        <MagnifyingGlassIcon className="w-4 h-4 mr-1" /> 
        Search
      </Button>
    </div>
  );
}