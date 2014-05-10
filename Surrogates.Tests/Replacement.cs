using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Tests
{
    class Replacement
    {
        internal void VoidParameterless()
        {
        }

        internal void VoidAsking4Instance(Dummy instance)
        {
            instance.Text = instance.GetType().Name;
        }

        internal void VoidThrowExceptionWithOriginalMethodName(string methodName)
        {
            throw new Exception(methodName);
        }

        internal void ComplexVoid(string text, Dummy instance, DateTime date, string methodName, Dummy.EvenMore someInstance)
        {
            instance.Text =
                text + '-' + methodName;
        }

        internal void ComplexVoidWithDifferentParameterNames(string arg0, Dummy instance, DateTime arg2, string arg3, Dummy.EvenMore arg4)
        {
            instance.Text =
                arg0 + '-' + arg3;
        }
    }
}
