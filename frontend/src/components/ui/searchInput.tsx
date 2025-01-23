import React, { useState } from "react";
import { SearchField } from "./SearchField";
import { useSearch } from "../../hooks/useSearch";

const SearchInput = () => {
  // Retrieve the hook methods for tags
  const [content, setContent] = useState<string>("");

  const { isSearching, handleSearch } = useSearch();

  const handleSearchClick = () => {
    handleSearch({
      tags: [],
      content
    });
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-4">
      <SearchField
        content={content}
        setContent={setContent}
        handleSearch={handleSearchClick}
        isSearching={isSearching}
      />
    </div>
  );
};

export default SearchInput;
