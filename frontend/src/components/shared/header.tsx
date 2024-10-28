import { useEffect, useState } from "react";
import { Button } from "rizzui";
import { ArrowUpTrayIcon } from '@heroicons/react/24/solid';
import { useModal } from "../../hooks/useModal";
import { UploadModal } from "../ui/UploadModal";
import { Tag } from "../../types/Tag";
import { ServiceLocator } from "../../serviceLocator";
import { IDocumentService } from "../../services/documentService";
import { getEmptyGuid } from "../../services/guidGenerator";
import { ModalSize } from "./Modal";

export default function Header() {
  const { Modal, isOpen, openModal, closeModal } = useModal();
  const [title, setTitle] = useState("");
  const [tags, setTags] = useState<Tag[]>([]);  // For multi-select
  const [selectedTags, setSelectedTags] = useState<Tag[]>([]);
  const [file, setFile] = useState<File | null>(null);


  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    let documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    console.log(title, tags);
    documentService.uploadDocument(
      {
        id: getEmptyGuid(),
        title: title,
        tags: selectedTags,
        content: "Document content goes here",
      }
    ).then((res) => {
    })
    console.log(selectedTags);
    const tagsArray = tags.map(tag => tag.value);
    closeModal();
    resetForm();
  };


  const resetForm = () => {
    setTitle("");
    setTags([]);
    setFile(null);
  };

  useEffect(() => {
    console.log("Selected tags updated: ", selectedTags);
  }, [selectedTags]);

  const handleTagChange = (newValue: Tag[]) => {
    let tagsWithoutIds: Tag[] = newValue.filter(t => t.id === "" || t.id === undefined);

    let updatedTagsWithoutIds = tagsWithoutIds.map(t => ({
      id: 'd290f1ee-6c54-4b01-90e6-d701748f0851',
      label: t.label,
      color: "red",
      value: t.value,
    }));

    setTags(prevTags => [...prevTags, ...updatedTagsWithoutIds]);

    console.log("Tags without ids", updatedTagsWithoutIds);

    let validTags = newValue.filter(t => t.id !== "" && t.id !== undefined);
    newValue = [...validTags, ...updatedTagsWithoutIds];
    console.log("Updated tags", newValue);

    setSelectedTags(newValue);
  };



  return (
    <>
      <div className="my-6 flex items-center justify-between">
        <h1 className="text-3xl font-bold">Document Management System</h1>
        <Button variant="outline" className="ml-2" onClick={openModal}>
          <ArrowUpTrayIcon className="w-4 h-4 mr-1" />
          Upload
        </Button>
      </div>

      <UploadModal size={ModalSize.SMALL} isOpen={isOpen} closeModal={closeModal} handleSubmit={handleSubmit} selectedTags={selectedTags} title={title} tags={tags} setTitle={setTitle} handleTagChange={handleTagChange} file={file} setFile={setFile} />
    </>
  );
}
