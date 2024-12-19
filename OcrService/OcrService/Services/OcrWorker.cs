using IronOcr;
using OcrService;

namespace WorkerService1;

public class OcrWorker
{
    private readonly IronTesseract _ironOcr;

    public OcrWorker()
    {
        _ironOcr = new IronTesseract();
    }

    public async Task<string> ProcessPdfAsync(Stream? stream)
    {
        if (!IsStreamValid(stream))
            throw new ArgumentException("Stream is not valid");
        
        stream!.Position = 0;
        using var ocrInput = new OcrInput();
        ocrInput.LoadPdf(stream);
        var result = await _ironOcr.ReadAsync(ocrInput);

        if (!IsValidText(result.Text))
            throw new ArgumentException("Text is not valid");
        Program.logger.Debug($"Extracted text.");
        return result.Text;
    }

    protected bool IsStreamValid(Stream? stream)
    {
        return stream is not null && stream.Length > 0;
    }

    protected bool IsValidText(string text)
    { 
        return !string.IsNullOrEmpty(text);
    }
}