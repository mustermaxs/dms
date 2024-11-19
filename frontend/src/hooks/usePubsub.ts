type EventCallback = (...args: any[]) => void;

class PubSub {
  private events: Map<string, EventCallback[]>;

  constructor() {
    this.events = new Map();
  }

  // Subscribe to an event
  subscribe(event: string, callback: EventCallback): () => void {
    if (!this.events.has(event)) {
      this.events.set(event, []);
    }
    this.events.get(event)!.push(callback);

    // Return an unsubscribe function
    return () => this.unsubscribe(event, callback);
  }

  // Unsubscribe from an event
  unsubscribe(event: string, callback: EventCallback): void {
    if (!this.events.has(event)) return;

    const listeners = this.events.get(event)!;
    const index = listeners.indexOf(callback);

    if (index !== -1) {
      listeners.splice(index, 1);
    }

    // Clean up the event if no listeners remain
    if (listeners.length === 0) {
      this.events.delete(event);
    }
  }

  // Publish an event
  publish(event: string, ...args: any[]): void {
    if (!this.events.has(event)) return;

    const listeners = this.events.get(event)!;
    for (const callback of listeners) {
      callback(...args);
    }
  }
}

// Usage example
const pubsub = new PubSub();

// Subscriber 1
const unsubscribe1 = pubsub.subscribe("my-event", (data) => {
  console.log("Subscriber 1 received:", data);
});

// Subscriber 2
const unsubscribe2 = pubsub.subscribe("my-event", (data) => {
  console.log("Subscriber 2 received:", data);
});

// Publisher emits an event
pubsub.publish("my-event", { message: "Hello, PubSub!" });

// Unsubscribe Subscriber 1
unsubscribe1();

// Publisher emits another event
pubsub.publish("my-event", { message: "Subscriber 1 should not receive this." });
