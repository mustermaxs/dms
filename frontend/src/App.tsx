import { useState } from 'react';
import Header from "./components/ui/header";
import BasicTable from "./components/ui/table";
import SearchInput from "./components/ui/searchInput";
import Container from "./components/ui/container";
import DocumentModal from "./components/ui/DocumentModal";
import { useModal } from "./hooks/useModal";

import Document from './types/Document';


function App() {
  const { isOpen, openModal, closeModal } = useModal();
  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);

  const handleOpenModal = (document: Document) => {
    console.log(document);
    setSelectedDocument(document);
    openModal();
  };

  return (
    <Container>
      {selectedDocument && (
        <DocumentModal 
          isOpen={isOpen} 
          onClose={closeModal}
          title={selectedDocument.title}
          content={selectedDocument.content}
        />
      )}
      <Header />
      <SearchInput handleSearch={() => {}} />
      <BasicTable openDocumentModal={handleOpenModal} />
    </Container>
  );
}

export default App;
