import { useContext, useEffect, useState } from "react";
import { FiDownload } from "react-icons/fi";
import { ActionIcon } from "rizzui";
import { useModal } from "../../hooks/useModal";
import { Document, DocumentContentDto, DocumentStatus } from "../../types/Document";
import { DocumentModal } from "./DocumentModal";
import { MockDocumentService as IDocumentService } from "../../services/documentService";
import { ServiceLocator } from "../../serviceLocator";
import "./DocumentTable.css";
import AppContext from "../context/AppContext";

export default function DocumentTable() {
  const { isOpen, openModal, closeModal } = useModal();
  // const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);

  const { documents, selectedDocument, setSelectedDocument, addMessage, isLoading } = useContext(AppContext);

  const showModal = async (document: Document) => {
    let documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
    let doc: Document = await documentService.getDocument(document.id);

    if (doc.status < DocumentStatus.Finished) {
      addMessage("Document is still being processed...");
    }
    setSelectedDocument(doc);
    openModal();
  };

  const renderStatusBadge = (status: DocumentStatus) => {
    switch (status) {
      case DocumentStatus.Finished:
        return <span className="inline-block bg-green-100 text-green-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">Processed</span>;
      case DocumentStatus.Pending || DocumentStatus.NotStarted:
        return <span className="inline-block bg-yellow-100 text-yellow-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">Pending</span>;
      case DocumentStatus.Failed:
        return <span className="inline-block bg-red-100 text-red-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">Failed</span>;
      default:
        return <span className="inline-block bg-gray-100 text-gray-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">{status}</span>;
    }
  };

  const handleDownload = (documentName: string) => {
    alert(`Downloading ${documentName}`);
  };

  return (
    <>
      {isLoading ? (
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-8 text-center">
          <p className="text-gray-500 animate-pulse">Loading documents...</p>
        </div>
      ) : documents && documents.length > 0 ? (
        <div className="bg-white rounded-lg shadow-sm border border-gray-200">
          <div className="overflow-x-auto">
            <table className="dms-table min-w-full divide-y divide-gray-200">
              <thead>
                <tr>
                  <th scope="col" className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider border-b">Document</th>
                  <th scope="col" className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider border-b">Tags</th>
                  <th scope="col" className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider border-b">Status</th>
                  <th scope="col" className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider border-b">Date</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {documents.map((doc, index) => (
                  <tr
                    key={doc.id}
                    className="hover:bg-gray-50 transition-colors cursor-pointer"
                    onClick={async () => await showModal(doc as Document)}
                  >
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-700">{doc.title}</td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {doc.tags.map((tag, idx) => (
                        <span
                          key={idx}
                          className="inline-block bg-blue-50 text-blue-700 text-xs font-medium mr-2 px-2.5 py-0.5 rounded-full"
                        >
                          {tag.label}
                        </span>
                      ))}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {renderStatusBadge(doc.status)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-normal text-gray-700">
                      {new Date(doc.uploadDateTime).toLocaleString(
                        'de-DE',
                        {
                          year: 'numeric',
                          month: '2-digit',
                          day: '2-digit',
                          hour: '2-digit',
                          minute: '2-digit',
                          hour12: false
                        }
                      ).replace(',', '')}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-8 text-center">
          <p className="text-gray-500">No documents found</p>
        </div>
      )}
      {selectedDocument && <DocumentModal isOpen={isOpen} closeModal={closeModal} />}
    </>
  );
}
