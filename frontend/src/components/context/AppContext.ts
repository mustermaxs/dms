import { createContext } from 'react';
import { Tag } from '../../types/Tag';
import { Document } from '../../types/Document';


interface AppContextType {
  documents: Document[];
  availableTags: Tag[];
  setDocuments: (documents: Document[]) => void;
  getDocument: (id: string) => Promise<Document>;
  getDocuments: () => Document[];
} 

const AppContext = createContext<AppContextType>({
  documents: [],
  getDocuments: () => [],
  availableTags: [],
  setDocuments: () => {},
  getDocument: () => Promise.resolve({} as Document),
});

export default AppContext;
