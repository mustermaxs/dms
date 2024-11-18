using IronOcr;

namespace WorkerService1;

public class OcrWorker
{
    private readonly IronTesseract _ironOcr;

    public OcrWorker()
    {
        _ironOcr = new IronTesseract();
    }

    public async Task<string> ProcessPdfAsync(Stream stream)
    {
        stream.Position = 0;
        using var ocrInput = new OcrInput();
        ocrInput.LoadPdf(stream);
        var result = await _ironOcr.ReadAsync(ocrInput);
        return result.Text;
    }
    
}