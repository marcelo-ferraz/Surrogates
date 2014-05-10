using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Tests
{
    class Dummy
    {
        internal class EvenMore
        {
            public string SomeText { get; set; }
        }

        public string Text { get; set; }

        internal void VoidParameterless()
        {
            Text = "simple";
        }

        internal void ComplexVoid(string text, DateTime date, EvenMore someInstance)
        {
            Text = "more complex";
        }
    }
}
