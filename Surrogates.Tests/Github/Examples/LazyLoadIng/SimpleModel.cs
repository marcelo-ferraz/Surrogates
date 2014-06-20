using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Github.Examples.LazyLoadIng
{
    public class SimpleModel
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int OutterId { get; set; }
    }
}
