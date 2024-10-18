import { useState } from "react";
import { FiDownload } from "react-icons/fi";
import { ActionIcon } from "rizzui";
import { useModal } from "../../hooks/useModal";

import Document from "../../types/Document";
import Label from "./Label";

export default function DocumentTable() {
  const { Modal, isOpen, openModal, closeModal } = useModal();
  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);

  const documents = [
    {
      title: "Document 1",
      content: "Lorem ipsum",
      tags: ["Important", "Work"],
    },
    {
      title: "Document 2",
      content: "Dolor sit amet",
      tags: ["Personal", "Urgent"],
    },
    {
      title: "Document 3",
      content: "Consectetur adipiscing",
      tags: ["Archived"],
    },
    {
      title: "Document 4",
      content: "Aliquam at",
      tags: ["Work", "In Progress"],
    },
  ] as Document[];

  const showModal = (document: Document) => {
    setSelectedDocument(document);
    openModal();
  };

  const handleDownload = (documentName: string) => {
    alert(`Downloading ${documentName}`);
  };

  return (
    <div className="overflow-x-auto">
      {/* Table */}
      <table className="min-w-full text-sm text-left text-gray-500">
        <thead className="text-sm text-gray-700 bg-gray-100">
          <tr>
            <th scope="col" className="px-6 py-3">Document</th>
            <th scope="col" className="px-6 py-3">Match</th>
            <th scope="col" className="px-6 py-3">Tags</th>
            <th scope="col" className="px-6 py-3"></th>
          </tr>
        </thead>
        <tbody>
          {documents.map((doc, index) => (
            <tr
              key={index}
              className="bg-white border-b hover:bg-gray-50 cursor-pointer"
              onClick={() => showModal(doc as Document)}
            >
              <td className="px-6 py-4 font-semibold">{doc.title}</td>
              <td className="px-6 py-4 font-semibold">{doc.content}</td>
              <td className="px-6 py-4">
                {doc.tags.map((tag, idx) => (
                  <span
                    key={idx}
                    className="inline-block bg-blue-100 text-blue-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded"
                  >
                    {tag}
                  </span>
                ))}
              </td>
              <td className="px-6 py-4 text-right">
                <ActionIcon variant="outline" rounded="md" onClick={(e) => {
                  e.stopPropagation();
                  handleDownload(doc.title)
                }} >
                <FiDownload />
                </ActionIcon>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Modal */}
      {selectedDocument && (
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
      )}
    </div>
  );
}
