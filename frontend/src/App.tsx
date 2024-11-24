import Header from "./components/shared/header";
import SearchInput from "./components/ui/searchInput";
import Container from "./components/shared/container";
import DocumentTable from './components/ui/DocumentTable';
import AppContext from "./components/context/AppContext";
import { useTags } from "./hooks/useTags";
import { useDocuments } from "./hooks/useDocuments";
import { useMsgModal } from "./hooks/useMsgModal";
import { useCheckProgressForDocuments } from "./services/uploadProgressInfo";
import { useFileStatus } from "./hooks/useFileStatus";
function App() {

  const { availableTags, setAvailableTags, setIsLoadingTags } = useTags();
  const { documents, setDocuments, getDocuments, updateDocument, uploadDocument, getDocument, selectedDocument, setSelectedDocument } = useDocuments();
  const {addMessage, removeMessage, messages} = useMsgModal();
  const {watchDocumentStatus, unwatchDocumentStatus} = useFileStatus();
  return (
    <>
      <AppContext.Provider value={{ 
          documents,
          getDocuments,
          updateDocument,
          uploadDocument,
          getDocument,
          availableTags,
          setAvailableTags,
          setIsLoadingTags,
          selectedDocument,
          setSelectedDocument,
          setDocuments,
          addMessage,
          messages,
          removeMessage,
          watchDocumentStatus,
          unwatchDocumentStatus
        }}>
        <Container>
          <Header />
          <SearchInput />
          <DocumentTable />
        </Container>
      </AppContext.Provider>
    </>
  );
}

export default App;
