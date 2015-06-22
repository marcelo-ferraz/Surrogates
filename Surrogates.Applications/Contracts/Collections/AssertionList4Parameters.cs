using Surrogates.Applications.Contracts.Model;
using System.Collections.Generic;

namespace Surrogates.Applications.Contracts.Collections
{
    public class AssertionList4Parameters : IParamValidator
    {
        internal IList<AssertionEntry4Parameters> Validators { get; set; }

        internal AssertionList4Parameters()
        {
            this.Validators =
                new List<AssertionEntry4Parameters>();
        }
    }
}