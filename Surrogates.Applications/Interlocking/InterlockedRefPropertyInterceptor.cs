using Surrogates.Utilities;

namespace Surrogates.Applications.Interlocking
{
    public class InterlockedRefPropertyInterceptor<T> 
        : InterlockedPropertyInterceptor<T>
    {
        protected override T GetField(T field)
        {
            return Clone.This(field);
        }
    }
}
