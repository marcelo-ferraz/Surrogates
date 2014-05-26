using Surrogates.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Tests.Entities
{
    public class Dummy
    {
        private int _int_Property;
        public virtual int Int_Property 
        {
            get { return _int_Property; }
            set { _int_Property = value; } 
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
        private int _id;
        public int Id {
            get {
                Int_1_WithField(_id);
                return 0;
            }
            set { throw new Exception(); }
        }

        private void Int_1_WithField(int _id)
        {
            throw new NotImplementedException();
        }
    }


    public class BaseClass
    {
        public virtual int Nhonho { get; set; }
    }

    public class Class1: BaseClass
    {
        public override int Nhonho
        {
            get
            {
                return NewGetMethod();
            }
            set
            {
                NewSetMethod(value);
            }
        }

        private void NewSetMethod(int value)
        {
            throw new NotImplementedException();
        }

        private int NewGetMethod()
        {
            throw new NotImplementedException();
        }
    }
}

