using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text.RegularExpressions;
using Surrogates.Mappers.Collections;

namespace Surrogates.Mappers.Parsers
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
                new Regex(@"((?<operation>\w+)(?:\s+)(?<members>(\w\.\w+,?)+)(?:\s*)((((?:(a|A)(c|C)(c|C)(e|E)(s|S)(s|S)(o|O)(r|R)(s|S)?\s+)(((?<accessor1>((s|S)|(g|G))(e|E)(t|T)(t|T)(e|E)(r|R))(?:\s*\=\s*)(?<accessMethod1>\w.\w+)(?:\s+))((?:(a|A)(n|N)(d|D)\s+)(?<accessor2>((s|S)|(g|G))(e|E)(t|T)(t|T)(e|E)(r|R))(?:\s*\=\s*)(?<accessMethod2>\w.\w+)(?:\s*))?))|(?:(w|W)(i|I)(t|T)(h|H)\s+)(?<interceptor>\w.\w+)))?)",
                    RegexOptions.Compiled);
        }

        [TargetedPatchingOptOut("")]
        private static Strategy.ForProperties ExtractGetterORSetter(FieldList interceptors, GroupCollection grp, Strategy.ForProperties strategy, int index)
        {
            var metName =
                    grp[string.Concat("accessMethod", index)].Value.Split('.');

            var intType = interceptors[metName[0]].FieldType;

            strategy.Fields.TryAdd(intType, ref metName[0]);

            var interceptor = 
                new Strategy.Interceptor
                {
                    Name = metName[0],
                    DeclaredType = intType,
                    Method = intType.GetMethod4Surrogacy(metName[1])
                };

            if (grp[string.Concat("accessor", index)].Value[0] == 'g' ||
                grp[string.Concat("accessor", index)].Value[0] == 'G')
            {
                strategy.Getter = interceptor;
            }
            else { strategy.Getter = interceptor; }

            return strategy;
        }

        protected virtual Strategy Get4Properties(Strategies strategies, GroupCollection grp)
        {
            var strategy = new Strategy.ForProperties(strategies);

            if (grp["members"].Success != null)
            {
                var owner = strategy.BaseType;

                var members = grp["members"]
                    .Value
                    .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var propName in members)
                {
                    // verify whether it has the lias or not, and throw an exception if does not
                    var prop =
                        owner.GetProp4Surrogacy(propName.Split('.')[1]);

                    strategy.Properties.Add(prop);
                }
            }

            if (grp["accessor1"].Success)
            {
                strategy = ExtractGetterORSetter(
                    strategies.Fields, grp, strategy, 1);

                if (grp["accessor2"].Success)
                {
                    strategy = ExtractGetterORSetter(
                        strategies.Fields, grp, strategy, 2);
                }
            }
            return strategy;
        }

        protected virtual Strategy Get4Methods(Strategies strategies, GroupCollection grp)
        {
            var strategy = 
                new Strategy.ForMethods(strategies);
            
            if (grp["members"].Success)
            {
                var owner = strategy.BaseType;

                var members = grp["members"]
                    .Value
                    .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var methName in members)
                {
                    // verify whether it has the alias or not, and throw an exception if does not
                    var method =
                        owner.GetMethod4Surrogacy(methName.Split('.')[1]);

                    strategy.Methods.Add(method);
                }
            }

            var interceptorName = grp["interceptor"]
                .Value
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

            var intType = strategies
                .Fields[interceptorName[0]]
                .FieldType;

            var intMethod =
                intType.GetMethod4Surrogacy(interceptorName[1]);

            strategy.Methods.Add(intMethod);

            return strategy;
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
                    .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            }

            return false;
        }

        public bool TryGetOperations(string cmd, string[] aliases, Type[] interceptors, ref Strategies strategies)
        {
            var matches =
                _operationsRegexp.Matches(cmd);

            for (int i = 0; i < aliases.Length; i++)
            {
                strategies.Fields.TryAdd(interceptors[i], ref aliases[i]);
            }

            for (int i = 0; i < matches.Count; i++)
            {
                if (!matches[i].Success) { return false; }

                var grps = matches[i].Groups;

                strategies.Add(grps["accessor1"].Success ?
                    Get4Properties(strategies, grps) :
                    Get4Methods(strategies, grps));
            }

            return true;
        }
    }
}
