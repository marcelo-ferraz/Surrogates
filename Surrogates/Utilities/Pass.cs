using Surrogates.Expressions;

namespace Surrogates.Utilities
{
    public static class Pass
    {
        public static void On<T>(ApplyExpression<T> baseExp, IExtension<T> ext)
        {
            ext.Strategies = baseExp.Strategies;
            ext.Factory = baseExp.Factory;
        }
    }
}
