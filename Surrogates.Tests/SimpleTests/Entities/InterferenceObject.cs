using System;

namespace Surrogates.Tests.Simple.Entities
{
    public class InterferenceObject
    {
        public void AccomplishNothing()
        {
            //Don't do anything
        }

        public void SetPropText_InstanceAndMethodName(Dummy  s_instance, string  s_name)
        {
             s_instance.Text =  s_instance.GetType().Name + "+" +  s_name;
        }

        public void AddToPropText__MethodName(string text, Dummy  s_instance, DateTime date, string  s_name, Dummy.EvenMore someInstance)
        {
             s_instance.Text += ", " + 
                text + " - property: " +  s_name;
        }

        public void Void_VariousParametersWithDifferentNames(string arg0, Dummy arg1, DateTime arg2, string arg3, Dummy.EvenMore arg4)
        {
            AddToPropText__MethodName(arg0, arg1, arg2, arg3, arg4);
        }

        public int AccomplishNothing_Return2()
        {
            AccomplishNothing();
            return 2;
        }

        public int AddToPropText__MethodName_Return2(string text, Dummy s_instance, DateTime date, string s_name, Dummy.EvenMore someInstance)
        {
            AddToPropText__MethodName(text,  s_instance, date,  s_name, someInstance);
            return 2;
        }

        public int DontAddToPropText__MethodName_Return2(string arg1, Dummy arg2, DateTime arg3, string arg4, Dummy.EvenMore arg5)
        {
            AddToPropText__MethodName(arg1, arg2, arg3, arg4, arg5);
            return 2;
        }

        public int SetPropText_InstanceAndMethodName_Return2(Dummy s_instance, string s_name)
        {
            SetPropText_InstanceAndMethodName(s_instance,  s_name);
            return 2;
        }

        public int SetPropText_info_Return_FieldPlus1(int field, Dummy s_instance)
        {
             s_instance.Text = "was added to the field +1";
            return field + 1;
        }

        public void SetPropText_TypeName(Dummy s_instance, int s_value)
        {
             s_instance.Text =  s_instance.GetType().Name;
        }
    }
}
