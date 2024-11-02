import { Input } from "rizzui"
import Label from "../shared/Label"
import { DateFormatter } from "../../services/dateFormatter"
import { TagInput } from "../shared/TagInput"

export const EditDocumentModal = ({ handleInputTitleChange, isOpen, closeModal, document, availableTags, editedTitle, setEditedTitle, editedTags, setEditedTags }) => {
    return (
<>
      <div className="mb-4">
        <Label title="Title" />
        <Input
          type="text"
          value={editedTitle}
          onChange={handleInputTitleChange}
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
    </>    )
}