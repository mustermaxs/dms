import { upload } from "@testing-library/user-event/dist/upload";
import {Document, DocumentContentDto, UpdateDocumentDto, UploadDocumentDto} from "../types/Document";
import { HttpService } from "./httpService";
import { mockData } from "./mockData";

export interface IDocumentService {
    getDocument(id: string): Promise<Document>;
    getAllDocuments(): Promise<Document[]>;
    uploadDocument(document: UploadDocumentDto): Promise<void>;
    updateDocument(document: UpdateDocumentDto): Promise<Document>;
    getDocumentContent(id: string): Promise<DocumentContentDto>;
    deleteDocument(id: string): Promise<void>;
}

export class MockDocumentService implements IDocumentService {
    deleteDocument(id: string): Promise<void> {
        return Promise.resolve();
    }
    getDocumentContent(id: string): Promise<DocumentContentDto> {
        console.log("ID", id);
        return Promise.resolve({ id: id, content: mockData.lorem } as DocumentContentDto);
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
                content: mockData.lorem,
                uploadDateTime: '2020-01-01T00:00:00',
                tags: [
                    {
                        id: '2',
                        label: 'important',
                        color: 'blue',
                        value: 'important'
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
            content: mockData.lorem,
            uploadDateTime: '2020-01-01T00:00:00',
            tags: [
                {
                    id: '2',
                    label: 'important',
                    color: 'blue',
                    value: 'important'
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
    updateDocument(document: UpdateDocumentDto): Promise<Document> {
        return Promise.resolve(document as Document);
    }
}

export class DocumentService implements IDocumentService {
    private httpService: HttpService;

    constructor() {
        this.httpService = new HttpService();
    }
    async deleteDocument(id: string): Promise<void> {
        var response = await this.httpService.delete<void>(`Documents/${id}`);
        return response.data;
    }

    public async getDocumentContent(id: string): Promise<DocumentContentDto> {
        const response = await this.httpService.get<DocumentContentDto>(`Documents/${id}/content`);
        return response.data;
    }

    public async getAllDocuments(): Promise<Document[]> {
        const response = await this.httpService.get<Document[]>('Documents');
        return response.data;
    }

    public async uploadDocument(document: UploadDocumentDto): Promise<void> {
        const response = await this.httpService.post<void>('Documents', document);
        if (response.status !== 200 && response.status !== 201) {
            throw new Error('Failed to upload document');
        }
    }

    public async getDocument(id: string): Promise<Document> {
        const response = await this.httpService.get<Document>(`Documents/${id}`);
        return response.data;
    }

    public async updateDocument(document: UpdateDocumentDto): Promise<Document> {
        const response = await this.httpService.put<Document>(`Documents/{document.id}`, document);
        if (response.status !== 200) {
            throw new Error('Failed to update document');
        }
        return response.data;
    }
} 