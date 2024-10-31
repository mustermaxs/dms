import { Tag } from "./Tag";

export type UploadDocumentDto = {
    title: string;
    content: string;
    tags: Tag[];
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
  }