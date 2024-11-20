import { useState } from "react";

export const useMsgModal = () => {
    const [messages, setMessages] = useState<{ id: number; content: string }[]>([]);
    const [msgIndex, setMsgIndex] = useState(0);

    const addMessage = (message: string) => {
        setMsgIndex(msgIndex + 1);
        const newMessage = { id: msgIndex, content: message };
        console.log("[useMsgModal] Message: ", message);
        setMessages((prevMessages) => [...prevMessages, newMessage]);
    };

    const removeMessage = (messageId: number) => {
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