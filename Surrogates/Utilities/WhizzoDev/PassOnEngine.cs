using Surrogates.Utilities.SDILReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Surrogates.Utilities.Mixins;

namespace Surrogates.Utilities.WhizzoDev
{
    /// <summary>
    /// Class that clones objects
    /// </summary>
    /// <remarks>
    /// Currently can deepclone to 1 level deep.
    /// Ex. Person.Addresses (Person.List<Address>) 
    /// -> Clones 'Person' deep
    /// -> Clones the objects of the 'Address' list deep
    /// -> Clones the sub-objects of the Address object shallow. (at the moment)
    /// </remarks>
    public static class PassOnEngine
    {
        // Dictionaries for caching the (pre)compiled generated IL code.
        private static Dictionary<Tuple<Type, Type>, Delegate> _cachedILShallow = new Dictionary<Tuple<Type, Type>, Delegate>();
        private static Dictionary<Tuple<Type, Type>, Delegate> _cachedILDeep = new Dictionary<Tuple<Type, Type>, Delegate>();


        private static void CopyProperties(Type returnType, Type sourceType, ILGenerator il, Action<PropertyInfo, PropertyInfo> whenSame, bool ignoreType = false)
        {
            var destPropertys =
                GetProperties(returnType);

            foreach (var srcProperty in GetProperties(sourceType))
            {
                foreach (var destProperty in destPropertys)
                {
                    Func<bool> isAssignable = () =>
                        destProperty.PropertyType.IsAssignableFrom(srcProperty.PropertyType) ||
                        srcProperty.PropertyType.IsAssignableFrom(destProperty.PropertyType);

                    var aliases = GetAliasesForProperty(srcProperty);

                    var namesAreEqual =
                        destProperty.Name.Equals(srcProperty.Name) ||
                        (aliases != null && Array.IndexOf(aliases, destProperty.Name, 0) > -1);

                    if ((ignoreType || isAssignable()) && namesAreEqual)
                    {
                        whenSame(srcProperty, destProperty);
                    }
                }
            }
        }

        private static void Construct(Type type, ILGenerator il)
        {
            ConstructorInfo cInfo = type.GetConstructor(Type.EmptyTypes);
            if (cInfo != null)
            {
                il.Emit(OpCodes.Newobj, cInfo);
            }
            else
            {
                var getType =
                    type.GetMethod("GetType");

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, getType);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);

                var methodInfo =
                    typeof(FormatterServices).GetMethod("GetUninitializedObject");

                il.Emit(OpCodes.Call, methodInfo);
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private static void EmitPassOn(ILGenerator generator, LocalBuilder cloneVariable, PropertyInfo srcProperty, PropertyInfo destProperty)
        {
            generator.Emit(OpCodes.Ldloc, cloneVariable);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, srcProperty.GetGetMethod());
            generator.Emit(OpCodes.Call, destProperty.GetSetMethod());
        }

        /// <summary>
        /// Helper method to clone a reference type.
        /// This method clones IList and IEnumerables and other reference types (classes)
        /// Arrays are not yet supported (ex. string[])
        /// </summary>
        /// <param name="il">IL il to emit code to.</param>
        /// <param name="cloneVar">Local store wheren the clone object is located. (or child of)</param>
        /// <param name="srcProperty">Property definition of the reference type to clone.</param>
        private static void CopyReferenceType(ILGenerator il, LocalBuilder cloneVar, PropertyInfo source, PropertyInfo destination)
        {
            // does not copy a delegate
            if (source.PropertyType.IsSubclassOf(typeof(Delegate))) { return; }

            il.Emit(OpCodes.Ldloc, cloneVar);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, source.GetGetMethod());
            il.Emit(OpCodes.Call, GetCorrectClonningMethod(source.PropertyType, destination.PropertyType));
            il.Emit(OpCodes.Call, destination.GetSetMethod());
        }

        /// <summary>
        /// Returns the type of cloning to apply on a certain srcProperty when in custom mode.
        /// Otherwise the main cloning method is returned.
        /// You can invoke custom mode by invoking the method Clone(T obj)
        /// </summary>
        /// <param name="srcProperty">Property to examine</param>
        /// <returns>Type of cloning to use for this srcProperty.</returns>
        private static CloneType GetCloneTypeForProperty(PropertyInfo prop)
        {
            var attributes =
                prop.GetCustomAttributes(typeof(CloneAttribute), true);

            return attributes != null && attributes.Length > 0 ?
                (attributes[0] as CloneAttribute).CloneType :
                CloneType.Deep;
        }

        private static string[] GetAliasesForProperty(PropertyInfo prop)
        {
            var attributes =
                prop.GetCustomAttributes(typeof(CloneAttribute), true);

            return attributes != null && attributes.Length > 0 ?
                (attributes[0] as CloneAttribute).Aliases :
                null;
        }

        private static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        internal static MethodInfo GetCorrectClonningMethod(Type source, Type destination)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(source) &&
                !typeof(IEnumerable).IsAssignableFrom(destination) &&
                !destination.IsArray &&
                !source.IsArray)
            {
                return typeof(PassOn)
                    .GetMethod("To", new Type[] { typeof(object) })
                    .MakeGenericMethod(destination);
            }

            var srcItem =
                source.IsArray ? source.GetElementType() :
                source.IsGenericType ? source.GetGenericArguments()[0] :
                null;

            var destType =
                destination.IsArray ? destination.GetElementType() :
                destination.IsGenericType ? destination.GetGenericArguments()[0] :
                null;

            if (srcItem == null || destType == null)
            {
                throw new NotSupportedException();
            }

            var cloneType = typeof(PassOn<>)
                .MakeGenericType(srcItem);

            return destination.IsArray ?
                cloneType.GetToArrayOfMethod(source, destType) :
                cloneType.GetToListOfMethod(source, destType);
        }

        internal static object MergeWithILDeep(object source, object destination)
        {
            if (source == null) { return destination; }
            if (destination == null) { return source; }

            Delegate myExec = null;

            var key =
                new Tuple<Type, Type>(destination.GetType(), source.GetType());

            if (!_cachedILDeep.TryGetValue(key, out myExec))
            {
                // Create ILGenerator            
                DynamicMethod dymMethod = new DynamicMethod(
                    "DoDeepMerge",
                    destination.GetType(),
                    new Type[] { source.GetType(), destination.GetType() },
                    Assembly.GetExecutingAssembly().ManifestModule,
                    true);

                ILGenerator il = dymMethod.GetILGenerator();

                LocalBuilder cloneVariable = il.DeclareLocal((source ?? destination).GetType());

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stloc, cloneVariable);

                CopyProperties(destination.GetType(), source.GetType(), il,
                    (src, dest) =>
                    {
                        if ((dest.PropertyType.IsAssignableFrom(src.PropertyType) &&
                                         (GetCloneTypeForProperty(src) == CloneType.Shallow ||
                                         src.PropertyType.IsValueType ||
                                         src.PropertyType == typeof(string))))
                        {
                            EmitPassOn(il, cloneVariable, src, dest);
                        }//CloneType.Deep
                        else if (src.PropertyType.IsClass)
                        {
                            CopyReferenceType(il, cloneVariable, src, dest);
                        }
                    });

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                var delType = typeof(Func<,,>)
                    .MakeGenericType(source.GetType(), destination.GetType(), destination.GetType());

                myExec = dymMethod.CreateDelegate(delType);
                _cachedILDeep.Add(key, myExec);
            }
            return myExec.DynamicInvoke(source, destination);
        }

        /// <summary>
        /// Generic cloning method that clones an object using IL.
        /// Only the first call of a certain type will hold back performance.
        /// After the first call, the compiled IL is executed. 
        /// </summary>
        /// <param name="source">Type of object to clone</param>
        /// <returns>Cloned object (deeply cloned)</returns>
        internal static object CloneObjectWithILDeep(Type returnType, object source)
        {
            if (source == null)
            { return null; }

            var key = new Tuple<Type, Type>(
                returnType.GetType(), source.GetType());

            Delegate myExec = null;

            if (!_cachedILDeep.TryGetValue(key, out myExec))
            {
                // Create ILGenerator            
                var dymMethod = new DynamicMethod(
                    "DoDeepClone",
                    returnType,
                    new Type[] { source.GetType() },
                    Assembly.GetExecutingAssembly().ManifestModule,
                    true);

                var il =
                    dymMethod.GetILGenerator();

                var cloneVariable =
                    il.DeclareLocal(returnType);

                Construct(returnType, il);

                il.Emit(OpCodes.Stloc, cloneVariable);

                CopyProperties(returnType, source.GetType(), il,
                    (src, dest) =>
                    {
                        if ((dest.PropertyType.IsAssignableFrom(src.PropertyType) &&
                            (GetCloneTypeForProperty(src) == CloneType.Shallow ||
                            src.PropertyType.IsValueType ||
                            src.PropertyType == typeof(string))))
                        {
                            EmitPassOn(il, cloneVariable, src, dest);
                        }//CloneType.Deep
                        else if (src.PropertyType.IsClass)
                        {
                            CopyReferenceType(il, cloneVariable, src, dest);
                        }
                    }, ignoreType: true);

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                var delType = typeof(Func<,>)
                    .MakeGenericType(source.GetType(), returnType);

                myExec = dymMethod.CreateDelegate(delType);
                _cachedILDeep.Add(key, myExec);
            }
            return myExec.DynamicInvoke(source);
        }

        /// <summary>    
        /// Generic cloning method that clones an object using IL.    
        /// Only the first call of a certain type will hold back performance.    
        /// After the first call, the compiled IL is executed.    
        /// </summary>    
        /// <typeparam name="T">Type of object to clone</typeparam>    
        /// <param name="source">Object to clone</param>    
        /// <returns>Cloned object (shallow)</returns>    
        internal static object CloneObjectWithILShallow(Type returnType, object source)
        {
            Delegate myExec = null;

            if (source == null) { return null; }


            var key = new Tuple<Type, Type>(
                returnType.GetType(), source.GetType());

            if (!_cachedILShallow.TryGetValue(key, out myExec))
            {
                DynamicMethod dymMethod =
                    new DynamicMethod("DoShallowClone", returnType, new Type[] { source.GetType() }, Assembly.GetExecutingAssembly().ManifestModule, true);

                var cInfo =
                    returnType.GetConstructor(new Type[] { });
                var il =
                    dymMethod.GetILGenerator();

                var local =
                    il.DeclareLocal(source.GetType());

                il.Emit(OpCodes.Newobj, cInfo);
                il.Emit(OpCodes.Stloc_0);

                CopyProperties(returnType, source.GetType(), il,
                    (src, dest) =>
                    {
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Call, src.GetGetMethod());
                        il.Emit(OpCodes.Call, dest.GetSetMethod());
                    });

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                var delType = typeof(Func<,>)
                    .MakeGenericType(source.GetType(), source.GetType());

                myExec = dymMethod.CreateDelegate(delType);
                _cachedILShallow.Add(key, myExec);
            }
            return myExec.DynamicInvoke(source);
        }
    }
}