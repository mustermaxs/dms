using MediatR;

namespace DMS.Application.Commands
{
    public interface ICommand : IRequest {}
    
    // public interface ICommandHandler : IRequestHandler<IRequest2, object> {}
    // public interface IRequestHandler<in TCommand, TResponse> where TResponse : class
    // {
    //     Task<TResponse> Handle(TCommand command);
    // }
}