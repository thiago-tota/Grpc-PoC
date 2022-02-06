using Grpc.Domain.Model;

namespace Grpc.Infrastructure.Repository.Resolver
{
    public interface IRepositoryResolver<T> where T : EntityBase
    {
        IRepository<T> Resolve(bool useOrm = true);
    }
}
