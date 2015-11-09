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
    public static class CloneHelper
    {
        #region Declarations
        // Dictionaries for caching the (pre)compiled generated IL code.
        private static Dictionary<Tuple<Type, Type>, Delegate> _cachedILShallow = new Dictionary<Tuple<Type, Type>, Delegate>();
        private static Dictionary<Tuple<Type, Type>, Delegate> _cachedILDeep = new Dictionary<Tuple<Type, Type>, Delegate>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as ShallowCloning and/or DeepCloning combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static T Clone<T>(object obj)
            where T : class
        {
            return (T)CloneObjectWithILDeep(typeof(T), obj);
        }


        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as ShallowCloning and/or DeepCloning combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static object Clone(Type returnType, object obj)
        {
            return CloneObjectWithILDeep(returnType, obj);
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static T Clone<T>(object obj, CloneType cloneType = CloneType.DeepCloning)
            where T : class
        {
            if (cloneType == CloneType.ShallowCloning)
            {
                return (T)CloneObjectWithILShallow(typeof(T), obj);
            }

            return (T)CloneObjectWithILDeep(typeof(T), obj);
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static object Clone(Type returnType, object obj, CloneType cloneType = CloneType.DeepCloning)
        {
            if (cloneType == CloneType.ShallowCloning)
            { return CloneObjectWithILShallow(returnType, obj); }

            return CloneObjectWithILDeep(returnType, obj);
        }

        public static T Merge<T>(T source, T destination)
        {
            return (T)MergeWithILDeep(source, destination);
        }

        public static object Merge(object source, object destination)
        {
            return MergeWithILDeep(source, destination);
        }

        #endregion

        #region Private Methods

        /// <summary>    
        /// Generic cloning method that clones an object using IL.    
        /// Only the first call of a certain type will hold back performance.    
        /// After the first call, the compiled IL is executed.    
        /// </summary>    
        /// <typeparam name="T">Type of object to clone</typeparam>    
        /// <param name="source">Object to clone</param>    
        /// <returns>Cloned object (shallow)</returns>    
        private static object CloneObjectWithILShallow(Type returnType, object source)
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
                        (aliases == null || Array.IndexOf(aliases, destProperty.Name, 0) > -1);

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

        /// <summary>
        /// Generic cloning method that clones an object using IL.
        /// Only the first call of a certain type will hold back performance.
        /// After the first call, the compiled IL is executed. 
        /// </summary>
        /// <param name="source">Type of object to clone</param>
        /// <returns>Cloned object (deeply cloned)</returns>
        private static object CloneObjectWithILDeep(Type returnType, object source)
        {
            if (source == null)
            { return null; }

            var key = new Tuple<Type, Type>(
                returnType.GetType(), source.GetType());

            Delegate myExec = null;

            if (!_cachedILDeep.TryGetValue(key, out myExec))
            {
                // Create ILGenerator            
                var dymMethod =
                    new DynamicMethod("DoDeepClone", returnType, new Type[] { source.GetType() }, Assembly.GetExecutingAssembly().ManifestModule, true);
                var il =
                    dymMethod.GetILGenerator();

                var cloneVariable =
                    il.DeclareLocal(returnType);

                Construct(returnType, il);

                il.Emit(OpCodes.Stloc, cloneVariable);


                CopyProperties(returnType, source.GetType(), il,
                    (src, dest) =>
                    {

                        if (GetCloneTypeForProperty(src) == CloneType.ShallowCloning)
                        {
                            il.Emit(OpCodes.Ldloc, cloneVariable);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Call, src.GetGetMethod());
                            il.Emit(OpCodes.Call, dest.GetSetMethod());
                        }
                        else //CloneType.DeepCloning
                        {
                            if (src.PropertyType.IsValueType || src.PropertyType == typeof(string))
                            {
                                il.Emit(OpCodes.Ldloc, cloneVariable);
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Call, src.GetGetMethod());
                                il.Emit(OpCodes.Call, dest.GetSetMethod());
                            }
                            else if (src.PropertyType.IsClass)
                            { CopyReferenceType(il, cloneVariable, src, dest); }
                        }
                    });

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                var delType = typeof(Func<,>)
                    .MakeGenericType(source.GetType(), returnType);

                myExec = dymMethod.CreateDelegate(delType);
                _cachedILDeep.Add(key, myExec);
            }
            return myExec.DynamicInvoke(source);
        }

        private static object MergeWithILDeep(object source, object destination)
        {
            if (source == null) { return destination; }
            if (destination == null) { return source; }

            Delegate myExec = null;

            var key =
                new Tuple<Type, Type>(destination.GetType(), source.GetType());

            if (!_cachedILDeep.TryGetValue(key, out myExec))
            {
                // Create ILGenerator            
                DynamicMethod dymMethod = new DynamicMethod("DoDeepMerge", destination.GetType(), new Type[] { source.GetType(), destination.GetType() }, Assembly.GetExecutingAssembly().ManifestModule, true);
                ILGenerator generator = dymMethod.GetILGenerator();

                CreateMerger(source, destination, generator);
                // testar com typof(Delegate)

                var delType = typeof(Func<,,>)
                    .MakeGenericType(source.GetType(), destination.GetType(), destination.GetType());

                myExec = dymMethod.CreateDelegate(delType);
                _cachedILDeep.Add(key, myExec);
            }
            return myExec.DynamicInvoke(source, destination);
        }

        private static void CreateMerger(object source, object destination, ILGenerator il)
        {
            LocalBuilder cloneVariable = il.DeclareLocal((source ?? destination).GetType());

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stloc, cloneVariable);

            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            CopyProperties(destination.GetType(), source.GetType(), il,
                (src, dest) =>
                {
                    if (GetCloneTypeForProperty(src) == CloneType.ShallowCloning)
                    {
                        EmitPassOn(il, cloneVariable, src, dest);
                        return;
                    }

                    // CloneType.DeepCloning
                    if (src.PropertyType.IsValueType || src.PropertyType == typeof(string))
                    {
                        EmitPassOn(il, cloneVariable, src, dest);
                    }
                    else if (src.PropertyType.IsClass)
                    {
                        CopyReferenceType(il, cloneVariable, src, dest);
                    }
                });

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
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
        private static void CopyReferenceType(ILGenerator generator, LocalBuilder cloneVar, PropertyInfo sourceProp, PropertyInfo destProp)
        {
            // does not copy a delegate
            if (sourceProp.PropertyType.IsSubclassOf(typeof(Delegate))) { return; }

            var lbTempVar =
                generator.DeclareLocal(sourceProp.PropertyType);

            if (sourceProp.PropertyType.GetInterface("IEnumerable") != null && sourceProp.PropertyType.GetInterface("IList") != null)
            {
                if (sourceProp.PropertyType.IsGenericType)
                {
                    Type argumentType = sourceProp.PropertyType.GetGenericArguments()[0];
                    Type genericTypeEnum = Type.GetType("System.Collections.Generic.IEnumerable`1[" + argumentType.FullName + "]");

                    ConstructorInfo ci = sourceProp.PropertyType.GetConstructor(new Type[] { genericTypeEnum });

                    if (ci != null && GetCloneTypeForProperty(sourceProp) == CloneType.ShallowCloning)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Call, sourceProp.GetGetMethod());
                        generator.Emit(OpCodes.Newobj, ci);
                        generator.Emit(OpCodes.Stloc, lbTempVar);
                        generator.Emit(OpCodes.Ldloc, cloneVar);
                        generator.Emit(OpCodes.Ldloc, lbTempVar);
                        generator.Emit(OpCodes.Call, destProp.GetSetMethod());
                    }
                    else
                    {
                        ci = sourceProp.PropertyType.GetConstructor(Type.EmptyTypes);
                        if (ci != null)
                        {
                            generator.Emit(OpCodes.Newobj, ci);
                            generator.Emit(OpCodes.Stloc, lbTempVar);
                            generator.Emit(OpCodes.Ldloc, cloneVar);
                            generator.Emit(OpCodes.Ldloc, lbTempVar);
                            generator.Emit(OpCodes.Call, destProp.GetSetMethod());
                            CloneList(generator, sourceProp, argumentType, lbTempVar);
                        }
                    }
                }
            }
            else
            {
                Construct(sourceProp.PropertyType, generator);

                generator.Emit(OpCodes.Stloc, lbTempVar);
                generator.Emit(OpCodes.Ldloc, cloneVar);
                generator.Emit(OpCodes.Ldloc, lbTempVar);
                generator.Emit(OpCodes.Call, sourceProp.GetSetMethod());

                CopyProperties(cloneVar.LocalType, sourceProp.PropertyType, generator,
                    (src, dest) =>
                    {
                        if (src.PropertyType.IsValueType || dest.PropertyType == typeof(string))
                        {
                            generator.Emit(OpCodes.Ldloc_1);
                            generator.Emit(OpCodes.Ldarg_0);
                            generator.Emit(OpCodes.Call, sourceProp.GetGetMethod());
                            generator.Emit(OpCodes.Call, dest.GetGetMethod());
                        }
                    });

            }
        }

        /// <summary>
        /// Makes a deep copy of an IList of IEnumerable
        /// Creating new objects of the list and containing objects. (using default constructor)
        /// And by invoking the deepclone method defined above. (recursive)
        /// </summary>
        /// <param name="il">IL il to emit code to.</param>
        /// <param name="listProperty">Property definition of the reference type of the list to clone.</param>
        /// <param name="typeToClone">Base-type to clone (argument of List<T></param>
        /// <param name="cloneVar">Local store wheren the clone object is located. (or child of)</param>
        private static void CloneList(ILGenerator generator, PropertyInfo listProperty, Type typeToClone, LocalBuilder cloneVar)
        {
            Type genIEnumeratorTyp = Type.GetType("System.Collections.Generic.IEnumerator`1[" + typeToClone.FullName + "]");
            Type genIEnumeratorTypLocal = Type.GetType(listProperty.PropertyType.Namespace + "." + listProperty.PropertyType.Name + "+Enumerator[[" + typeToClone.FullName + "]]");
            LocalBuilder lbEnumObject = generator.DeclareLocal(genIEnumeratorTyp);
            LocalBuilder lbCheckStatement = generator.DeclareLocal(typeof(bool));
            Label checkOfWhile = generator.DefineLabel();
            Label startOfWhile = generator.DefineLabel();
            MethodInfo miEnumerator = listProperty.PropertyType.GetMethod("GetEnumerator");
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, listProperty.GetGetMethod());
            generator.Emit(OpCodes.Callvirt, miEnumerator);
            if (genIEnumeratorTypLocal != null)
            {
                generator.Emit(OpCodes.Box, genIEnumeratorTypLocal);
            }
            generator.Emit(OpCodes.Stloc, lbEnumObject);
            generator.Emit(OpCodes.Br_S, checkOfWhile);
            generator.MarkLabel(startOfWhile);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ldloc, cloneVar);
            generator.Emit(OpCodes.Ldloc, lbEnumObject);
            MethodInfo miCurrent = genIEnumeratorTyp.GetProperty("Current").GetGetMethod();
            generator.Emit(OpCodes.Callvirt, miCurrent);
            Type cloneHelper = Type.GetType(typeof(CloneHelper).Namespace + "." + typeof(CloneHelper).Name);
            MethodInfo miDeepClone = cloneHelper.GetMethod("CloneObjectWithILDeep", BindingFlags.Static | BindingFlags.NonPublic);
            generator.Emit(OpCodes.Call, miDeepClone);
            MethodInfo miAdd = listProperty.PropertyType.GetMethod("Add");
            generator.Emit(OpCodes.Callvirt, miAdd);
            generator.Emit(OpCodes.Nop);
            generator.MarkLabel(checkOfWhile);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ldloc, lbEnumObject);
            MethodInfo miMoveNext = typeof(IEnumerator).GetMethod("MoveNext");
            generator.Emit(OpCodes.Callvirt, miMoveNext);
            generator.Emit(OpCodes.Stloc, lbCheckStatement);
            generator.Emit(OpCodes.Ldloc, lbCheckStatement);
            generator.Emit(OpCodes.Brtrue_S, startOfWhile);
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
                CloneType.ShallowCloning;
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


        #endregion
    }
}
