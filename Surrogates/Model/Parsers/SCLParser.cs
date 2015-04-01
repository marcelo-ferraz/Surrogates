using Surrogates.Model.Collections;
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;
using System.Text.RegularExpressions;

namespace Surrogates.Model.Parsers
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
        private static Strategy.ForProperties ExtractGetterORSetter(FieldList interceptors, GroupCollection grp, Strategy.ForProperties strategy, int index, IDictionary<string, Type> dic)
        {
            var metName =
                    grp[string.Concat("accessMethod", index)].Value.Split('.');

            var intType = dic[metName[0]];

            var interceptor = new Strategy.Interceptor(
                metName[0], intType, intType.GetMethod4Surrogacy(metName[1]));

            if (grp[string.Concat("accessor", index)].Value[0] == 'g' ||
                grp[string.Concat("accessor", index)].Value[0] == 'G')
            {
                strategy.Getter = interceptor;
            }
            else { strategy.Getter = interceptor; }

            return strategy;
        }

        private static InterferenceKind GetInterferenceKind(GroupCollection grp, Strategy strategy)
        {
            InterferenceKind kind;
            if (Enum.TryParse<InterferenceKind>(grp["operation"].Value, true, out kind))
            {
                return kind;
            }

            strategy.Kind = InterferenceKind.Extensions;
            strategy.KindExtended =
                grp["operation"].Value.ToLower();

            if (!Strategy.Executioners.ContainsKey(strategy.KindExtended))
            {
                throw new NotSupportedException(strategy.KindExtended);
            }

            return kind;
        }


        protected virtual Strategy Get4Properties(Strategies strategies, GroupCollection grp, IDictionary<string, Type> dic)
        {
            var strategy = new Strategy.ForProperties(strategies);

            strategy.Kind = 
                GetInterferenceKind(grp, strategy);

            if (grp["members"].Success)
            {
                var owner = strategy.BaseType;

                var members = grp["members"]
                    .Value
                    .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var propName in members)
                {
                    // verify whether it has the alias or not, and throw an exception if does not
                    var prop =
                        owner.GetProp4Surrogacy(propName.Split('.')[1]);

                    strategy.Properties.Add(prop);
                }
            }

            if (grp["accessor1"].Success)
            {
                strategy = ExtractGetterORSetter(
                    strategies.Fields, grp, strategy, 1, dic);

                if (grp["accessor2"].Success)
                {
                    strategy = ExtractGetterORSetter(
                        strategies.Fields, grp, strategy, 2, dic);
                }
            }
            return strategy;
        }

        protected virtual Strategy Get4Methods(Strategies strategies, GroupCollection grp, IDictionary<string, Type> dic)
        {
            var strategy = 
                new Strategy.ForMethods(strategies);

            strategy.Kind =
                GetInterferenceKind(grp, strategy);

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

            if (grp["interceptor"].Success)
            {
                var interceptorName = grp["interceptor"]
                    .Value
                    .Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                var intType = dic[interceptorName[0]];

                var intMethod =
                    intType.GetMethod4Surrogacy(interceptorName[1]);

                strategy.Interceptor = new Strategy.Interceptor(
                    interceptorName[0], intType, intMethod);
            }

            return strategy;
        }

        public bool TryGetAliases(string cmd, ref string[] aliases)
        {
            var match =
                _aliasesRegexp.Match(cmd);

            if (match.Success)
            {
                var arr = match.Groups["aliases"]
                    .Value
                    .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                Array.Resize(ref aliases, arr.Length);

                arr.CopyTo(aliases, 0);

                return true;
            }

            return false;
        }

        public bool TryGetOperations(string cmd, string[] aliases, Type[] interceptors, ref Strategies strategies)
        {
            var matches =
                _operationsRegexp.Matches(cmd);

            int i = 1;

            var dic = aliases.Length > 1 ?
                interceptors.ToDictionary(@int => aliases[i].Trim(), @int => @int) :
                null;

            for (int j = 0; j < matches.Count; j++)
            {
                if (!matches[j].Success) { return false; }

                var grps = matches[j].Groups;

                strategies.Add(grps["accessor1"].Success ?
                    Get4Properties(strategies, grps, dic) :
                    Get4Methods(strategies, grps, dic));
            }

            return true;
        }
    }
}
