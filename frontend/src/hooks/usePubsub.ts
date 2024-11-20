import { useCallback, useRef } from "react";
import { createToken } from "../services/uploadProgressInfo";

export interface PubsubEvent {
    event: string;
    data: any;
    token: string;
};

export type Subscriber = {
    token: string;
    handle: (ev: PubsubEvent, unsubscribe: () => void) => void;
};

export class PubSub {
    public events: Map<string, Subscriber[]>;

    constructor() {
        this.events = new Map();
    }

    private createSubscriber(callback: (ev: PubsubEvent) => void): Subscriber {
        return {
            token: createToken(),
            handle: callback
        }
    }

    public topicExists(topic: string): boolean {
        return this.events.has(topic);
    }

    public hasTopicSubscribers(topic: string): boolean {
        if (!this.events.has(topic)) return false;

        return this.events.get(topic)!.length > 0;
    }

    subscribe(event: string, callback: (ev: PubsubEvent) => void): () => void {
        if (!this.topicExists(event)) {
            this.events.set(event, []);
        }

        const subscriber = this.createSubscriber(callback);
        this.events.get(event)!.push(subscriber);

        return () => this.unsubscribe(event, subscriber.token);
    }


    unsubscribe(event: string, token: string): void {
        if (!this.events.has(event)) return;

        const subscribers: Subscriber[] = this.events.get(event)!;
        const index = subscribers.findIndex(subscriber => subscriber.token === token);

        if (index !== -1) {
            subscribers.splice(index, 1);
        }

        if (subscribers.length === 0) {
            this.events.delete(event);
        }
    }


    publish(topic: string, event: PubsubEvent): void {
        if (!this.events.has(topic)) return;

        const subscribers = this.events.get(topic)!;

        for (const subscriber of subscribers) {
            subscriber.handle(event, () => {this.unsubscribe(topic, subscriber.token)});
        }
    }

    public removeTopic(topic: string): void {
        this.events.delete(topic);
    }
}

export function usePubsub() {
    const pubsubRef = useRef(new PubSub());

    const subscribe = (event: string, callback: (ev: PubsubEvent) => void): (() => void) => {
            return pubsubRef.current.subscribe(event, callback);
        };

    const unsubscribe = useCallback((event: string, token: string): void => {
        pubsubRef.current.unsubscribe(event, token);
    }, []);

    const publish = (topic: string, event: PubsubEvent): void => {
        pubsubRef.current.publish(topic, event);
    };

    return {
        subscribe,
        unsubscribe,
        publish,
        hasTopicSubscribers: (topic: string) => pubsubRef.current.hasTopicSubscribers(topic),
        topicExists: (topic: string) => pubsubRef.current.topicExists(topic),
        removeTopic: (topic: string) => pubsubRef.current.removeTopic(topic),
        events: pubsubRef.current.events,
        getSubscribersForTopic: (topic: string) => pubsubRef.current.events.get(topic)
    };
}