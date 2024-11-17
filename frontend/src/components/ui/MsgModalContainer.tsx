import { useContext, useEffect, useState } from "react";
import "./MsgModal.css";
import MsgModal from "./MsgModal";
import AppContext from "../context/AppContext";

export const useMsgModal = () => {
    const [messages, setMessages] = useState<{ id: string; content: string }[]>([]);

    const addMessage = (message: string) => {
        const newMessage = { id: Date.now().toString(), content: message };
        console.log("ADDED MESSAGE " + newMessage.content);
        setMessages((prevMessages) => [...prevMessages, newMessage]);
        console.log(messages);
    };

    const removeMessage = (messageId: string) => {
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

export const MsgModalContainer = () => {
    const { messages, addMessage, removeMessage } = useContext(AppContext);
    // Example usage of addMessage
    const handleAddMessage = () => {
        addMessage("This is a new message!");
        console.log("!!! MESSAGE:" + messages);
    };

    return (
        <>
                <div>
                    <div className="msgModalContainer">
                        {messages.map((message) => (
                            <MsgModal
                                key={message.id}
                                message={message.content}
                                handleClick={() => {}}
                                title={undefined}
                                isVisible={true}
                                type={undefined}
                                setIsVisible={removeMessage} />
                        ))}
                    </div>
                </div>
        </>
    );
}
