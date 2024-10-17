using DMS.Domain;
using DMS.Domain.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace DMS.Infrastructure.Repositories;



    public class UnitOfWork : IUnitOfWork
    {
        private readonly DmsDbContext _context;
        private IDomainEventDispatcher _eventDispatcher;
        private IDmsDocumentRepository? _documentRepository;
        private ITagRepository? _tagRepository;
        private IDocumentTagRepository? _documentTagRepository;
        private IDbContextTransaction? _transaction = null;

        public UnitOfWork(DmsDbContext context, IDomainEventDispatcher eventDispatcher)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
        }
        public IDmsDocumentRepository DmsDocumentRepository => _documentRepository ??= new DmsDocumentRepository(_context, _eventDispatcher);
        public ITagRepository TagRepository => _tagRepository ??= new TagRepository(_context, _eventDispatcher);
        public IDocumentTagRepository DocumentTagRepository => _documentTagRepository ??= new DocumentTagRepository(_context, _eventDispatcher);

        public async Task BeginTransactionAsync()
        {
            _transaction ??= await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch (Exception)
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            await _context.DisposeAsync();
        }
        
        public async Task RollbackAsync(Exception ex)
        {
            await _context.DisposeAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }