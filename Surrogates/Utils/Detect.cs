using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Surrogates.Mappers;

namespace Surrogates.Utils
{
    public static class Detect
    {
        public static bool IsAutomatic(Property prop)
        {
            var owner = 
                prop.Original.ReflectedType;

            var field = owner.GetField(
                string.Format("<{0}>k__BackingField", prop.Original.Name), 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            return field != null;
        }
    }
}
