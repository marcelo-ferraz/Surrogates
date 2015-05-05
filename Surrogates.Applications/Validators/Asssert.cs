using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Surrogates.Applications.Validators
{
    public class Assert
    {
        public class Entry4
        {
            public class Parameters
            {
                public string ParameterName { get; set; }
                public Func<int, ParameterInfo, Action<object[]>> Action { get; set; }
            }

            public class Properties
            {
                public Delegate Property { get; set; }
                public Delegate Validation { get; set; }
            }
        }

        public class List4
        {
            public class Parameters : IParamValidators
            {
                internal IList<Entry4.Parameters> Validators { get; set; }

                internal Parameters()
                {
                    this.Validators =
                        new List<Entry4.Parameters>();
                }
            }

            public class Properties : IPropValidators
            {
                internal IList<Entry4.Properties> Validators { get; set; }

                internal Properties()
                {
                    this.Validators =
                        new List<Entry4.Properties>();
                }
            }
        }
    }
}
