import React, { useEffect, useRef, memo } from 'react';
import { FiX } from "react-icons/fi";
import { ActionIcon } from "rizzui";

export class ModalSize {
  static SMALL = {width: '30rem', height: '21rem'};
  static MEDIUM = {width: '50rem', height: '10rem'};
  static LARGE = {width: '70rem', height: '90%'};
}


interface ModalProps {
  title: string;
  children: React.ReactNode;
  isOpen: boolean;
  closeModal: () => void;
  size?: typeof ModalSize.SMALL | typeof ModalSize.MEDIUM | typeof ModalSize.LARGE;
}

const Modal: React.FC<ModalProps> = memo(({ title, children, isOpen, closeModal, size=ModalSize.SMALL }) => {
  const modalRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleEsc = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        closeModal();
      }
    };

    if (isOpen) {
      document.addEventListener("keydown", handleEsc);
      modalRef.current?.focus();  // Focus on the modal when it's open
    } else {
      document.removeEventListener("keydown", handleEsc);
    }

    return () => document.removeEventListener("keydown", handleEsc);
  }, [isOpen]);

  if (!isOpen) return null;

  return (
    <div
      className="fixed inset-0 z-10 flex items-center justify-center bg-black bg-opacity-50 hover:cursor-pointer"
      onClick={closeModal}
      ref={modalRef}
      tabIndex={-1}
    >
      <div
      style={{maxWidth: size.width, minHeight: size.height}}
        className={"bg-white rounded-lg shadow-lg w-full hover:cursor-default"}
        onClick={(e) => e.stopPropagation()}
      >
        <div 
        style={{maxHeight: '6rem'}}
        className="flex justify-between items-center border-b p-4">
          <h2
          style={{paddingRight: '3rem',  textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden'}}
          title={title}
          className="text-lg font-semibold">{title}</h2>
          <ActionIcon
            variant="outline"
            size="sm"
            rounded="full"
            onClick={closeModal}
          >
            <FiX />
          </ActionIcon>
        </div>
        <div style={{minHeight: '100%'}} className="p-4">
          {children}
        </div>
      </div>
    </div>
  );
});

export default Modal;