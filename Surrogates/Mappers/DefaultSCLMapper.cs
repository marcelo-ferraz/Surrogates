using Surrogates.Mappers.Entities;
using Surrogates.Mappers.Parsers;
using Surrogates.Tactics;
using System;

namespace Surrogates.Mappers
{
    /// <summary>
    /// Mapper made for Surrogates Command Language
    /// </summary>
    public class DefaultSCLMapper : IMapper<string>
    {
        private T Change<T>(object value)
        {
            return __refvalue( __makeref(value),T);
        }

        public Strategies Strategies { get; set; }

        protected MappingState State;

        public void Accept<T>(string input, params Type[] interceptors)
        {
            var parser = new SCLParser();

            string[] aliases = null;
            
            if (!parser.TryGetAliases(input, ref aliases)) 
            { throw new FormatException("Could not extract 'Aliases' from the command written"); }

            var strategies = new Strategies();

            if (!parser.TryGetOperations<T>(input, aliases, interceptors, ref strategies))
            { throw new FormatException("Could extract an expression from the command written"); }

            Strategies = strategies;
        }

        public Type Flush()
        {
            return this.Strategies.Apply();
        }
    }
}
