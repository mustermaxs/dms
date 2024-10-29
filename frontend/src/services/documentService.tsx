import { upload } from "@testing-library/user-event/dist/upload";
import {Document, UploadDocumentDto} from "../types/Document";
import { HttpService } from "./httpService";

export interface IDocumentService {
    getDocument(id: string): Promise<Document>;
    getAllDocuments(): Promise<Document[]>;
    uploadDocument(document: UploadDocumentDto): Promise<void>;
    getDocumentContent(id: string): Promise<string>;
}

export class MockDocumentService implements IDocumentService {
    getDocumentContent(id: string): Promise<string> {
        console.log("ID", id);
        return Promise.resolve("content");
    }
    getAllDocuments(): Promise<Document[]> {
        return Promise.resolve(
            [{
                id: "234978fe978fs987",
                title: "Document 1.pdf",
                content: "Lorem ipsum",
                modificationDateTime: '2020-01-01T00:00:00',
                uploadDateTime: '2020-01-01T00:00:00',
                tags: [
                    {
                        id: '2',
                        label: 'personal',
                        color: 'blue',
                        value: 'personal'
                    },
                    {
                        id: '3',
                        label: 'family',
                        color: 'green',
                        value: 'family'
                    }
                ],
            },
            {
                id: "14987sgkjh25",
                title: "Document 2.pdf",
                content: "Lorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablabla",
                uploadDateTime: '2020-01-01T00:00:00',
                tags: [
                    {
                        id: '2',
                        label: 'penis',
                        color: 'blue',
                        value: 'penis'
                    },
                    {
                        id: '3',
                        label: 'hobby',
                        color: 'green',
                        value: 'hobby'
                    }
                ],
            }
        ] as Document[]);
    }
    getDocument(id: string): Promise<Document> {
        let mockDocks: Map<string, Document> = new Map();

        let mockDock1: Document = {
            id: id,
            title: "Document 1.pdf",
            content: "Lorem ipsum",
            uploadDateTime: '2020-01-01T00:00:00',
            tags: [
                {
                    id: '2',
                    label: 'personal',
                    color: 'blue',
                    value: 'personal',
                    
                },
                {
                    id: '3',
                    label: 'family',
                    color: 'green',
                    value: 'family',
                }
            ],
        } as Document;

        let mockDock2: Document =             {
            id: "14987sgkjh25",
            title: "Document 2.pdf",
            content: "Lorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablablaLorem ipsum dolor blablabla",
            uploadDateTime: '2020-01-01T00:00:00',
            tags: [
                {
                    id: '2',
                    label: 'penis',
                    color: 'blue',
                    value: 'penis'
                },
                {
                    id: '3',
                    label: 'hobby',
                    color: 'green',
                    value: 'hobby'
                }
            ],
        } as Document;

        mockDocks.set('234978fe978fs987', mockDock1);
        mockDocks.set("14987sgkjh25", mockDock2);

        return Promise.resolve(mockDocks.get(id) as Document);
    }
    uploadDocument(document: UploadDocumentDto): Promise<void> {
        return Promise.resolve();
    }
    updateDocument(document: Document): Promise<void> {
        return Promise.resolve();
    }
}

export class DocumentService implements IDocumentService {
    private httpService: HttpService;

    constructor() {
        this.httpService = new HttpService();
    }
    getDocumentContent(id: string): Promise<string> {
        throw new Error("Method not implemented.");
    }
    public async getAllDocuments(): Promise<Document[]> {
        return await this.httpService.get<Document[]>('Documents');
    }
    public async uploadDocument(document: UploadDocumentDto): Promise<void> {
        console.log("DOCUMENT: ", document);
        return await this.httpService.post<void>('Documents', document);
    }
    public async getDocument(id: string): Promise<Document> {
        return await this.httpService.get<Document>(`Documents/${id}`);
    }
    public async updateDocument(document: Document): Promise<void> {
        return await this.httpService.put<void>(`Documents/${document.id}`, document);
    }
}