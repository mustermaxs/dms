import Modal from "../shared/Modal";
import Label from "../shared/Label";
import { DateFormatter } from "../../services/dateFormatter";
import { Document } from "../../types/Document";
import { Button } from "rizzui";
import "../ui/DocumentModal.css";

export const DocumentModal = ({ isOpen, closeModal, selectedDocument }) => {
  const document: Document = selectedDocument.selectedDocument;

  return (
        <Modal isOpen={isOpen} closeModal={closeModal} title={`${document.title}`}>
          <Label title="Upload Datetime" />
          <p className="my-2">{DateFormatter.toDateString(new Date(document.uploadDateTime))}</p>
          {document.modificationDateTime != null && <><Label title="Modification Datetime" /><p className="my-2">{DateFormatter.toDateString(new Date(document.modificationDateTime))}</p></>}
          <Label title="Tags" />
          <ul className="mb-4">
            {document.tags.map((tag, index) => (
              <li key={index} className="inline-block bg-blue-100 text-blue-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">
                {tag.label}
              </li>
            ))}
          </ul>
          <Label title="Content" />
          <p className="dms-document-content my-2">{document.content}</p>
          <div className="flex justify-end">
            {/* TODO Implement Edit Mode */}
          <Button type="button" className="mt-4"> 
            Edit
          </Button>
          </div>
        </Modal>
    )
}
