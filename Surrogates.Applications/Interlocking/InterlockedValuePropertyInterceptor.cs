
namespace Surrogates.Applications.Interlocking
{
    public class InterlockedValuePropertyInterceptor<T>
        : InterlockedPropertyInterceptor<T>
    {
        protected override T GetField(T field)
        {
            return field;
        }
    }
}