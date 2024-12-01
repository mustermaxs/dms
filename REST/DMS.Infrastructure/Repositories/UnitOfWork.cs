using System.Collections.ObjectModel;
using DMS.Application;
using DMS.Application.Interfaces;
using DMS.Domain;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Documents;
using DMS.Domain.Entities.Tags;
using DMS.Domain.IRepositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DMS.Infrastructure.Repositories;
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DmsDbContext _context;
        private readonly IMediator _mediator;
        private readonly IValidator<Tag> _tagValidator;
        private readonly IValidator<DmsDocument> _documentValidator;

        private readonly IServiceProvider _serviceProvider;

        private IDmsDocumentRepository? _documentRepository;
        private ITagRepository? _tagRepository;
        private IDbContextTransaction? _transaction = null;

        public UnitOfWork(
            DmsDbContext context,
            IMediator mediator,
            IValidator<Tag> tagValidator,
            IValidator<DmsDocument> documentValidator,
            IServiceProvider serviceProvider
            )
        {
            _context = context;
            _mediator = mediator;
                _tagValidator = tagValidator;
            _documentValidator = documentValidator;
            _serviceProvider = serviceProvider;
            // _documentTagValidator = documentTagValidator;
            _documentRepository = serviceProvider.GetRequiredService<IDmsDocumentRepository>();
            _tagRepository = serviceProvider.GetRequiredService<ITagRepository>();
        }
        public IDmsDocumentRepository DmsDocumentRepository => _documentRepository;
        public ITagRepository TagRepository => _tagRepository;

        private IReadOnlyCollection<object> GetDomainEventsFromEntities()
        {
            // var entitiesWithEvents = _context.ChangeTracker.Entries<Entity>().ToList();
            // var domainEvents = entitiesWithEvents.SelectMany(e => e.Entity.DomainEvents).ToList();
            // return new ReadOnlyCollection<object>(domainEvents);
            return Entity.DomainEvents.Cast<IDomainEvent>().ToList();
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
                await _transaction.CommitAsync();
                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent);
                }
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