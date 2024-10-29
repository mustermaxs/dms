import { useState, useEffect } from "react";
import { Document, UploadDocumentDto } from "../types/Document";
import { ServiceLocator } from "../serviceLocator";
import { MockDocumentService as IDocumentService } from "../services/documentService";

export const useDocuments = () => {
  const [documents, setDocuments] = useState<Document[]>([]);
  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);

  const getDocuments = () => {
    return documents;
  };

  const getDocument = async (id: string) => {
    const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    const document = await documentService.getDocument(id);
    return document;
  };

  const updateDocument = async (document: Document) => {
    const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    const updatedDocument = await documentService.updateDocument(document);

    setDocuments(documents.map(doc => doc.id === document.id ? document : doc));
    setSelectedDocument(document);

    return updatedDocument;
  };

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
      setDocuments(documents);
    };

    fetchDocuments();
  }, []);

  return { documents, getDocuments, setDocuments, getDocument, updateDocument, selectedDocument, setSelectedDocument };
};
