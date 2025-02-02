
import { createContext, Dispatch, SetStateAction } from "react";
import { Document, DocumentContentDto, DocumentStatus, UpdateDocumentDto, UploadDocumentDto } from "../../types/Document";
import { Tag } from "../../types/Tag";
import { DocumentStatusEvent, useFileStatus } from "../../hooks/useFileStatus";

interface AppContextProps {
  isLoading: boolean;
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
  addMessage: (message: string) => void;
  messages: { id: number; content: string }[];
  removeMessage: (id: number) => void;
  watchDocumentStatus: (documentId: string, callback: (ev: DocumentStatusEvent) => Promise<void>) => () => void;
  unwatchDocumentStatus: (documentId: string, token: string) => void;
  refetchSelectedDocument: (id: string) => Promise<Document>;
}

const AppContext = createContext<AppContextProps>({
  isLoading: false,
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
  addMessage: () => {},
  messages: [],
  removeMessage: () => {},
  watchDocumentStatus: () => () => {},
  unwatchDocumentStatus: () => {},
  refetchSelectedDocument: () => Promise.resolve({} as Document),
});
export default AppContext;
