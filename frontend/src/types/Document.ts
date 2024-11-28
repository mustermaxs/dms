import { Tag } from "./Tag";

export type UploadDocumentDto = {
    title: string;
    content: string;
    tags: Tag[];
    fileType: string;
  }

export type UpdateDocumentDto = {
    id: string;
    title: string;
    tags: Tag[];
}

export type Document = {
    id: string;
    title: string;
    content: string;
    tags: Tag[];
    modificationDateTime: string;
    uploadDateTime: string;
    status: DocumentStatus;
    fileExtension: string;
  }

  export type DocumentContentDto = {
    id: string;
    content: string;
  }

  export enum DocumentStatus {
    Pending,
    NotStarted,
    Finished,
    Failed
  }

  export type DocumentType = {
    name: string;
  }

  export type DocumentSearchResult = {
    id: string;
    title: string;
    match: string | null;
    uploadDateTime: string;
    modificationDateTime: string;
    status: DocumentStatus;
    tags: Tag[];
    documentType: DocumentType;
  }

  export type SearchDocumentResponse = {
    success: boolean;
    message: string;
    content: DocumentSearchResult[];
  }
