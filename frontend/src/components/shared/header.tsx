import { Button } from "rizzui";
import { ArrowUpTrayIcon } from '@heroicons/react/24/solid';
import { useModal } from "../../hooks/useModal";
import { UploadModal } from "../ui/UploadModal";
import { ModalSize } from "./Modal";
import { MsgModalContainer, useMsgModal } from "../ui/MsgModalContainer";
import { useContext, useEffect } from "react";
import AppContext from "../context/AppContext";

export default function Header() {
  const { isOpen, openModal, closeModal } = useModal();
  const {addMessage, messages} = useContext(AppContext);

  useEffect(() => {
    addMessage("BLABLA to the Document Management System!");
    console.log("ADD MESSAGE");
  }, []);

  return (
    <>
      <div className="my-6 flex items-center justify-between">
            <MsgModalContainer />
        <h1 className="text-3xl font-bold">📃Document Management System</h1>
        <Button variant="outline" className="ml-2" onClick={openModal}>
          <ArrowUpTrayIcon className="w-4 h-4 mr-1" />
          Upload
        </Button>
      </div>

      <UploadModal size={ModalSize.SMALL} isOpen={isOpen} closeModal={closeModal} />
    </>
  );
}
