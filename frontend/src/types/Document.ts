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