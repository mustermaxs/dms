
export type FileInfo = {
    base64content: string,
    fileType: string
};

export async function fileToBase64(file: File): Promise<FileInfo> {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
            const content = reader.result as string;
            const fileType = file.type;
            const base64content = content.toString().replace("data:application/pdf;base64,", "");
            const fileInfo: FileInfo = {
                base64content: base64content,
                fileType: fileType
            };
            resolve(fileInfo);
        };
        reader.onerror = error => reject(error);
        reader.readAsDataURL(file);

    });
}