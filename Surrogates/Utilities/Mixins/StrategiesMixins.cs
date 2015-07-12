using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Utilities.Mixins
{
    public static class StrategiesMixins
    {
        internal static PropertyBuilder DefineNewProperty<T>(this Strategies self, string name)
        {
            return self.DefineNewProperty(typeof(T), name);
        }

        internal static PropertyBuilder DefineNewProperty(this Strategies self, Type type, string name)
        {
            FieldInfo field = null;

            for (int i = 0; i < self.Fields.Count; i++)
            {
                if (TryMatchField(type, name, self.Fields[i], out field))
                { break; }
            }

            foreach (var f in self.BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (TryMatchField(type, name, f, out field))
                { break; }
            }

            return self.Builder.DefineNewProperty(type, name, field);
        }

        private static bool TryMatchField(Type type, string name, FieldInfo field, out FieldInfo foundField)
        {
            var hasCompatibleName =
                (field.Name[0] == '_' && field.Name.Substring(2) == name.Substring(1)) ||
                (field.Name == name);

            if (hasCompatibleName && field.FieldType.IsAssignableFrom(type))
            {
                foundField = field;
                return true;
            }

            foundField = null;
            return false;
        }
    }
}