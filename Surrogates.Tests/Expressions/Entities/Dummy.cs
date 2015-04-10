using System;

namespace Surrogates.Tests.Expressions.Entities
{
    public class Dummy
    {
        public Dummy()
        {

        }

        public Dummy(string txt)
        {
            Text = txt;
        }

        public class EvenMore
        {
            public string SomeText { get; set; }
        }

        private int _int_Property;
        public virtual int Int_Property
        {
            get { return _int_Property; }
            set 
            {
                _int_Property = NotImplementedMethod(value); 
            }
        }

        private int NotImplementedMethod(int value)
        {
            throw new NotImplementedException();
        }

        public virtual int AccessItWillThrowException
        {
            get { throw new Exception("get"); }
            set { throw new Exception("set"); }
        }

        public string Text { get; set; }

        public virtual void SetPropText_simple()
        {
            Text = "simple";
        }

        public virtual void SetPropText_complex(string text, DateTime date, EvenMore someInstance)
        {
            Text = "complex";
        }

        public virtual int GetTheNumberTwo()
        {
            return 2;
        }

        public virtual double CompareFirstToSecond(int first, int second)
        {
            return first.CompareTo(second);
        }

        public virtual int Call_SetPropText_simple_Return_1()
        {
            SetPropText_simple();
            return 1;
        }

        public virtual int Call_SetPropText_complex_Return_1(string text, DateTime date, EvenMore someInstance)
        {
            SetPropText_complex(text, date, someInstance);
            return 1;
        }
    }
}