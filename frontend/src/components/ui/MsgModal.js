import { useContext, useEffect, useState } from "react";
import "./MsgModal.css";
import { FiX } from "react-icons/fi";
import { ActionIcon } from "rizzui";
import AppContext from "../context/AppContext";

function MsgModal({
  title,
  message,
  handleClick,
  type,
  messageId
}) {
  const [isOpen, setIsOpen] = useState(true);
  const {removeMessage} = useContext(AppContext);
  const style = ["error", "normal"].includes(type) ? type : "error";

  const autoHideAfterSeconds = (seconds) => {
    setTimeout(() => {
      if (!isOpen || !isOpen) return;
      setIsOpen(false);
      removeMessage(messageId);
    }, seconds);
  };

  useEffect(() => {
    setIsOpen(isOpen);
    autoHideAfterSeconds(10000);
  }, [isOpen]);

  const handleClose = () => {
    setIsOpen(false);
  };

  return (
    <>
      {isOpen &&(
        <div className="msgModal">
          <div id="text">
            {message}
          </div>
          <ActionIcon
            variant="outline"
            size="sm"
            rounded="full"
            onClick={handleClose}
          >
            <FiX />
          </ActionIcon>
        </div>
      )}
    </>
  );
}

export default MsgModal;
