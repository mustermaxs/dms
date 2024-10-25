import Modal from "../shared/Modal";
import Label from "../shared/Label";

export const DocumentModal = ({ isOpen, closeModal, selectedDocument }) => {
  const document = selectedDocument.selectedDocument;
  console.log(document);
    return (
        <Modal isOpen={isOpen} closeModal={closeModal} title={`${document.title}`}>
          <Label title="Tags" />
          <ul className="mb-4">
            {document.tags.map((tag, index) => (
              <li key={index} className="inline-block bg-blue-100 text-blue-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">
                {tag.label}
              </li>
            ))}
          </ul>
          <Label title="Match" />
          <p className="my-2">{document.content}</p>
        </Modal>
    )
}
