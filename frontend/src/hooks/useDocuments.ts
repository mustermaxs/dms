import { useState, useEffect } from "react";
import { Document, DocumentContentDto, UpdateDocumentDto, UploadDocumentDto } from "../types/Document";
import { ServiceLocator } from "../serviceLocator";
import { IDocumentService } from "../services/documentService";
import { assert } from "console";

export const useDocuments = () => {
    const [documents, setDocuments] = useState<Document[]>([]);
    const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const getDocuments = () => documents;

    const updateDocumentInState = (document: Document) => {
        setDocuments((prevDocuments) =>
            prevDocuments.map((doc) => 
                doc.id === document.id ? { ...doc, ...document } : doc
            )
        );
    }

    const refetchSelectedDocument = async (id: string): Promise<Document> => {
        console.log("[useDocuments] Refetching document " + id);
        let refetechedDoc = await getDocument(id);
        let docBeforeUpdate = selectedDocument;
        updateDocumentInState(refetechedDoc);
        setSelectedDocument(refetechedDoc);
        console.assert(docBeforeUpdate !== selectedDocument);
        return selectedDocument;
    }

    const getDocument = async (id: string): Promise<Document | null> => {
        try {
            setError(null);
            const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
            const document = await documentService.getDocument(id);
            // TODO masi
            // if (document != null) {
            //     setDocuments((prevDocuments) => [...prevDocuments, document]);
            // }
            return document;
        } catch (err) {
            setError('Failed to fetch document');
            return null;
        }
    };

    const updateDocument = async (document: UpdateDocumentDto): Promise<Document | null> => {
        try {
            setError(null);
            const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
            const updatedDocument = await documentService.updateDocument(document);

            setDocuments((prevDocuments) =>
                prevDocuments.map((doc) => 
                    doc.id === updatedDocument.id ? { ...doc, ...updatedDocument } : doc
                )
            );

            return updatedDocument;
        } catch (err) {
            setError('Failed to update document');
            return null;
        }
    };

    const uploadDocument = async (document: UploadDocumentDto): Promise<Document | null> => {
        try {
            setError(null);
            const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
            await documentService.uploadDocument(document);
            
            // Refresh the documents list after upload
            const updatedDocs = await documentService.getAllDocuments();
            setDocuments(updatedDocs);
            
            return updatedDocs.find(doc => doc.title === document.title) || null;
        } catch (err) {
            setError('Failed to upload document');
            return null;
        }
    };

    const deleteDocument = async (id: string): Promise<void> => {
        try {
            setError(null);
            const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
            await documentService.deleteDocument(id);
            setDocuments((prevDocuments) => prevDocuments.filter((doc) => doc.id !== id));
        } catch (err) {
            setError('Failed to delete document');
        }
    }


    const getDocumentContent = async (id: string): Promise<DocumentContentDto | null> => {
        try {
            setError(null);
            const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
            const documentContentRes = await documentService.getDocumentContent(id);
            return documentContentRes;
        } catch (err) {
            setError('Failed to fetch document content');
            return null;
        }
    };


    useEffect(() => {
        const fetchDocuments = async () => {
            try {
                setError(null);
                setIsLoading(true);
                const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
                const fetchedDocuments = await documentService.getAllDocuments();
                setDocuments(fetchedDocuments);
            } catch (err) {
                setError('Failed to fetch documents');
                setDocuments([]);
            } finally {
                setIsLoading(false);
            }
        };

        fetchDocuments();
    }, []);

    return { 
        documents, 
        getDocuments, 
        setDocuments, 
        getDocument, 
        updateDocument, 
        selectedDocument, 
        setSelectedDocument, 
        uploadDocument, 
        error,
        deleteDocument,
        getDocumentContent,
        refetchSelectedDocument,
        isLoading
    };
};
