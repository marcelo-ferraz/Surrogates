using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Aspects.ExecutingElsewhere
{
    public interface IHasTasks
    {        
        Dictionary<IntPtr, Task<object>> Tasks { get; set; }
    }
}
