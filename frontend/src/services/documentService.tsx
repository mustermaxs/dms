import {Document, UploadDocumentDto} from "../types/Document";
import { HttpService } from "./httpService";

export interface IDocumentService {
    getDocument(id: string): Promise<Document>;
    getAllDocuments(): Promise<Document[]>;
    uploadDocument(document: UploadDocumentDto): Promise<void>;
}

export class MockDocumentService implements IDocumentService {
    getAllDocuments(): Promise<Document[]> {
        return Promise.resolve(
            [{
                id: "234978fe978fs987",
                title: "Document 1.pdf",
                content: "Lorem ipsum",
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
                content: "Lorem ipsum dolor blablabla",
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
        return Promise.resolve(
            {
                id: id,
                title: "Document 1.pdf",
                content: "Lorem ipsum",
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
            } as Document);
    }
    uploadDocument(document: UploadDocumentDto): Promise<void> {
        return Promise.resolve();
    }
}

export class DocumentService implements IDocumentService {
    private httpService: HttpService;

    constructor() {
        this.httpService = new HttpService();
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
}