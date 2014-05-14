using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Tests.Entities
{
    public class Dummy
    {
        public class EvenMore
        {
            public string SomeText { get; set; }
        }

        public string Text { get; set; }

        public virtual void Void_ParameterLess()
        {
            Text += "simple";
        }

        public virtual void Void_VariousParameters(string text, DateTime date, EvenMore someInstance)
        {
            Text = "complex";
        }

        public virtual int IntParameterless()
        {
            return 2;
        }

        public virtual double DoubleSimpleParameters(int first, int second)
        {
            return first.CompareTo(second);
        }

        public int nhonho2() { return 2; }

        public void nhonho()
        {
            nhonho2();
        }
    }
}
