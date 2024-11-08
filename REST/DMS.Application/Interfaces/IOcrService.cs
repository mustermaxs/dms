using DMS.Application.DTOs;
using DMS.Domain.Entities;

namespace DMS.Application.Interfaces;

public interface IOcrService
{
    Task<string> ProcessDocumentAsync(DmsDocumentDto document);
}