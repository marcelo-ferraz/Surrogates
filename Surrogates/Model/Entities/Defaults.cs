using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Model.Entities
{
    [Flags]
    public enum Access
    {
        Basic = 0,
        Container = 1,
        StateBag = 2,
        AnyMethod = 4,
        AnyField = 8,
        AnyBaseProperty = 16,
        AnyNewProperty = 32,                
        Instance = 64        
    }
}
