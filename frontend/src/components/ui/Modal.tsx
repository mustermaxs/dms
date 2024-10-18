import React, { useEffect, useRef, memo } from 'react';
import { FiX } from "react-icons/fi";
import { ActionIcon } from "rizzui";

interface ModalProps {
  title: string;
  children: React.ReactNode;
  isOpen: boolean;
  closeModal: () => void;
}

const Modal: React.FC<ModalProps> = memo(({ title, children, isOpen, closeModal }) => {
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
        className="bg-white rounded-lg shadow-lg w-full max-w-md hover:cursor-default"
        onClick={(e) => e.stopPropagation()}  // Prevent close when clicking inside modal
      >
        <div className="flex justify-between items-center border-b p-4">
          <h2 className="text-lg font-semibold">{title}</h2>
          <ActionIcon
            variant="outline"
            size="sm"
            rounded="full"
            onClick={closeModal}
          >
            <FiX />
          </ActionIcon>
        </div>
        <div className="p-4">
          {children}
        </div>
      </div>
    </div>
  );
});

export default Modal;