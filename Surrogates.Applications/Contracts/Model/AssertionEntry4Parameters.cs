using System;
using System.Reflection;

namespace Surrogates.Applications.Contracts.Model
{
    public class AssertionEntry4Parameters
    {
        public string ParameterName { get; set; }
        public Func<int, ParameterInfo[], Action<object[]>> Action { get; set; }
    }
}