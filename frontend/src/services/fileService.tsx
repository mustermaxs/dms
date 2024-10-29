export async function fileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
            const content = reader.result as string;
            const base64content = content.toString().replace("data:application/pdf;base64,", "");
            resolve(base64content);
        };
        reader.onerror = error => reject(error);
        reader.readAsDataURL(file);

    });
}