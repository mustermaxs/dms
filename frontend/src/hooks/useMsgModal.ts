import { useState } from "react";

export const useMsgModal = () => {
    const [messages, setMessages] = useState<{ id: string; content: string }[]>([]);

    const addMessage = (message: string) => {
        const newMessage = { id: Date.now().toString(), content: message };
        console.log("ADDED MESSAGE " + newMessage.id);
        setMessages((prevMessages) => [...prevMessages, newMessage]);
    };

    const removeMessage = (messageId: string) => {
        console.log("REMOVE MESSAGE " + messageId);
        setMessages((prevMessages) => prevMessages.filter((m) => m.id !== messageId));
    };

    const getMessages = () => {
        return messages;
    };

    return {
        messages,
        addMessage,
        removeMessage,
        getMessages,
    };
}