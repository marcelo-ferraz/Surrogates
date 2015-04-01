using System;

namespace Surrogates
{
    public class IncoherentBody4PropertyException : Exception
    {
        public IncoherentBody4PropertyException()
            : base("There was a problem whilst inspecting the body of the property you provided.")
        { }
    }
}
