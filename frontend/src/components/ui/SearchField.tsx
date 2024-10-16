import { MagnifyingGlassIcon } from "@heroicons/react/24/solid";
import { useState, ChangeEvent } from "react";
import { Button } from "rizzui";

interface TagAndContentInputProps {
  tags: string[];
  addTag: (tag: string) => void;
  removeTag: (tag: string) => void;
  content: string;
  setContent: (content: string) => void;
  isSearching: boolean;
  setIsSearching: (isSearching: boolean) => void;
  className?: string;
  handleSearch: () => void;
}

export const SearchField = ({
  tags,
  addTag,
  removeTag,
  isSearching,
  setContent,
  className,
  handleSearch
}: TagAndContentInputProps) => {
  const [userInput, setUserInput] = useState<string>("");

  // Handle input change
  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    setUserInput(e.target.value);
    setContent(e.target.value);
  };

  // Handle tag creation on space after "#tag"
  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === " " && userInput.split(" ")[userInput.split(" ").length - 1].startsWith("#")) {
      e.preventDefault(); // Prevent adding a space to the input
      const tag = userInput.split(" ")[userInput.split(" ").length - 1]; // Extract last word
      if (tag.length > 1) {
        addTag(tag.substring(1)); // Add the tag without the "#"
        setUserInput(userInput.substring(0, userInput.length - tag.length));
        setContent(userInput.substring(0, userInput.length - tag.length));
      }
    }

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
          placeholder="Add a #tag or search term"
          className="flex-grow border border-gray-300 rounded-md px-4 py-2 focus:outline-none focus:ring-1 focus:ring-black focus:border-black"
          onKeyDown={handleKeyPress}
          onChange={handleInputChange}
          value={userInput}
        />
        <Button className="flex items-center px-4 py-2" onClick={handleSearch}>
          <MagnifyingGlassIcon className="w-4 h-4 mr-1" />
          {isSearching ? "Searching..." : "Search"}
        </Button>
      </div>

      {/* Tag Display */}
      <div className="flex flex-row flex-wrap gap-3 mt-2 h-7">
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
      </div>
    </div>
  );
};
