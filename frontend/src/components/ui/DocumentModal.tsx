import Modal from "../shared/Modal";
import Label from "../shared/Label";
import { DateFormatter } from "../../services/dateFormatter";
import { Document, UpdateDocumentDto } from "../../types/Document";
import { Button, Input } from "rizzui";
import "../ui/DocumentModal.css";
import { useModal } from "../../hooks/useModal";
import { ContentViewerModal } from "./ContentViewerModal";
import { useContext, useState, useEffect } from "react";
import { TagInput } from "../shared/TagInput";
import TagContext from "../context/AppContext";
import { FaEye } from "react-icons/fa";
import { useDocuments } from "../../hooks/useDocuments";

export const DocumentModal = ({ isOpen, closeModal, selectedDocument }) => {
  const document: Document = selectedDocument.selectedDocument;
  const contentModal = useModal();
  const [isEditMode, setIsEditMode] = useState(false);
  const [editedTitle, setEditedTitle] = useState(document.title);
  const [editedTags, setEditedTags] = useState(
    document.tags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", value: tag.id, label: tag.label, color: "#ffffff" }))
  );

  const { updateDocument } = useDocuments();

  const { availableTags } = useContext(TagContext);

  useEffect(() => {
    setEditedTitle(document.title);
    setEditedTags(document.tags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", value: tag.value, label: tag.label, color: "#ffffff" })));
    setIsEditMode(false);
  }, [document]);

  useEffect(() => {
    console.log(editedTags);
  }, [editedTags]);

  const handleSave = () => {
    const updatedDocument = {
      id: document.id,
      title: editedTitle,
      tags: editedTags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", label: tag.label, color: "#000000", value: tag.value })),
    } as UpdateDocumentDto;

    updateDocument(updatedDocument as UpdateDocumentDto);

    setIsEditMode(false);
  };

  const handleClose = () => {
    setEditedTags(document.tags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", label: tag.label, color: "#000000", value: tag.value  })));
    setIsEditMode(false);
    closeModal();
  }

  const handleCancel = () => {
    setEditedTitle(document.title);
    setEditedTags(document.tags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", label: tag.label, color: "#000000", value: tag.value  })));
    setIsEditMode(false);
  }

  const ViewMode = () => (
    <>
      <Label title="Upload Datetime" />
      <p className="my-2">{DateFormatter.toDateString(new Date(document.uploadDateTime))}</p>
      
      {document.modificationDateTime && (
        <>
          <Label title="Modification Datetime" />
          <p className="my-2">{DateFormatter.toDateString(new Date(document.modificationDateTime))}</p>
        </>
      )}
      
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
    </>
  );

  const EditMode = () => (
    <>
      <div className="mb-4">
        <Label title="Title" />
        <Input
          type="text"
          value={editedTitle}
          onChange={(e) => setEditedTitle(e.target.value)}
          className="mb-4"
        />
      </div>

      {document.modificationDateTime && (
        <>
          <Label title="Modification Datetime" />
          <p className="my-2">{DateFormatter.toDateString(new Date(document.modificationDateTime))}</p>
        </>
      )}

      <Label title="Tags" />
      <TagInput
        selectedTags={editedTags}
        onChange={setEditedTags}
        availableTags={availableTags}
        className="mb-4"
      />

      <Label title="Content" />
      <p className="dms-document-content my-2">{document.content}</p>
    </>
  );

  return (
    <>
      <Modal isOpen={isOpen} closeModal={handleClose} title={isEditMode ? "Edit Document" : document.title}>
        {isEditMode ? <EditMode /> : <ViewMode />}
        <div className="flex justify-between">
          <div className="flex justify-start">
            <Button variant="outline" type="button" onClick={() => contentModal.openModal()} className="mt-4"> 
            <FaEye />

          </Button>
        </div>
        <div className="flex justify-end">

          {isEditMode ? (
            <>
              <Button type="button" onClick={handleCancel} className="mt-4 mx-1"> 
                Cancel
              </Button>
              <Button type="button" onClick={handleSave} className="mt-4 mx-1 bg-green-600"> 
                Save
              </Button>
            </>
          ) : (
            <Button type="button" onClick={() => setIsEditMode(true)} className="mt-4 mx-1"> 
              Edit
            </Button>
          )}
          </div>
        </div>
      </Modal>
      {document.title && <ContentViewerModal isOpen={contentModal.isOpen} closeModal={contentModal.closeModal} openModal={contentModal.openModal} document={document} />}
    </>
  );
};
