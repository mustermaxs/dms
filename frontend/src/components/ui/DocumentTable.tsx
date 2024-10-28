import { useContext, useEffect, useState } from "react";
import { FiDownload } from "react-icons/fi";
import { ActionIcon } from "rizzui";
import { useModal } from "../../hooks/useModal";
import { Document } from "../../types/Document";
import { DocumentModal } from "./DocumentModal";
import { MockDocumentService as IDocumentService } from "../../services/documentService";
import { ServiceLocator } from "../../serviceLocator";
import "./DocumentTable.css";
import AppContext from "../context/AppContext";

export default function DocumentTable() {
  const { isOpen, openModal, closeModal } = useModal();
  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);
  const { documents } = useContext(AppContext);


  const showModal = async (document: Document) => {
    let documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    let doc: Document = await documentService.getDocument(document.id);
    setSelectedDocument(doc);
    openModal();
  };

  const handleDownload = (documentName: string) => {
    alert(`Downloading ${documentName}`);
  };

  return (
    <>
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
                onClick={async () => await showModal(doc as Document)}
              >
                <td
                  style={{ paddingRight: '5rem', textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}
                  className="dms-doctitle-col px-6 py-4 font-semibold" title={doc.title}>{doc.title}</td>
                <td style={{
                  verticalAlign: 'center',
                  maxHeight: '3.5rem',
                  maxWidth: '20rem',
                  textOverflow: 'ellipsis',
                  WebkitBoxOrient: 'vertical',
                  WebkitLineClamp: '2',
                  overflow: 'hidden',
                  display: '-webkit-box'
                }} className="px-6 py-4 font-semibold">{doc.content}</td>
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

        {selectedDocument && <DocumentModal isOpen={isOpen} closeModal={closeModal} selectedDocument={{ selectedDocument }} />}

      </div>
    </>
  );
}
