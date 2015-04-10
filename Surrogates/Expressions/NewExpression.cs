
using Surrogates.Tactics;
using System.Reflection.Emit;

namespace Surrogates.Expressions
{
    public class NewExpression
    {
        private ModuleBuilder _moduleBuilder;
        private BaseContainer4Surrogacy _container;
        public Strategies Strategies { get; set; }
        public string Name { get; set; }

        public NewExpression(ModuleBuilder moduleBuilder, BaseContainer4Surrogacy container)
        {
            this._moduleBuilder = moduleBuilder;
            this._container = container;
        }

        public ExpressionFactory<T> From<T>(string name = "")
        {
            Name = name;

            Strategies = new Strategies(
                typeof(T), name, _moduleBuilder);
            
            return new ExpressionFactory<T>(
                _container,
                new Strategy(Strategies), 
                Strategies);
        }
    }
}