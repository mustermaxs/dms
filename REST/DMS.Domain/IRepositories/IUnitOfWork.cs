namespace DMS.Domain.IRepositories;

    public interface IUnitOfWork : IDisposable
    {
        public IDmsDocumentRepository DmsDocumentRepository { get; }
        public ITagRepository TagRepository { get; }
        public IDocumentTagRepository DocumentTagRepository { get; }
        Task CommitAsync();
        Task RollbackAsync();
        Task RollbackAsync(Exception ex);
        public Task BeginTransactionAsync();
    }
