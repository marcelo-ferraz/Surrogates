using System;

namespace Surrogates
{
    public class IncoherentPropertyOnBodyException : Exception
    {
        public IncoherentPropertyOnBodyException()
            : base("There was a problem whilst inspecting the body of the property you provided.")
        { }
    }
}
