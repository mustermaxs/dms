import Modal from "../shared/Modal";
import Label from "../shared/Label";
import { DateFormatter } from "../../services/dateFormatter";
import { Document, DocumentContentDto, DocumentStatus, UpdateDocumentDto } from "../../types/Document";
import { Button, Input } from "rizzui";
import "../ui/DocumentModal.css";
import { useModal } from "../../hooks/useModal";
import { ContentViewerModal } from "./ContentViewerModal";
import { useContext, useState, useEffect } from "react";
import { TagInput } from "../shared/TagInput";
import { FaEye } from "react-icons/fa";
import { useDocuments } from "../../hooks/useDocuments";
import AppContext from "../context/AppContext";
import { ServiceLocator } from "../../serviceLocator";
import { IDocumentService } from "../../services/documentService";
import { EditDocumentModal } from "./EditDocumentModal";
import "../../App.css";
import { title } from "process";

export const DocumentModal = ({ isOpen, closeModal }) => {
  const { availableTags, setIsLoadingTags, selectedDocument: document, setSelectedDocument, setDocuments, addMessage } = useContext(AppContext);
  const contentModal = useModal();
  const [isEditMode, setIsEditMode] = useState(false);
  const [editedTitle, setEditedTitle] = useState(document.title);
  const [editedTags, setEditedTags] = useState(
    document.tags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", value: tag.id, label: tag.label, color: "#ffffff" }))
  );
  const [documentContentObj, setDocumentContentObj] = useState<DocumentContentDto | null>(null);
  const { updateDocument, deleteDocument } = useDocuments();
  const [isEditable, setIsEditable] = useState<{ [key: string]: boolean }>({
    title: true,
    tags: true,
    content: true,
    delete: true
  });

  useEffect(() => {
    setIsLoadingTags(true);
  }, [document]);

  const handleInputTitleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setEditedTitle(event.target.value);
  };


  useEffect(() => {
    setIsEditable({
      title: true,
      tags: true,
      content: true,
      delete: document.status === DocumentStatus.Finished
    });
    setEditedTitle(document.title);
    setEditedTags(document.tags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", value: tag.value, label: tag.label, color: "#ffffff" })));
    setIsEditMode(false);
  }, [document]);

  useEffect(() => {
  }, [editedTags]);

  const handleSave = async () => {
    const updatedDocument = {
      id: document.id,
      title: editedTitle,
      tags: editedTags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", label: tag.label, color: "#000000", value: tag.value })),
    } as UpdateDocumentDto;

    const updatedDoc = await updateDocument(updatedDocument);

    if (!updatedDoc) {
      console.log("Error updating document ", document.id);
      return;
    }

    setSelectedDocument(updatedDoc);

    setDocuments((prevDocuments: Document[]) =>
      prevDocuments.map((doc) =>
        doc.id === updatedDoc.id ? { ...doc, ...updatedDoc } : doc
      )
    );

    setIsEditMode(false);
  };

  const handleDelete = async () => {
    if (document.status < DocumentStatus.Finished)
    {
      addMessage("Cannot be deleted. Still processing document...");
      return;
    }
    setDocuments((prevDocuments: Document[]) =>
      prevDocuments.filter((doc) => doc.id !== document.id)
    );
    await deleteDocument(document.id);
    closeModal();
  };

  const handleClose = () => {
    setEditedTags(document.tags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", label: tag.label, color: "#000000", value: tag.value })));
    setIsEditMode(false);
    closeModal();
  }

  const handleCancel = () => {
    setEditedTitle(document.title);
    setEditedTags(document.tags.map(tag => ({ id: tag.id || "00000000-0000-0000-0000-000000000000", label: tag.label, color: "#000000", value: tag.value })));
    setIsEditMode(false);
  }

  const onClickViewContent = () => {
    if (document.status < DocumentStatus.Finished)
    {
      addMessage("Document is still being processed... Try again later.");
      return;
    }

    contentModal.openModal();
    setDocumentContentObj(document);
  };

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

  return (
    <>
      <Modal isOpen={isOpen} closeModal={handleClose} title={isEditMode ? "Edit Document" : document.title}>
        {isEditMode ? <EditDocumentModal
          handleInputTitleChange={handleInputTitleChange}
          isOpen={isOpen}
          closeModal={closeModal}
          document={document}
          availableTags={availableTags}
          editedTitle={editedTitle}
          setEditedTitle={setEditedTitle}
          editedTags={editedTags}
          setEditedTags={setEditedTags}
          editableFields={isEditable} /> : <ViewMode />}
        <div className="flex justify-between">
          <div className="flex justify-start">
            <Button variant="outline" type="button" onClick={() => onClickViewContent()} className="mt-4">
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
                <Button type="button" onClick={handleDelete} className={`${!isEditable.delete && 'btn-disabled'} mt-4 mx-1 bg-red`}>
                  Delete
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
