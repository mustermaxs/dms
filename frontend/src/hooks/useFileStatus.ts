import { useEffect, useState } from "react";
import { PubSub, PubsubEvent, usePubsub } from "./usePubsub";
import { statusChangeEvent } from "../services/uploadProgressInfo";
import { ServiceLocator } from "../serviceLocator";
import { IDocumentService } from "../services/documentService";
import { DocumentStatus } from "../types/Document";

const config = require("../config.json");
const isDebugMode = config.debug;

type documentStatusInfo = {
    documentId: string,
    status: DocumentStatus
};

export class DocumentStatusEvent implements PubsubEvent {
    event: string;
    data: DocumentStatus;
    token: string;
    documentId: string;

    constructor(event: string, data: DocumentStatus, token: string, documentId: string) {
        this.event = event;
        this.data = DocumentStatus.NotStarted;
        this.token = token;
        this.documentId = documentId;
    }
};

export const useFileStatus = () => {
    const pubsub = usePubsub();
    const [watchDocuments, setWatchDocuments] = useState<boolean>(false);
    const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    const [documents, setDocuments] = useState<string[]>([]);
    const [documentStatusInfo, setDocumentStatusInfo] = useState<documentStatusInfo[]>([]);

    const removeStatusInfo = (documentId: string) => {
        let infos = [...documentStatusInfo];
        let infoToRemove = infos.find((info) => info.documentId === documentId);
        if (infoToRemove) {
            infos.splice(infos.indexOf(infoToRemove), 1);
            setDocumentStatusInfo(infos);
        }
    };

    const setStatus = (documentId: string, status: DocumentStatus) => {
        let infos = [...documentStatusInfo];
        let infoToUpdate = infos.find((info) => info.documentId === documentId);
        if (infoToUpdate) {
            removeStatusInfo(documentId);
        }
        setDocumentStatusInfo([...infos, { documentId: documentId, status: status }]);
    };

    const addSubscriber = (documentId: string, callback: (ev: DocumentStatusEvent) => void) => {
        const unsubscribe = pubsub.subscribe(documentId, callback);
        return unsubscribe;
    };

    const unwatchDocumentStatus = (documentId: string, token: string) => {
        console.log("[watchlist] UNWATCHING DOCUMENT " + documentId);
        
        pubsub.unsubscribe(documentId, token);
        if (!pubsub.hasTopicSubscribers(documentId)) {
            pubsub.removeTopic(documentId);
            setWatchDocuments(false);
            setDocuments([]);
            return;
        }
        
        let documentIds = [...documents];
        setDocuments(documentIds.splice(documentIds.indexOf(documentId), 1));
    };

    const watchDocumentStatus = (documentId: string, callback: (ev: DocumentStatusEvent) => void) => {
        console.log("[watchlist] WATCHING DOCUMENT " + documentId);
        setWatchDocuments(true);
        setDocuments([...documents, documentId]);
        setStatus(documentId, DocumentStatus.NotStarted);
        return addSubscriber(documentId, callback);
    };

    const fetchStatusForDocuments = async () => {
        console.log("FETCHING DOCUMENTS", pubsub.events);
        pubsub.events.forEach(async (subscribers, documentId) => {
            console.log("[watchlist] Fetching status for document " + documentId);
            const document = await documentService.getDocument(documentId);

            if (document === null || document === undefined) {
                console.warn(`[watchlist] Document ${documentId} not found on server!`);
                return;
            }
            
            let previousStatus = documentStatusInfo.find((info) => info.documentId === documentId);

            if (previousStatus === undefined) {
                console.error(`[watchlist] Document ${documentId} not found in documentStatusInfo!`);
                return;
            }
            if (document.status === previousStatus.status) {
                return;
            }
            
            setStatus(documentId, document.status);
            console.log(subscribers);
            try
            {
                subscribers.forEach((subscriber) => {
                    let event: DocumentStatusEvent = {
                        documentId: documentId,
                        data: document.status,
                        token: subscriber.token,
                        event: documentId
                    };
                    subscriber.handle(event, () => {pubsub.unsubscribe(documentId, subscriber.token);});
                })
            }
            catch (err) {
                console.error(err);
            }

        });
    };

    useEffect(() => {
        try {
            if (!watchDocuments) return;
            if (pubsub.events.size === 0) return;

            const invervalId = setInterval(() => {
                fetchStatusForDocuments();
            }, 1000);

            return () => {
                clearInterval(invervalId);
            };
        } catch (err) {
            console.error(err);
        }
    }, [watchDocuments, pubsub.events]);

    return {
        watchDocumentStatus,
        unwatchDocumentStatus
    };
};