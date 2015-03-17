using Surrogates.Utilities;

namespace Surrogates.Appliances
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
