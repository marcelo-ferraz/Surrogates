using Surrogates.Expressions;
using Surrogates.Tactics;

namespace Surrogates.Applications
{
    public class ShallowExtension<T> : IExtension<T>
    {
        public Strategies Strategies { get; set; }
        public ExpressionFactory<T> Factory { get; set; }
        
        public BaseContainer4Surrogacy Container { get; set; }
    }
}
