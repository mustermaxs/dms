import { Tag } from "../types/Tag";

export function getTags(): Promise<Tag[]> {
    return fetch('/api/tags')
        .then(response => response.json())
        .then(data => data.tags);
}