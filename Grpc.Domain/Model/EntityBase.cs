namespace Grpc.Domain.Model
{
    public abstract class EntityBase
    {
        public abstract string Namespace { get; }
        public virtual object Id { get; set; }
    }
}
