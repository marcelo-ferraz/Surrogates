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

        public virtual int Int_1_ParameterLess()
        {
            Void_ParameterLess();
            return 1;
        }


        public int nhonho2() {
            nhonho();
            return 0; 
        }

        public void nhonho()
        {
        }

        ReplacementObj _interceptor0;
        public virtual int Int_1_VariousParameters(string text, DateTime date, EvenMore someInstance)
        {
            Void_VariousParameters(null, new DateTime(), null);
            return 1;
        }
    }
}
