import React, { useState } from "react";
import useTagInput from '../../hooks/useTagInput';
import { SearchField } from "./SearchField";
import { MagnifyingGlassIcon } from "@heroicons/react/24/solid";
import { Button } from "rizzui";

const SearchInput = ({
  handleSearch,
  isSearching,
  setIsSearching
}: {
  handleSearch: (searchData: { tags: string[]; content: string }) => void;
  isSearching: boolean;
  setIsSearching: (isSearching: boolean) => void;
}) => {

  // Retrieve the hook methods for tags
  const { tags, handleAddTag, handleRemoveTag } = useTagInput();
  const [content, setContent] = useState<string>(""); // Store the content separately
  const handleSearchClick = () => {
    setIsSearching(true);
    // Send both the tags and the content as JSON
    handleSearch({
      tags,
      content
    });
  };

  return (
      <SearchField
        tags={tags}
        addTag={handleAddTag}
        removeTag={handleRemoveTag}
        content={content}
        setContent={setContent}
        handleSearch={handleSearchClick}
        isSearching={isSearching}
        setIsSearching={setIsSearching}
      />
  );
};

export default SearchInput;
