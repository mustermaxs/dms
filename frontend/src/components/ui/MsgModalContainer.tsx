import { useContext, useEffect, useState } from "react";
import "./MsgModal.css";
import MsgModal from "./MsgModal";
import AppContext from "../context/AppContext";

export const MsgModalContainer = () => {
    const { messages, addMessage, removeMessage } = useContext(AppContext);
    
    return (
        <>
                <div>
                    <div className="msgModalContainer">
                        {messages.map((message) => (
                            <MsgModal
                                key={message.id}
                                messageId={message.id}
                                message={message.content}
                                handleClick={undefined}
                                title={undefined}
                                type={undefined}
                                />
                        ))}
                    </div>
                </div>
        </>
    );
}
