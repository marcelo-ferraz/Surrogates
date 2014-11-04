using System;

namespace Surrogates.Tests.Simple.Entities
{
    public class InterferenceObject
    {
        public void Void_ParameterLess()
        {
            //Don't do anything
        }

        public void Void_InstanceAndMethodName(Dummy  s_instance, string  s_name)
        {
             s_instance.Text =  s_instance.GetType().Name + "+" +  s_name;
        }

        public void Void_VariousParametersPlusIntanceAndMethodName(string text, Dummy  s_instance, DateTime date, string  s_name, Dummy.EvenMore someInstance)
        {
             s_instance.Text += ", " + 
                text + " - method: " +  s_name;
        }

        public void Void_VariousParametersWithDifferentNames(string arg0, Dummy arg1, DateTime arg2, string arg3, Dummy.EvenMore arg4)
        {
            Void_VariousParametersPlusIntanceAndMethodName(arg0, arg1, arg2, arg3, arg4);
        }

        public int Int_2_ParameterLess()
        {
            Void_ParameterLess();
            return 2;
        }

        public int Int_2_VariousParametersPlusIntanceAndMethodName(string text, Dummy s_instance, DateTime date, string s_name, Dummy.EvenMore someInstance)
        {
            Void_VariousParametersPlusIntanceAndMethodName(text,  s_instance, date,  s_name, someInstance);
            return 2;
        }

        public int Int_2_VariousParametersWithDifferentNames(string arg1, Dummy arg2, DateTime arg3, string arg4, Dummy.EvenMore arg5)
        {
            Void_VariousParametersPlusIntanceAndMethodName(arg1, arg2, arg3, arg4, arg5);
            return 2;
        }

        public int Int_2_InstanceAndMethodName(Dummy s_instance, string s_name)
        {
            Void_InstanceAndMethodName(s_instance,  s_name);
            return 2;
        }

        public int Int_1_ReturnFieldAndInstance(int field, Dummy s_instance)
        {
             s_instance.Text = "was added to the field +1";
            return field + 1;
        }

        public void Void_InstanceAndField(Dummy s_instance, int s_value)
        {
             s_instance.Text =  s_instance.GetType().Name;
        }
    }
}
