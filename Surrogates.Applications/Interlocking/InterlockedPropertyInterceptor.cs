using Surrogates.Applications.Infrastructure;

namespace Surrogates.Applications.Interlocking
{
    public abstract class InterlockedPropertyInterceptor : Locked4RW
    {        
        protected object _field;
        
        protected abstract object GetField(object field);

        public object Get()
        {
            object ret = null;

            base.Read(
                () => 
                    ret = GetField(_field));

            return ret;
        }

        public void Set(object s_value)
        {   
            base.Write(() => 
                _field = s_value);
        }
    }
}