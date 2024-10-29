import Header from "./components/shared/header";
import SearchInput from "./components/ui/searchInput";
import Container from "./components/shared/container";
import DocumentTable from './components/ui/DocumentTable';
import AppContext from "./components/context/AppContext";
import { useTags } from "./hooks/useTags";
import { useDocuments } from "./hooks/useDocuments";

function App() {

  const { availableTags } = useTags();
  const { documents, getDocuments, setDocuments, getDocument, selectedDocument, setSelectedDocument } = useDocuments();

  return (
    <>
      <AppContext.Provider value={{ documents, getDocuments, setDocuments, getDocument, availableTags, selectedDocument, setSelectedDocument }}>
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
