import { useEffect, useState } from "react";
import { IDocumentService } from "./documentService";
import { ServiceLocator } from "../serviceLocator";
import { Document, DocumentStatus } from "../types/Document";
import { useDocuments } from "../hooks/useDocuments";

export const useCheckProgressForDocuments = () => {
    const [documentsToWatch, setDocumentsToWatch] = useState<{ [key: string]: { status: DocumentStatus, callbacks: [ (status: DocumentStatus, id: string) => void] } }>({});
    const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');

    useEffect(() => {
        const fetchProgress = async () => {
            try {
                const updatedDocuments = { ...documentsToWatch };

                for (const documentId in documentsToWatch) {
                    const documentProgress = await documentService.getDocument(documentId);

                    if (!documentProgress) {
                        delete updatedDocuments[documentId];
                    }
                    updatedDocuments[documentId].status = documentProgress.status;
                    updatedDocuments[documentId].callbacks.forEach(callback => callback(documentProgress.status, documentId));
                }

                setDocumentsToWatch(updatedDocuments);
            } catch (error) {
                console.error("Failed to fetch document progress:", error);
            }
        };

        fetchProgress();

        const intervalId = setInterval(fetchProgress, 2000);

        return () => clearInterval(intervalId);
    }, [documentsToWatch]);

    const unwatchDocumentStatus = (documentId: string) => {
        let remainingDocuments = { ...documentsToWatch };
        delete remainingDocuments[documentId];
        setDocumentsToWatch(remainingDocuments);
    };

    const watchDocumentStatus = (documentId: string, callback: (status: DocumentStatus, id: string) => void) => {
        if (documentsToWatch[documentId]) {
            documentsToWatch[documentId].callbacks.push(callback);
            return;
        }

        setDocumentsToWatch(prevState => ({
            ...prevState,
            [documentId]: {
                status: DocumentStatus.Pending,
                callbacks: [callback]
            }
        }));
    };

    return {
        watchDocumentStatus,
        unwatchDocumentStatus
    };
}
