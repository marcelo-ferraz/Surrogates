using Surrogates.Tests.Simple.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Tests.Simple.Entities
{
    public class Dummy
    {
        private int _int_Property;
        public virtual int Int_Property
        {
            get { return _int_Property; }
            set 
            {
                _int_Property = Nhonho(value); 
            }
        }

        private int Nhonho(int value)
        {
            throw new NotImplementedException();
        }

        public virtual int NewExpectedException
        {
            get { throw new Exception("get"); }
            set { throw new Exception("set"); }
        }

        public class EvenMore
        {
            public string SomeText { get; set; }
        }

        public string Text { get; set; }

        public virtual void Void_ParameterLess()
        {
            Text = "simple";
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

        public virtual int Int_1_VariousParameters(string text, DateTime date, EvenMore someInstance)
        {
            Void_VariousParameters(text, date, someInstance);
            return 1;
        }
    }
}