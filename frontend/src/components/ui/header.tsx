import { useState } from "react";
import { Button, Input } from "rizzui";
import { ArrowUpTrayIcon } from '@heroicons/react/24/solid';
import { useModal } from "../../hooks/useModal";
import CreatableSelect from 'react-select/creatable';
import { MultiValue, ActionMeta } from 'react-select';
import Label from "./Label";

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


  const customStyles = {
    multiValue: (styles: any) => ({
      ...styles,
      backgroundColor: '#DBEAFE', // bg-blue-100
      color: '#1E40AF', // text-blue-800
      fontSize: '0.9rem', // text-xs
      fontWeight: '600', // font-semibold
      padding: '0.3rem 0.4rem', // px-2.5 py-0.5
      borderRadius: '0.375rem', // rounded
      ':hover': {
        cursor: 'pointer',
      },
    }),
    multiValueLabel: (styles: any) => ({
      ...styles,
      color: '#1E40AF', // text-blue-800
      padding: 0,
    }),
    multiValueRemove: (styles: any) => ({
      ...styles,
      color: '#1E40AF', // text-blue-800
      marginLeft: '0.3rem',
      padding: '0.1rem',
      ':hover': {
        backgroundColor: '#BFDBFE',
      },
    }),
    control: (styles: any) => ({
      ...styles,
      fontSize: '0.9rem',
      borderRadius: '0.375rem', // rounded
      ':hover': {
        borderColor: 'black', // border-blue-800
        cursor: 'text',
      },
    }),
    menu: (styles: any) => ({
      ...styles,
      fontSize: '0.9rem',
    }),

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

      <Modal isOpen={isOpen} closeModal={handleCloseModal} title="Upload Document">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <Label title="Title" />
            <Input
              id="title"
              type="text"
              placeholder="Enter a title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
            />
          </div>
          <div>
            <Label title="Tags" />
            <CreatableSelect
              options={[{ label: "Personal", value: "Personal" }, { label: "Work", value: "Work" }, { label: "In Progress", value: "In Progress" }]}
              isMulti
              value={tags}
              onChange={handleTagChange}
              placeholder="Add or create tags..."
              className="mt-1 "
              styles={customStyles}
            />
          </div>
         
          <div>
              <Label title="File" />
              <input
                id="file"
                type="file"
                onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)}
                required
                className="block w-full text-sm text-gray-500
                  file:mr-2 file:py-3 file:px-5
                  file:rounded-lg file:border-0
                  file:bg-gray-700 file:text-white
                  hover:file:bg-gray-600
                  focus:file:bg-gray-700
                  focus:outline-none
                  cursor-pointer"
              />
          </div>
          <div className="flex justify-end">
            <Button type="submit" className="mt-4">
              Upload
            </Button>
          </div>
        </form>
      </Modal>
    </>
  );
}
