using FluentValidation;

namespace DMS.Domain.ValueObjects;

public class FileTypeValidator : AbstractValidator<FileType>
{
    private List<string> _validFileTypes = new List<string> { "pdf", "docx", "txt" };
    public FileTypeValidator()
    {
        RuleFor(e => e.Name).NotNull();
        RuleFor(e => e.Name).Must(e => _validFileTypes.Contains(e)).WithMessage("Invalid file type");
    }
}