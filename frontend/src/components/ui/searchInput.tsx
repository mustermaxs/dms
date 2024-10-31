import React, { useState } from "react";
import useTagInput from '../../hooks/useTagInput';
import { SearchField } from "./SearchField";
import { useSearch } from "../../hooks/useSearch";
import { useContext } from "react";
import AppContext from "../context/AppContext";

const SearchInput = () => {
  // Retrieve the hook methods for tags
  const { tags, handleAddTag, handleRemoveTag } = useTagInput();
  const [content, setContent] = useState<string>("");

  const { isSearching, handleSearch } = useSearch();

  const handleSearchClick = () => {
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
    />
  );
};

export default SearchInput;
