import { useState } from "react";

export const useMsgModal = () => {  // TODO masi BUG masi messages get same key
    const [messages, setMessages] = useState<{ id: number; content: string }[]>([]);

    const getNewMsgIndex = () => {
        return new Date().getTime();
    };

    const addMessage = (message: string) => {
        const newMessage = { id: getNewMsgIndex(), content: message };
        console.log("[useMsgModal] Message: ", message);
        setMessages((prevMessages) => [...prevMessages, newMessage]);
    };

    const removeMessage = (messageId: number) => {
        console.log("[useMsgModal] Removing message " + messageId);
        if (messages.findIndex((m) => m.id === messageId) === -1) 
            console.error("[useMsgModal] Message " + messageId + " not found");
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