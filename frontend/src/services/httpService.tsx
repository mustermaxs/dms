export class HttpService {
    public readonly config: any;
    public readonly baseUrl: string;

    public constructor() { 
        this.config = require('../config.json');
        this.baseUrl = this.config.apiUrl;
        console.log(this.baseUrl);
    }

    private urlBuilder(url: string): string {
        return this.baseUrl + url;
    }

    public async get<T>(url: string): Promise<T> {
        let completeUrl: string = this.urlBuilder(url);

        return fetch(completeUrl, {
            method: 'GET',
            mode: 'cors',
            cache: 'no-cache',
        })
            .then(response => response.json())
            .then(data => data);
    }
    public async post<T>(url: string, data: any): Promise<T> {
        let completeUrl: string = this.urlBuilder(url);

        return fetch(completeUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())
            .then(data => data);
    }

    public async put<T>(url: string, data: any): Promise<T> {
        let completeUrl: string = this.urlBuilder(url);

        return fetch(completeUrl, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())
            .then(data => data);
    }

    public async delete<T>(url: string): Promise<T> {
        let completeUrl: string = this.urlBuilder(url);

        return fetch(completeUrl, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => data);
    }
}