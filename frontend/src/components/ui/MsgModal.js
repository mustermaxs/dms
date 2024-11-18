import { useEffect, useState } from "react";
import "./MsgModal.css";
import { FiX } from "react-icons/fi";
import { ActionIcon } from "rizzui";

function MsgModal({
  title,
  message,
  handleClick,
  isVisible,
  type,
  setIsVisible,
}) {
  const [isOpen, setIsOpen] = useState(false);
  const style = ["error", "normal"].includes(type) ? type : "error";

  useEffect(() => {
    setIsOpen(isVisible);
  }, [isVisible]);

  const handleClose = () => {
    setIsOpen(false);
    setIsVisible(false);
  };

  // if (!isOpen) return null;
  return (
    <>
      {isOpen &&(
        <div className="msgModal">

          <div id="text">
            <p>{message}</p>
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
