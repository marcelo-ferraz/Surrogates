using Surrogates.Mappers.Entities;
using Surrogates.Mappers.Parsers;
using Surrogates.Tactics;
using System;

namespace Surrogates.Mappers
{
    /// <summary>
    /// Mapper made for Surrogates Command Language
    /// </summary>
    public class DefaultSCLMapper : IFlushTypes
    {
        private T Change<T>(object value)
        {
            return __refvalue( __makeref(value),T);
        }

        protected MappingState State;
        private Strategies _strategies; 
        private Type[] _interceptors;

        public void Accept(string input, Strategies strategies, params Type[] interceptors)
        {
            var parser = new SCLParser();
            _strategies = strategies;

            string[] aliases = null;
            
            if (!parser.TryGetAliases(input, ref aliases)) 
            { throw new FormatException("Could not extract 'Aliases' from the command written"); }

            if (!parser.TryGetOperations(input, aliases, _interceptors, ref strategies))
            { throw new FormatException("Could extract an expression from the command written"); }
        }

        public Type Flush()
        {
            return this._strategies.Apply();
        }
    }
}
