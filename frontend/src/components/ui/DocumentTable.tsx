import { useEffect, useState } from "react";
import { FiDownload } from "react-icons/fi";
import { ActionIcon } from "rizzui";
import { useModal } from "../../hooks/useModal";
import Document from "../../types/Document";
import { DocumentModal } from "./DocumentModal";
import { IDocumentService } from "../../services/documentService";
import { ServiceLocator } from "../../serviceLocator";
import "./DocumentTable.css";

export default function DocumentTable() {
  const { isOpen, openModal, closeModal } = useModal();
  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);
  const [documents, setDocuments] = useState<Document[]>([]);

  useEffect(() => {
    let documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    documentService.getAllDocuments().then(documents => {
      setDocuments(documents);
    });
  }, []);

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
      <table className="dms-table min-w-full text-sm text-left text-gray-500">
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
              <td
               style={{paddingRight: '5rem',  textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden'}}
               className="dms-doctitle-col px-6 py-4 font-semibold" title={doc.title}>{doc.title}</td>
              <td className="px-6 py-4 font-semibold">{doc.content}</td>
              <td className="px-6 py-4" title={doc.tags.map((tag) => `#${tag.label}`).join(' ')}>
                {doc.tags.map((tag, idx) => (
                  <span
                    key={idx}
                    className="inline-block bg-blue-100 text-blue-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded"
                  >
                    {tag.label}
                  </span>
                ))}
              </td>
              <td className="px-6 py-4 text-right" title="Download">
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
      {selectedDocument && <DocumentModal isOpen={isOpen} closeModal={closeModal} selectedDocument={{selectedDocument}} />}
      
    </div>
  );
}
