using Surrogates.Utilities;

namespace Surrogates.Aspects.Interlocking
{
    public class InterlockedRefPropertyInterceptor 
        : InterlockedPropertyInterceptor
    {
        protected override object GetField(object field)
        {
            return field;//Utilities.Parser.CloneHelper.Clone(srcField);
        }
    }
}
