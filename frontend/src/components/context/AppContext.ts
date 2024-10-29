import { createContext } from 'react';
import { Tag } from '../../types/Tag';
import { Document } from '../../types/Document';


interface AppContextType {
  documents: Document[];
  availableTags: Tag[];
  setDocuments: (documents: Document[]) => void;
  getDocument: (id: string) => Promise<Document>;
  getDocuments: () => Document[];
  selectedDocument: Document | null;
  setSelectedDocument: (document: Document) => void;
} 

const AppContext = createContext<AppContextType>({
  documents: [],
  getDocuments: () => [],
  availableTags: [],
  setDocuments: () => {},
  getDocument: () => Promise.resolve({} as Document),
  selectedDocument: null,
  setSelectedDocument: () => {},
});

export default AppContext;
