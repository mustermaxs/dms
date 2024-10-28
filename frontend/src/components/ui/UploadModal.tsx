import Modal from "../shared/Modal";
import Label from "../shared/Label";
import { Input, Button } from "rizzui";
import { TagInput } from "../shared/TagInput";
import { useContext, useState } from "react";
import TagContext from "../context/AppContext";

export const UploadModal = ({ size, isOpen, closeModal, handleSubmit, title, setTitle, tags, handleTagChange, selectedTags, file, setFile }) => {

  const { availableTags } = useContext(TagContext);


  return (
    <Modal size={size} isOpen={isOpen} closeModal={closeModal} title="Upload Document">
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
          <TagInput
            selectedTags={selectedTags}
            onChange={handleTagChange}
            availableTags={availableTags}
            className="mt-1"
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
  )
}
