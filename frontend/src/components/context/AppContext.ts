
import { createContext, Dispatch, SetStateAction } from "react";
import { Document, UpdateDocumentDto, UploadDocumentDto } from "../../types/Document";
import { Tag } from "../../types/Tag";

interface AppContextProps {
  documents: Document[];
  setDocuments: Dispatch<SetStateAction<Document[]>>;
  getDocument: (id: string) => Promise<Document>;
  getDocuments: () => Document[];
  selectedDocument: Document | null;
  availableTags: Tag[];
  setAvailableTags: Dispatch<SetStateAction<Tag[]>>;
  setIsLoadingTags: Dispatch<SetStateAction<boolean>>;
  uploadDocument: (document: UploadDocumentDto) => Promise<Document>;
  updateDocument: (document: UpdateDocumentDto) => Promise<Document>;
  setSelectedDocument: Dispatch<SetStateAction<Document | null>>;
}

const AppContext = createContext<AppContextProps>({
  documents: [],
  setDocuments: () => {},
  getDocument: () => Promise.resolve({} as Document),
  getDocuments: () => [],
  selectedDocument: null,
  availableTags: [],
  setAvailableTags: () => {},
  setIsLoadingTags: () => {},
  uploadDocument: () => Promise.resolve({} as Document),
  updateDocument: () => Promise.resolve({} as Document),
  setSelectedDocument: () => {},
});
export default AppContext;
