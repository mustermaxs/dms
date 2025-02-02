using System.Collections.ObjectModel;
using DMS.Application;
using DMS.Application.Interfaces;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace DMS.Infrastructure.Repositories;
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DmsDbContext _context;
        private readonly IMediator _mediator;
        private readonly IValidator<Tag> _tagValidator;
        private readonly IValidator<DmsDocument> _documentValidator;
        private readonly IValidator<DocumentTag> _documentTagValidator;
        private IDmsDocumentRepository? _documentRepository;
        private ITagRepository? _tagRepository;
        private IDocumentTagRepository? _documentTagRepository;
        private IDbContextTransaction? _transaction = null;

        public UnitOfWork(
            DmsDbContext context,
            IMediator mediator,
            IValidator<Tag> tagValidator,
            IValidator<DmsDocument> documentValidator,
            IValidator<DocumentTag> documentTagValidator)
        {
            _context = context;
            _mediator = mediator;
                _tagValidator = tagValidator;
            _documentValidator = documentValidator;
            _documentTagValidator = documentTagValidator;
        }
        public IDmsDocumentRepository DmsDocumentRepository => _documentRepository ??= new DmsDocumentRepository(_context, _documentValidator);
        public ITagRepository TagRepository => _tagRepository ??= new TagRepository(_context, _tagValidator);
        public IDocumentTagRepository DocumentTagRepository => _documentTagRepository ??= new DocumentTagRepository(_context, _documentTagValidator);

        private IReadOnlyCollection<object> GetDomainEventsFromEntities()
        {
            var entitiesWithEvents = _context.ChangeTracker.Entries<Entity>().ToList();
            var domainEvents = entitiesWithEvents.SelectMany(e => e.Entity.DomainEvents).ToList();
            return new ReadOnlyCollection<object>(domainEvents);
        }

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
                var domainEvents = GetDomainEventsFromEntities();
                
                await _context.SaveChangesAsync();
                await Task.WhenAll(domainEvents
                    .Select(domainEvent => 
                        _mediator.Publish(domainEvent))
                    .ToList());
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