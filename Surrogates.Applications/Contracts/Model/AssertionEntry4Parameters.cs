using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Surrogates.Applications.Contracts.Model
{
    public class AssertionEntry4Parameters
    {
        public string ParameterName { get; set; }
        public Func<int, ParameterInfo[], Action<object[]>> Action { get; set; }
    }
}