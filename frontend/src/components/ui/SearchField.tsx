import { MagnifyingGlassIcon } from "@heroicons/react/24/solid";
import { useState, ChangeEvent } from "react";
import { Button } from "rizzui";

interface TagAndContentInputProps {
  content: string;
  setContent: (content: string) => void;
  isSearching: boolean;
  className?: string;
  handleSearch: () => void;
}

export const SearchField = ({
  isSearching,
  setContent,
  className,
  handleSearch
}: TagAndContentInputProps) => {
  const [userInput, setUserInput] = useState<string>("");

  // Handle input change
  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    setUserInput(newValue);
    setContent(newValue);
 
  };

  // Handle tag creation on space after "#tag"
  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {


    if (e.key === "Enter") {
      handleSearch();
    }
  };

  return (
    <div className={`flex flex-col space-y-2 ${className}`}>
      {/* Input field and search button on the same line */}
      <div className="flex items-center space-x-2">
        <input
          type="text"
          placeholder="Search by title, tags or content..."
          className="flex-grow border border-gray-300 rounded-md px-4 py-2 focus:outline-none focus:ring-1 focus:ring-black focus:border-black"
          onKeyDown={handleKeyPress}
          onChange={handleInputChange}
          value={userInput}
        />
        <Button variant={isSearching ? "flat" : "outline"} className="flex items-center px-4 py-2 border-2 border-gray-200" onClick={handleSearch}>
          <MagnifyingGlassIcon className="w-4 h-4 mr-1" />
          Search
        </Button>
      </div>

      {/* Tag Display */}
      {/* <div className="flex flex-row flex-wrap gap-3 mt-2 h-7">
        <span className="  text-gray-700  text-sm font-semibold py-0.7 rounded-sm h-5">Tags: </span>
        {tags.length ? tags.map((tag, index) => (
          <span
            key={index}
            className="inline-block bg-blue-100 text-blue-800 text-xs font-semibold px-2.5 py-0.7 rounded h-5"
          >
            {tag}
            <button
              className="ml-2 hover:text-blue-500"
              onClick={() => removeTag(tag)}
            >
              &times;
            </button>
          </span>
        )) : <span className="text-gray-300 text-sm font-normal py-0.7 rounded-sm h-5">No tags added</span>}
      </div> */}
    </div>
  );
};
