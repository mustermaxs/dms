interface HttpResponse<T> {
    data: T;
    status: number;
}

export class HttpService {
    public readonly config: any;
    public readonly baseUrl: string;

    public constructor() { 
        this.config = require('../config.json');
        this.baseUrl = this.config.apiUrl;
    }

    private urlBuilder(url: string): string {
        return this.baseUrl + url;
    }

    public async get<T>(url: string): Promise<HttpResponse<T>> {
        let completeUrl: string = this.urlBuilder(url);

        return fetch(completeUrl, {
            method: 'GET',
            mode: 'cors',
            cache: 'no-cache',
        })
            .then(async response => ({
                data: (await response.json()).content,
                status: response.status
            }));
    }


    public async post<T>(url: string, data: any): Promise<HttpResponse<T>> {
        let completeUrl: string = this.urlBuilder(url);

        return fetch(completeUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(async response => ({
                data: (await response.json()).content,
                status: response.status
            }));
    }

    public async put<T>(url: string, data: any): Promise<HttpResponse<T>> {
        let completeUrl: string = this.urlBuilder(url);

        return fetch(completeUrl, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(async response => ({
                data: (await response.json()).content,
                status: response.status
            }));
    }

    public async delete<T>(url: string): Promise<HttpResponse<T>> {
        let completeUrl: string = this.urlBuilder(url);

        return fetch(completeUrl, {
            method: 'DELETE'
        })
            .then(async response => ({
                data: (await response.json()).content,
                status: response.status
            }));
    }
}