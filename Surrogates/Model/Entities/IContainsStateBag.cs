using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Surrogates.Model.Entities
{
    public interface IContainsStateBag
    {
        DynamicObj StateBag { get; }
    }
}
