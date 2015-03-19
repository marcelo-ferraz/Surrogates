using Surrogates.Expressions;
using Surrogates.Mappers.Entities;
using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Mappers
{
    public class SCLParser
    {
        private static Regex _aliasesRegexp;
        private static Regex _operationsRegexp;

        static SCLParser()
        {
            _aliasesRegexp = 
                new Regex(@"^(?:(a|A)(s|S)\s+(?<aliases>(\w,?\s)+))(?<rest>.+)$", RegexOptions.Compiled);

            _operationsRegexp = 
                new Regex(@"((?<operation>\w+)(?:\s+)(?<members>(\w\.\w+,?\s)+)(?:\s*)(((?:(a|A)(c|C)(c|C)(e|E)(s|S)(s|S)(o|O)(r|R)(s|S)?\s+)((((?<accessor1>((s|S)|(g|G))(e|E)(t|T)(t|T)(e|E)(r|R))(?:\s*\=\s*)(?<accessMethod1>\w.\w+)(?:\s+)))((?:and\s+)(?<accessor2>((s|S)|(g|G))(e|E)(t|T)(t|T)(e|E)(r|R))(?:\s*\=\s*)(?<accessMethod2>\w.\w+)(?:\s+))?)|(?:(w|W)(i|I)(t|T)(h|H)\s+)(?<property>\w.\w+))))",
                    RegexOptions.Compiled);
        }

        public bool TryGetAliases(string cmd, ref string[] aliases)
        {
            var match = 
                _aliasesRegexp.Match(cmd);

            if (match.Success) 
            {
                aliases = match
                    .Groups["aliases"]
                    .Value
                    .Split(new [] {", "}, StringSplitOptions.RemoveEmptyEntries);
            }

            return false;
        }

        public bool TryGetOperations(string cmd, ref Strategies strategies)
        {
            var matches =
                _operationsRegexp.Matches(cmd);

            for (int i = 0; i < matches.Count; i++)
            {
                if (!matches[i].Success) { return false; }

                var grp = matches[i].Groups;

                var op = grp["operation"].Value;
                
                Strategy strategy = grp["accessor1"].Success ?
                    (Strategy) new Strategy.ForProperties() :
                    new Strategy.ForMethods();

                if(grp["members"].Success != null)
                {
                    var members = grp["members"]
                        .Value
                        .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);                                        
                }

                if (grp["properties"].Success)
                {
                    __refvalue(__makeref(strategy), Strategy.ForProperties).Properties.Add(new Property2(strategy));
                    //grp["property"].Value;
                    continue;
                }
                
                if (grp["accessor1"].Success)
                {   
                    //grp["accessor1"].Value, grp["accessMethod1"].Value);

                    if (grp["accessor2"].Success)
                    {
                        //grp["accessor2"].Value, grp["accessMethod2"].Value);
                    }
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Mapper made for Surrogates Command Language
    /// </summary>
    public class SCLMapper: IMapper
    {
        private T Change<T>(object value)
        {
            return __refvalue( __makeref(value),T);
        }

        protected IFlushTypes MapExpression;

        protected MappingState State;

        public void Accept<T>(string cmd, IMappingExpression<T> expression)
        {
            var parser = new SCLParser();

            string[] aliases = null;

            if (!parser.TryGetAliases(cmd, ref aliases)) 
            { throw new FormatException("Could not extract 'Aliases' from the command written"); }


            
            parser.OperationFound += 
                op => 
                {
                    if (op.ToLower() == "replace")
                    { expression = expression.Replace; }
                    else if (op.ToLower() == "disable")
                    { expression = expression.Disable; }
                    else if (op.ToLower() == "visit")
                    { expression = expression.Visit; }
                };

            parser.MembersFound += m => { };
            parser.MethodFound += m => {};
            parser.AccessorFound += (a, m) => { };
            parser.MethodFound += m => { };

            if (!parser.TryGetOperations(cmd))
            { throw new FormatException("Could extract an expression from the command written"); }
        }

        public Type Flush()
        {
            throw new NotImplementedException();
        }
    }
}
