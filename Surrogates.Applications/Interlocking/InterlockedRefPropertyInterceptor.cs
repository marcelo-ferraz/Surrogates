using Surrogates.Utilities;

namespace Surrogates.Applications.Interlocking
{
    public class InterlockedRefPropertyInterceptor<T> 
        : InterlockedPropertyInterceptor<T>
        where T : class
    {
        protected override T GetField(T field)
        {
            return Clone.This(field);
        }
    }
}
