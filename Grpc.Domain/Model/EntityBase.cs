using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Domain.Model
{
    public abstract class EntityBase
    {
        public virtual object Id { get; set; }
    }
}
