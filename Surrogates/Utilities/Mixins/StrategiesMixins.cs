using Surrogates.Tactics;
using System;
using System.Reflection;
using System.Reflection.Emit;

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

            foreach (var f in self.BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (TryMatchField(type, name, f, ref field))
                { break; }
            }

            if (field == null)
            {
                for (int i = 0; i < self.Fields.Count; i++)
                {
                    if (TryMatchField(type, name, self.Fields[i], ref field))
                    { break; }
                }
            }

            return self.Builder.DefineNewProperty(type, name, field);
        }

        private static bool TryMatchField(Type type, string name, FieldInfo field, ref FieldInfo foundField)
        {
            var hasCompatibleName =
                (field.Name[0] == '_' && field.Name.Substring(2) == name.Substring(1)) ||
                (field.Name == name);

            if (hasCompatibleName && 
                // can still occur an invalid cast exception, though
                (field.FieldType.IsAssignableFrom(type) || type.IsAssignableFrom(field.FieldType)))
            {
                foundField = field;
                return true;
            }

            return false;
        }
    }
}