namespace DMS.Application.Interfaces;
using Domain.IRepositories;

    public interface IUnitOfWork : IDisposable
    {
        public IDmsDocumentRepository DmsDocumentRepository { get; }
        public ITagRepository TagRepository { get; }
        Task CommitAsync();
        Task RollbackAsync();
        Task RollbackAsync(Exception ex);
        public Task BeginTransactionAsync();
    }
