import { useEffect, useState } from "react";
import { IDocumentService } from "./documentService";
import { ServiceLocator } from "../serviceLocator";
import { Document, DocumentStatus } from "../types/Document";
import { useDocuments } from "../hooks/useDocuments";

const config = require("../config.json");

function createToken() {
    return Math.random().toString(36).substr(2);
}

export type statusChangeEvent = {
    documentId: string;
    status: DocumentStatus;
    token: string;
};

export type subscriber = {
    token: string;
    handle: (ev: statusChangeEvent, token: string) => void;
};

export type DocumentProgressRegistry = {
    status: DocumentStatus;
    subscribers: subscriber[];
};

export const useCheckProgressForDocuments = () => {
    const [documentsToWatch, setDocumentsToWatch] = useState<{ [documentId: string]: DocumentProgressRegistry }>({});
    const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    const [mockApplied, setMockApplied] = useState<boolean>(false);
    const [watchDocuments, setWatchDocuments] = useState<boolean>(false);

    const isDocumentRegistered = (documentId: string) => {
        console.log("[watchlist] document is registered: ", documentId in documentsToWatch);
        return documentId in documentsToWatch;
    };

    const tryAddDocumentToWatchList = (documentId: string) => {
        if (!isDocumentRegistered(documentId)) {
            console.log("[watchlist] Registering document", documentId);

            setDocumentsToWatch(prevState => {
                const updatedState = { ...prevState };
                updatedState[documentId] = { status: DocumentStatus.NotStarted, subscribers: [] };
                console.log("[watchlist] tryAddDocumentToWatchList state: ", updatedState);
                return updatedState;
            });
            console.log("[watchlist] Added document to watch list", documentId);
        }

        const isDebugMode = config.debug;

        if (isDebugMode === "true") {
            console.log("[watchlist] Is debug mode. Applying mock progress");
            setMockApplied(true);
        }
    };

    const removeDocumentFromWatchList = (id: string) => {
        if (!isDocumentRegistered(id)) {
            console.log(`[watchlist] Document ${id} is not registered!`);
            return;
        }
        setDocumentsToWatch((prevState) => {
            const updatedWatchlist = { ...prevState };
            delete updatedWatchlist[id];
            console.log("[watchlist] Remaining documents: ", updatedWatchlist);
            return updatedWatchlist;
        });
    
        console.log("[watchlist] Removing document from watch list", id);
    };
    

    useEffect(() => {
        console.log("[watchlist] Is Mock Mode: ", mockApplied);
        let t = null;

        if (mockApplied) {
            setTimeout(() => {
                console.log("[watchlist] MOCK APPLIED");
                mockProgressChange()();

            }, 6000);
        }
    }, [mockApplied]);

    const removeSubscriber = (token: string) => {
        setDocumentsToWatch(prevState => {
            const updatedState = { ...prevState };
            for (const documentId in updatedState) {
                updatedState[documentId].subscribers = updatedState[documentId].subscribers.filter(subscriber => subscriber.token !== token);
                console.log("[watchlist] Remove subscriber from document", documentId);
            }
            return updatedState;
        });
    };

    const subscriberExists = (documentId: string, token: string) => {
        return documentsToWatch[documentId].subscribers.some(subscriber => subscriber.token === token);
    };

    const addSubscriber = (documentId: string, subscribersCallback: (ev: statusChangeEvent, token: string) => void) => {
        let subscriberToken = createToken();

        setDocumentsToWatch((prevState) => {
            const updatedState = { ...prevState };

            if (!updatedState[documentId]) {
                console.error(`[watchlist] Document ${documentId} is not registered!`);
                return prevState;
            }

            updatedState[documentId].subscribers.push({ token: subscriberToken, handle: subscribersCallback });
            return updatedState;
        });

        return subscriberToken;
    };

    const mockProgressChange = () => {
        const fetchProgress = async () => {
            const updatedDocuments = { ...documentsToWatch };
    
            for (const documentId in documentsToWatch) {
                const documentProgress = await documentService.getDocument(documentId);
    
                    if (documentProgress.status === documentsToWatch[documentId].status) {
                        continue;
                    }
    
                    if (documentsToWatch[documentId].subscribers.length === 0) {
                        removeDocumentFromWatchList(documentId);
                        continue;
                    }
    
                    documentsToWatch[documentId].subscribers.forEach((subscriber) => {
                        let event: statusChangeEvent = {
                            documentId: documentId,
                            status: documentProgress.status,
                            token: subscriber.token
                        };
                        subscriber.handle(event, subscriber.token);
                    });
                }
            };

        return fetchProgress;
    };

    const updateDocumentStatus = (documentId: string, status: DocumentStatus) => {
        setDocumentsToWatch(prevState => {
            const updatedState = { ...prevState };
            updatedState[documentId].status = status;
            return updatedState;
        });
    };

    const isWatchListEmpty = () => {
        return Object.keys(documentsToWatch).length === 0;
    };

    const hasDocumentSubscribers = (documentId: string) => {
        if (!isDocumentRegistered(documentId))
            return false;
        return documentsToWatch[documentId].subscribers.length > 0;
    }


    useEffect(() => {
        if (config.debug === "true") {
            return;
        }
        const fetchProgress = async () => {
            try {
                const updatedDocuments = { ...documentsToWatch };

                for (const documentId in documentsToWatch) {
                    const documentProgress = await documentService.getDocument(documentId);

                    if (documentProgress === null || documentProgress === undefined) {
                        console.warn(`[watchlist] Document ${documentId} not found on server!`);
                        removeDocumentFromWatchList(documentId);
                        continue;
                    }

                    if (documentsToWatch[documentId].subscribers.length === 0) {
                        removeDocumentFromWatchList(documentId);
                        continue;
                    }

                    if (documentProgress.status === documentsToWatch[documentId].status) {
                        continue;
                    }
                    
                    updateDocumentStatus(documentId, documentProgress.status);

                    documentsToWatch[documentId].subscribers.forEach((subscriber) => {
                        let event: statusChangeEvent = {
                            documentId: documentId,
                            status: documentProgress.status,
                            token: subscriber.token
                        };

                        subscriber.handle(event, subscriber.token);
                    });

                }
            } catch (error) {
                console.error("[watchlist] Failed to fetch document progress:", error);
            }
        };

        if (!isWatchListEmpty())
        {
            fetchProgress();
            const intervalId = setInterval(fetchProgress, 2000);
    
            return () => clearInterval(intervalId);
        }

    }, [watchDocuments]);

    const unwatchDocumentStatus = (documentId: string, token: string) => {
        if (!subscriberExists)
            return;

        removeSubscriber(token);

        if (isWatchListEmpty())
            setWatchDocuments(false);
    
        if (!hasDocumentSubscribers(documentId))
            removeDocumentFromWatchList(documentId);
    };

    const watchDocumentStatus = (documentId: string, callback: (ev: statusChangeEvent, token: string) => void) => {
        console.log("[watchlist] WATCHING DOCUMENT " + documentId);
        setWatchDocuments(true);
        tryAddDocumentToWatchList(documentId);
        return addSubscriber(documentId, callback);
    };

    return {
        watchDocumentStatus,
        unwatchDocumentStatus
    };
}
