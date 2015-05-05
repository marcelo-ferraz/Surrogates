using Surrogates.Utilities;

namespace Surrogates.Applications.Interlocking
{
    public class InterlockedRefPropertyInterceptor 
        : InterlockedPropertyInterceptor
    {
        protected override object GetField(object field)
        {
            return Clone.This(field);
        }
    }
}
