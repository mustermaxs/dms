import Modal from "../shared/Modal";
import Label from "../shared/Label";
import { Input, Button } from "rizzui";
import { TagInput } from "../shared/TagInput";
import { useContext, useEffect, useState } from "react";
import { FileInfo, fileToBase64 } from "../../services/fileService";
import { getEmptyGuid } from "../../services/guidGenerator";
import { Tag } from "../../types/Tag";
import AppContext from "../context/AppContext";
import { Document, DocumentStatus } from "../../types/Document";

export const UploadModal = ({ size, isOpen, closeModal }) => {

  const { availableTags, setIsLoadingTags, uploadDocument, watchDocumentStatus, unwatchDocumentStatus, addMessage, refetchSelectedDocument } = useContext(AppContext);

  const [tags, setTags] = useState<Tag[]>([]);
  const [selectedTags, setSelectedTags] = useState<Tag[]>([]);
  const [file, setFile] = useState<File | null>(null);
  const [title, setTitle] = useState("");


  const handleTagChange = (newValue: Tag[]) => {
    let tagsWithoutIds: Tag[] = newValue.filter(t => t.id === "" || t.id === undefined);

    let updatedTagsWithoutIds = tagsWithoutIds.map(t => ({
      id: getEmptyGuid(),
      label: t.label,
      color: "red",
      value: t.value,
    }));

    setTags(prevTags => [...prevTags, ...updatedTagsWithoutIds]);
    let validTags = newValue.filter(t => t.id !== "" && t.id !== undefined);
    newValue = [...validTags, ...updatedTagsWithoutIds];
    setSelectedTags(newValue);
  };



  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    let fileInfo: FileInfo = await fileToBase64(file as File);
    let response: Document = await uploadDocument({
      title: title,
      tags: selectedTags,
      content: fileInfo.base64content,
      fileType: fileInfo.fileType
    });

    if (!response) {
      addMessage("Document could not be uploaded. Please check your form!");
      return;
    }

    watchDocumentStatus(response.id, async (ev) => {
      switch (ev.data) {
        case DocumentStatus.Pending:
          unwatchDocumentStatus(response.id, ev.token);
          addMessage(`Document ${title} is being processed!`);
          break;
        case DocumentStatus.Finished:
          unwatchDocumentStatus(response.id, ev.token);
          await refetchSelectedDocument(ev.documentId);
          addMessage(`Document ${title} is ready!`);
          break;
        case DocumentStatus.NotStarted:
          addMessage(`Document ${title} is not ready yet! Still needs to be processed.`);
          break;
        case DocumentStatus.Failed:
          unwatchDocumentStatus(response.id, ev.token);
          addMessage(`Document ${title} could not be uploaded. Please check your form!`);
          break;
        default:
          addMessage("FAILURE");
          break;
      }
      closeModal();
    });

    const resetForm = () => {
      setTitle("");
      setTags([]);
      setSelectedTags([]);
      setFile(null);
    };

    closeModal();
    resetForm();
  };

  useEffect(() => {
    setIsLoadingTags(true);
  }, [isOpen]);

  return (
    <Modal size={size} isOpen={isOpen} closeModal={closeModal} title="Upload Document">
      <form onSubmit={(ev) => { handleSubmit(ev) }} className="space-y-4">
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
