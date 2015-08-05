using Surrogates.Expressions;
using Surrogates.Tactics;

namespace Surrogates.Aspects.Model
{
    public class ShallowExtension<T> : IExtension<T>
    {
        /// <summary>
        /// The general strategy which will be used to create a new type
        /// </summary>
        public Strategies Strategies { get; set; }

        /// <summary>
        /// The expression responsible to create the tree of expressions
        /// </summary>
        public ExpressionFactory<T> Factory { get; set; }
        
        /// <summary>
        /// The containing container
        /// </summary>
        public BaseContainer4Surrogacy Container { get; set; }
    }
}
