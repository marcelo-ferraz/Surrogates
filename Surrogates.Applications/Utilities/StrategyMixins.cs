﻿
using Surrogates.Model;
using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Surrogates.Applications.Utilities
{
    public static class MixinsHelpers
    {
        public static Dictionary<string, T> MergeProperty<T>(this Strategy.ForProperties self, string name, T value)
        {
            return self.MergeProperty<T>(name, p => value);
        }

        public static Dictionary<string, T> MergeProperty<T>(this Strategy.ForProperties self, string name, Func<SurrogatedProperty, T> getValue)
        {
            var prop = self
                .NewProperties
                .Find(p => p.Name == name);

            var newValues = self
                .Properties
                .ToDictionary(p => p.Original.Name, p => getValue(p));

            return prop != null ?
                ((Dictionary<string, T>)prop.DefaultValue).MergeLeft(newValues) :
                newValues;
        }

        public static Dictionary<IntPtr, T> MergeProperty<T>(this Strategy self, string name, Func<MethodInfo, T> getValue)
        {
            var newValues =
                self.BaseMethods.ToDictionary(m => m.MethodHandle.Value, getValue);

            var paramsProp = self
                .NewProperties
                .FirstOrDefault(p => p.Name == "Params");

            return paramsProp != null ?
                ((Dictionary<IntPtr, T>)paramsProp.DefaultValue).MergeLeft(newValues) :
                newValues;
        }
    }
}
