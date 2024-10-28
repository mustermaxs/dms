import { Tag } from "../types/Tag";
import { HttpService } from "./httpService";

export function getTags(): Promise<Tag[]> {
    return fetch('/api/tags')
        .then(response => response.json())
        .then(data => data.tags);
}

export interface ITagService {
    getTags(): Promise<Tag[]>;
}

export class MockTagService implements ITagService {
    constructor() {
        console.log("MockTagService constructor called");
    }

    getTags(): Promise<Tag[]> {
        let tags: Tag[] = [
            {
                id: '1',
                label: 'work',
                color: 'red',
                value: 'work',
            },
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
        ];
        return Promise.resolve(tags);
    }
}

export class TagService implements ITagService {
    private httpService: HttpService;
    
    constructor() {
        this.httpService = new HttpService();
    }

    getTags(): Promise<Tag[]> {
        return this.httpService.get<Tag[]>('Tags');
    }
}