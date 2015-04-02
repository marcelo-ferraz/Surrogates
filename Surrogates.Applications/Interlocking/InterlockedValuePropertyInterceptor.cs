
namespace Surrogates.Applications.Interlocking
{
    public class InterlockedValuePropertyInterceptor<T>
        : InterlockedPropertyInterceptor<T>
        where T : struct
    {
        protected override T GetField(T field)
        {
            return field;
        }
    }
}