import { useState } from "react";
import { Button } from "rizzui";
import { ArrowUpTrayIcon } from '@heroicons/react/24/solid';
import { useModal } from "../../hooks/useModal";
import { MultiValue, ActionMeta } from 'react-select';
import { UploadModal } from "../ui/UploadModal";

export default function Header() {
  const { Modal, isOpen, openModal, closeModal } = useModal();
  const [title, setTitle] = useState("");
  const [tags, setTags] = useState<MultiValue<{ label: string; value: string }>>([]);  // For multi-select
  const [file, setFile] = useState<File | null>(null);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const tagsArray = tags.map(tag => tag.value);
    console.log("Uploading:", { title, tags: tagsArray, file });
    closeModal();
    resetForm();
  };

  const handleCloseModal = () => {
    closeModal();
    resetForm();
  };

  const resetForm = () => {
    setTitle("");
    setTags([]);
    setFile(null);
  };

  const handleTagChange = (newValue: MultiValue<{ label: string; value: string }>, actionMeta: ActionMeta<{ label: string; value: string }>) => {
    setTags(newValue);
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

      <UploadModal isOpen={isOpen} closeModal={closeModal} handleSubmit={handleSubmit} title={title} tags={tags} setTitle={setTitle} handleTagChange={handleTagChange} file={file} setFile={setFile} />
    </>
  );
}
