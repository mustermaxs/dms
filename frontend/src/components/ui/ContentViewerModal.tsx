import { useEffect, useState } from "react";
import { useModal } from "../../hooks/useModal";
import Modal, { ModalSize } from "../shared/Modal";
import "./ContentViewerModal.css";
import { ServiceLocator } from "../../serviceLocator";
import { IDocumentService } from "../../services/documentService";
import { useDocuments } from "../../hooks/useDocuments";
import { DocumentContentDto, DocumentStatus } from "../../types/Document";
import { SkeletonText } from "./SkeletonText";
import MsgModal from "./MsgModal";
import { MsgModalContainer, useMsgModal } from "./MsgModalContainer";

export const ContentViewerModal = ({ isOpen, closeModal, openModal, document }) => {
    const [documentBase64Content, setDocumentBase64Content] = useState<string | null>(null);
    const [documentContentDto, setDocumentContentDto] = useState<DocumentContentDto | null>(null);
    const { getDocumentContent } = useDocuments();
    
    // const [document, setDocument] = useState<Document>();

    const createIframeSrcAttribute = (content: string) => {
        return `data:application/pdf;base64,${content}`;
    };

    const setBase64Content = async (id: string) => {
        const documentService = ServiceLocator.resolve<IDocumentService>('IDocumentService');
        const content = await documentService.getDocumentContent(id);
        // setDocumentBase64Content(createIframeSrcAttribute(content));
    };

    const fetchDocumentContent = async (id: string) => {
        let docContent = await getDocumentContent(id);
        setDocumentContentDto(docContent);
    };

    useEffect(() => {
        if (isOpen) {
            fetchDocumentContent(document.id);
        }
    }, [isOpen]);



    return (
        <>
      {/* <MsgModal isVisible={document.status < DocumentStatus.Finished} setIsVisible={setDocumentContentDto} title="" message="Document is being processed..." handleClick={undefined} type="normal" /> */}
            <Modal size={ModalSize.LARGE} isOpen={isOpen} closeModal={closeModal} title={document.title}>
                {
                (documentContentDto && document.status === DocumentStatus.Finished) ?
                    <div className="pdf-viewer-container">
                        <p style={{ width: '100%', height: '100%' }}>
                            {documentContentDto.content}
                        </p>
                        {/* <iframe
                        style={{ width: '100%', height: '100%' }}
                        title="pdf-viewer"
                        id="pdf-viewer"
                        src={documentBase64Content}
                        ></iframe> */}
                    </div>
                    : <SkeletonText />
                    }
            </Modal>
        </>
    )
}
