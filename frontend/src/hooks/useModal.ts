import { useState } from "react";
import Modal from "../components/shared/Modal"

export const useModal = () => {

  const [isOpen, setIsOpen] = useState(false);
  const openModal = () => setIsOpen(true);
  const closeModal = () => setIsOpen(false);

  return { Modal, isOpen, openModal, closeModal };
};
