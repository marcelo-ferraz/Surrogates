using Surrogates.Applications.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Surrogates.Applications.Contracts.Collections
{
    public class AssertionList4Properties : IPropValidator
    {
        internal IList<AssertionEntry4Properties> Validators { get; set; }

        internal AssertionList4Properties()
        {
            this.Validators =
                new List<AssertionEntry4Properties>();
        }
    }
}