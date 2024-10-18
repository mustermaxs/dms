import Modal from "../shared/Modal";
import Label from "../shared/Label";
import { Input, Button } from "rizzui";
import CreatableSelect from "react-select/creatable";

export const UploadModal = ({ isOpen, closeModal, handleSubmit, title, tags, setTitle, handleTagChange, file, setFile }) => {

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
    <Modal isOpen={isOpen} closeModal={closeModal} title="Upload Document">
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
  )
}