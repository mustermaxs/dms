import { useState } from "react";

interface SearchData {
  tags: string[];
  content: string;
}

export const useSearch = () => {
  const [isSearching, setIsSearching] = useState<boolean>(false);
  const [foundDocuments, setFoundDocuments] = useState<Document[]>([]);

  const handleSearch = (searchData: SearchData) => {
    setIsSearching(true);
    setTimeout(() => {
      console.log("searchData");
      console.table(searchData);
      setFoundDocuments([]);
      setIsSearching(false);
    }, 500);
  };

  return { isSearching, handleSearch, setIsSearching, foundDocuments };
};
