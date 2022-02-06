using Grpc.Domain.Model;
using System.Reflection;

namespace Grpc.Infrastructure.Repository.Resolver
{
    public class RepositoryResolver<T>
        : IRepositoryResolver<T> where T : EntityBase
    {
        public IRepository<T> Resolve(bool useOrm = true)
        {
            var type = Assembly.GetAssembly(typeof(RepositoryResolver<T>))
                .GetType($"CustomerRepository{(useOrm ? "Ef" : "")}");

            var instance = Activator.CreateInstance(type);

            return instance as IRepository<T>;
        }
    }
}
