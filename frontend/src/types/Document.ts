import { Tag } from "./Tag";

type Document = {
    id: string;
    title: string;
    content: string;
    tags: Tag[];
  }

export default Document;