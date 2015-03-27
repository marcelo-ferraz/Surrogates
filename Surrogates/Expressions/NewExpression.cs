
using Surrogates.Tactics;
using System.Reflection.Emit;

namespace Surrogates.Expressions
{
    public class NewExpression
    {
        private ModuleBuilder _moduleBuilder;

        public Strategies Strategies { get; set; }
        public string Name { get; set; }

        public NewExpression(ModuleBuilder moduleBuilder)
        {
            this._moduleBuilder = moduleBuilder;
        }

        public ExpressionFactory<T> From<T>(string name = "")
        {
            Name = name;

            Strategies = new Strategies(
                typeof(T), name, _moduleBuilder);
            
            return new ExpressionFactory<T>(
                new Strategy(Strategies), Strategies);
        }
    }
}