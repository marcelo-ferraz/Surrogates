using Surrogates.Utilities;

namespace Surrogates.Aspects.Interlocking
{
    public class InterlockedRefPropertyInterceptor 
        : InterlockedPropertyInterceptor
    {
        protected override object GetField(object field)
        {
            return field;//Utilities.WhizzoDev.CloneHelper.Clone(srcField);
        }
    }
}
