using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Tests.Entities
{
    public class ReplacementObj
    {
        public void Void_ParameterLess()
        {
            //Don't do anything
        }

        public void Void_InstanceAndMethodName(Dummy instance, string methodName)
        {
            instance.Text = instance.GetType().Name + "+" + methodName;
        }

        public void Void_VariousParametersPlusIntanceAndMethodName(string text, Dummy instance, DateTime date, string methodName, Dummy.EvenMore someInstance)
        {
            instance.Text += ", " + 
                text + " - method: " + methodName;
        }

        public void Void_VariousParametersWithDifferentNames(string arg0, Dummy arg1, DateTime arg2, string arg3, Dummy.EvenMore arg4)
        {
            Void_VariousParametersPlusIntanceAndMethodName(arg0, arg1, arg2, arg3, arg4);
        }

        public int IntAsking4InstanceAndMethodName(Dummy instance, string methodName)
        {
            instance.Text = methodName;
            return 4;
        }
        public double DoubleWithOriginalParameters(int first, int second)
        {
            return first + second;
        }

        public int Int_2_ParameterLess()
        {
            return 2;
        }

        public int Int_2_VariousParametersPlusIntanceAndMethodName(string text, Dummy instance, DateTime date, string methodName, Dummy.EvenMore someInstance)
        {
            Void_VariousParametersPlusIntanceAndMethodName(text, instance, date, methodName, someInstance);
            return 2;
        }

        public int Int_2_VariousParametersWithDifferentNames(string arg1, Dummy arg2, DateTime arg3, string arg4, Dummy.EvenMore arg5)
        {
            Void_VariousParametersPlusIntanceAndMethodName(arg1, arg2, arg3, arg4, arg5);
            return 2;
        }

        public int Int_2_InstanceAndMethodName(Dummy instance, string methodName)
        {
            Void_InstanceAndMethodName(instance, methodName);
            return 2;
        }
    }
}
