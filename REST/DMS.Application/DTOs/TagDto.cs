namespace DMS.Application.DTOs;
public class TagDto
{
    public Guid? Id { get; set; } = Guid.Empty;
    public string Label { get; set; }
    public string Color { get; set; }
    public string Value { get; set; }
}