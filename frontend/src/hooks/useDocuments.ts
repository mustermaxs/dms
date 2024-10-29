import { useState, useEffect, useContext } from "react";
import { Document, UpdateDocumentDto, UploadDocumentDto } from "../types/Document";
import { ServiceLocator } from "../serviceLocator";
import { IDocumentService } from "../services/documentService";

export const useDocuments = () => {

  const [ documents, setDocuments ] = useState<Document[]>([]);

  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);

  const getDocuments = () => {
    return documents;
  };

  const getDocument = async (id: string) => {
    const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    const document = await documentService.getDocument(id);
    return document;
  };

  const updateDocument = async (document: UpdateDocumentDto): Promise<Document> => {
    const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    const updatedDocument = await documentService.updateDocument(document);
  
    setDocuments((prevDocuments) =>
      prevDocuments.map((doc) => 
        doc.id === updatedDocument.id ? { ...doc, ...updatedDocument } : doc
      )
    );


    return updatedDocument as Document;
  };

    // setDocuments((prevDocuments) => {

    //   prevDocuments.map((doc) => {
    //     if (doc.id === updatedDocument.id) {
    //       return updatedDocument;
    //     }
    //     return doc;
    //   });
    // });


  const uploadDocument = async (document: UploadDocumentDto) => {
    //todo
  };

  const deleteDocument = async (id: string) => {
    //todo
  };

  useEffect(() => {
    const fetchDocuments = async () => {
      const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
      const documents = await documentService.getAllDocuments();

      console.log("documents", documents);

      setDocuments(documents);
    };

    fetchDocuments();
  }, []);

  return { documents, getDocuments, setDocuments, getDocument, updateDocument, selectedDocument, setSelectedDocument };
};
