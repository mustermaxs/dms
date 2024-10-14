import React from 'react';
import { Modal } from "rizzui";

interface DocumentModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  content: string;
}

const DocumentModal: React.FC<DocumentModalProps> = ({ isOpen, onClose, title, content }) => {
  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <div className="p-4">
        <h1 className="text-xl font-bold">{title}</h1>
        <p>{content}</p>
      </div>
    </Modal>
  );
};

export default DocumentModal;
