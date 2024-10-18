import Modal from "../shared/Modal";
import Label from "../shared/Label";

export const DocumentModal = ({ isOpen, closeModal, selectedDocument }) => {
    return (
        <Modal isOpen={isOpen} closeModal={closeModal} title={`${selectedDocument.title} Details`}>
          <Label title="Tags" />
          <ul className="mb-4">
            {selectedDocument.tags.map((tag, index) => (
              <li key={index} className="inline-block bg-blue-100 text-blue-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">
                {tag}
              </li>
            ))}
          </ul>
          <Label title="Match" />
          <p className="my-2">{selectedDocument.content}</p>
        </Modal>
    )
}
