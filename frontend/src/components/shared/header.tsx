import { Button } from "rizzui";
import { ArrowUpTrayIcon } from '@heroicons/react/24/solid';
import { useModal } from "../../hooks/useModal";
import { UploadModal } from "../ui/UploadModal";
import { ModalSize } from "./Modal";
import { MsgModalContainer } from "../ui/MsgModalContainer";
import { useEffect } from "react";

export default function Header() {
  const { isOpen, openModal, closeModal } = useModal();

  useEffect(() => {
  }, []);

  return (
    <>
      <div className="bg-white border-b sticky top-0">
        <div className="max-w-screen-2xl mx-auto py-3 mb-4">
          <div className="flex items-center justify-between">
            <h1 className="flex items-center">
              <span className="text-2xl font-bold">
                Document Management System
              </span>
            </h1>
            <div className="flex items-center gap-3">
              <MsgModalContainer />
              <Button 
                variant="solid"
                className="flex items-center px-4 py-2 text-sm font-medium bg-gray-900 text-white hover:bg-gray-800 transition-colors"
                onClick={openModal}
              >
                <ArrowUpTrayIcon className="w-4 h-4 mr-2" />
                Upload Document
              </Button>
            </div>
          </div>
        </div>
      </div>

      <UploadModal size={ModalSize.SMALL} isOpen={isOpen} closeModal={closeModal} />
    </>
  );
}
