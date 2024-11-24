import { useContext, useState } from "react";
import SearchData from "../types/SearchData";
import AppContext from "../components/context/AppContext";
import { DocumentService } from "../services/documentService";
import { Document } from "../types/Document";

export const useSearch = () => {
  const [isSearching, setIsSearching] = useState<boolean>(false);
  const { setDocuments } = useContext(AppContext);
  const documentService = new DocumentService();

  const handleSearch = async (searchData: SearchData) => {
    const query = [
      ...searchData.tags,
      searchData.content
    ].filter(Boolean).join(' ');

    if (!query.trim()) {
      try {
        const allDocuments = await documentService.getAllDocuments();
        setDocuments(allDocuments);
      } catch (error) {
        console.error('Error fetching all documents:', error);
        setDocuments([]);
      }
      return;
    }

    setIsSearching(true);
    try {
      const response = await documentService.searchDocuments(query);
      console.log('response', response);

      if (!response.success) {
        console.error('Search failed:', response.message);
        setDocuments([]);
        return;
      }

      const documents: Document[] = response.content.map(result => ({
        id: result.id,
        title: result.title,
        content: '',
        tags: result.tags,
        modificationDateTime: result.modificationDateTime,
        uploadDateTime: result.uploadDateTime,
        status: result.status
      }));
      setDocuments(documents);
      
    } catch (error) {
      console.error('Search error:', error);
      setDocuments([]);
    } finally {
      setIsSearching(false);
    }
  };

  return { isSearching, handleSearch };
};
