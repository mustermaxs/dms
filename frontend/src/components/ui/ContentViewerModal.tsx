import { useEffect, useState } from "react";
import { useModal } from "../../hooks/useModal";
import Modal, { ModalSize } from "../shared/Modal";
import "./ContentViewerModal.css";
import { ServiceLocator } from "../../serviceLocator";
import { IDocumentService } from "../../services/documentService";
import { Document } from "../../types/Document";

export const ContentViewerModal = ({isOpen, closeModal, openModal, document}) => {
    const [isDocumentContentLoaded, setIsDocumentContentLoaded] = useState(false);
    const [documentBase64Content, setDocumentBase64Content] = useState<string | null>(null);
    // const [document, setDocument] = useState<Document>();

    const createIframeSrcAttribute = (content: string) => {
        return `data:application/pdf;base64,${content}`;
    };

    const setBase64Content = async (id: string) => {
        const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
        const content = await documentService.getDocumentContent(id);
        setIsDocumentContentLoaded(true);
        setDocumentBase64Content(createIframeSrcAttribute(content));
    };

    useEffect(() => {
        if (isOpen) {
            console.log(document);
        }
    }, [isOpen]);

    return (
        <>
            <Modal size={ModalSize.LARGE} isOpen={isOpen} closeModal={closeModal} title={document.title}>
                <div className="pdf-viewer-container">
                    <p style={{ width: '100%', height: '100%' }}>
                        {document.content}
                    </p>
                    {/* <iframe
                        style={{ width: '100%', height: '100%' }}
                        title="pdf-viewer"
                        id="pdf-viewer"
                        src={documentBase64Content}
                        ></iframe> */}
                </div>
            </Modal>
            </>
    )
}
