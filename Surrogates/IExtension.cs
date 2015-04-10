using Surrogates.Expressions;
using Surrogates.Tactics;

namespace Surrogates
{
    public interface IExtension<TBase>
    {
        Strategies Strategies { get; set; }

        ExpressionFactory<TBase> Factory { get; set; }

        BaseContainer4Surrogacy Container { get; set; }
    }
}
