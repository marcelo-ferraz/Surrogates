﻿using Surrogates.Utilities.SDILReader;
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
        private static Dictionary<Type, Delegate> _cachedILShallow = new Dictionary<Type, Delegate>();
        private static Dictionary<Type, Delegate> _cachedILDeep = new Dictionary<Type, Delegate>();
                
        #endregion

        #region Public Methods

        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as ShallowCloning and/or DeepCloning combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static T Clone<T>(T obj)
            where T : class
        {
            return (T)CloneObjectWithILDeep(obj);
        }


        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as ShallowCloning and/or DeepCloning combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static object Clone(object obj)        
        {
            return CloneObjectWithILDeep(obj);
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static T Clone<T>(T obj, CloneType cloneType = CloneType.DeepCloning)
            where T: class
        {            
            switch (cloneType)
            {
                case CloneType.None:
                    throw new InvalidOperationException("No need to call this method?");
                case CloneType.ShallowCloning:
                    return (T) CloneObjectWithILShallow(obj);
                case CloneType.DeepCloning:
                    return (T) CloneObjectWithILDeep(obj);
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static object Clone(object obj, CloneType cloneType = CloneType.DeepCloning)            
        {
            switch (cloneType)
            {
                case CloneType.None:
                    throw new InvalidOperationException("No need to call this method?");
                case CloneType.ShallowCloning:
                    return CloneObjectWithILShallow(obj);
                case CloneType.DeepCloning:
                    return CloneObjectWithILDeep(obj);
                default:
                    break;
            }
            return null;
        }

        public static T Merge<T>(T source, T destination)            
        { 
            return (T) MergeWithILDeep(source, destination);            
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
        /// <param name="myObject">Object to clone</param>    
        /// <returns>Cloned object (shallow)</returns>    
        private static object CloneObjectWithILShallow(object myObject)
        {
            Delegate myExec = null;
            if (!_cachedILShallow.TryGetValue(myObject.GetType(), out myExec))
            {
                DynamicMethod dymMethod = new DynamicMethod("DoShallowClone", myObject.GetType(), new Type[] { myObject.GetType() }, Assembly.GetExecutingAssembly().ManifestModule, true);
                ConstructorInfo cInfo = myObject.GetType().GetConstructor(new Type[] { });
                ILGenerator generator = dymMethod.GetILGenerator();
                LocalBuilder lbf = generator.DeclareLocal(myObject.GetType());
                generator.Emit(OpCodes.Newobj, cInfo);
                generator.Emit(OpCodes.Stloc_0);
                foreach (FieldInfo field in myObject.GetType().GetFields(System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public))
                {
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field);
                    generator.Emit(OpCodes.Stfld, field);
                }
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ret);

                var delType = typeof(Func<,>)
                    .MakeGenericType(myObject.GetType(), myObject.GetType());

                myExec = dymMethod.CreateDelegate(delType);
                _cachedILShallow.Add(myObject.GetType(), myExec);
            }
            return myExec.DynamicInvoke(myObject);
        }
        
        private static void Construct(Type type, ILGenerator generator)
        {
            ConstructorInfo cInfo = type.GetConstructor(Type.EmptyTypes);
            if (cInfo != null)
            {
                generator.Emit(OpCodes.Newobj, cInfo);
            }
            else
            {
                MethodInfo getType = type.GetMethod("GetType");
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, getType);
                generator.Emit(OpCodes.Stloc_0);
                generator.Emit(OpCodes.Ldloc_0);

                MethodInfo methodInfo = typeof(FormatterServices).GetMethod("GetUninitializedObject");
                generator.Emit(OpCodes.Call, methodInfo);
                generator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// Generic cloning method that clones an object using IL.
        /// Only the first call of a certain type will hold back performance.
        /// After the first call, the compiled IL is executed. 
        /// </summary>
        /// <param name="myObject">Type of object to clone</param>
        /// <returns>Cloned object (deeply cloned)</returns>
        private static object CloneObjectWithILDeep(object myObject)
        {
            Delegate myExec = null;
            if (!_cachedILDeep.TryGetValue(myObject.GetType(), out myExec))
            {
                // Create ILGenerator            
                DynamicMethod dymMethod = new DynamicMethod("DoDeepClone", myObject.GetType(), new Type[] { myObject.GetType() }, Assembly.GetExecutingAssembly().ManifestModule, true);
                ILGenerator generator = dymMethod.GetILGenerator();
                LocalBuilder cloneVariable = generator.DeclareLocal(myObject.GetType());

                Construct(myObject.GetType(), generator);

                generator.Emit(OpCodes.Stloc, cloneVariable);

                foreach (FieldInfo field in myObject.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public))
                {

                    switch (GetCloneTypeForField(field))
                    {
                        case CloneType.ShallowCloning:
                            {
                                generator.Emit(OpCodes.Ldloc, cloneVariable);
                                generator.Emit(OpCodes.Ldarg_0);
                                generator.Emit(OpCodes.Ldfld, field);
                                generator.Emit(OpCodes.Stfld, field);
                                break;
                            }
                        case CloneType.DeepCloning:
                            {
                                if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                                {
                                    generator.Emit(OpCodes.Ldloc, cloneVariable);
                                    generator.Emit(OpCodes.Ldarg_0);
                                    generator.Emit(OpCodes.Ldfld, field);
                                    generator.Emit(OpCodes.Stfld, field);
                                }
                                else if (field.FieldType.IsClass)
                                    CopyReferenceType(generator, cloneVariable, field);
                                break;
                            }
                        case CloneType.None:
                            {
                                // Do nothing here, field is not cloned.
                            }
                            break;

                    }
                }
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ret);

                var delType = typeof(Func<,>)
                    .MakeGenericType(myObject.GetType(), myObject.GetType());

                myExec = dymMethod.CreateDelegate(delType);
                _cachedILDeep.Add(myObject.GetType(), myExec);
            }
            return myExec.DynamicInvoke(myObject);
        }

        private static object MergeWithILDeep(object source, object destination)
        {
            Delegate myExec = null;
            if (!_cachedILDeep.TryGetValue(destination.GetType(), out myExec))
            {
                // Create ILGenerator            
                DynamicMethod dymMethod = new DynamicMethod("DoDeepClone", destination.GetType(), new Type[] { source.GetType(), destination.GetType() }, Assembly.GetExecutingAssembly().ManifestModule, true);
                ILGenerator generator = dymMethod.GetILGenerator();

                CreateMerger(source, destination, generator);
                // testar com typof(Delegate)

                var delType = typeof(Func<,,>)
                    .MakeGenericType(source.GetType(), destination.GetType(), destination.GetType());

                myExec = dymMethod.CreateDelegate(delType);
                _cachedILDeep.Add(destination.GetType(), myExec);
            }
            return myExec.DynamicInvoke(source, destination);
        }

        public static void CreateMerger(object source, object destination, ILGenerator generator)
        {
            LocalBuilder cloneVariable = generator.DeclareLocal(destination.GetType());

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stloc, cloneVariable);

            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            foreach (FieldInfo field in source.GetType().GetFields(flags))
            {
                switch (GetCloneTypeForField(field))
                {
                    case CloneType.ShallowCloning:
                        {
                            EmitPassOn(generator, cloneVariable, field);
                            break;
                        }
                    case CloneType.DeepCloning:
                        {
                            if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                            {
                                EmitPassOn(generator, cloneVariable, field);
                            }
                            else if (field.FieldType.IsClass)
                            {
                                CopyReferenceType(generator, cloneVariable, field);
                            }

                            break;
                        }
                    case CloneType.None:
                        {
                            // Do nothing here, field is not cloned.
                        }
                        break;
                }
            }

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
        }

        private static void EmitPassOn(ILGenerator generator, LocalBuilder cloneVariable, FieldInfo field)
        {
            generator.Emit(OpCodes.Ldloc, cloneVariable);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);
            generator.Emit(OpCodes.Stfld, field);
        }

        /// <summary>
        /// Helper method to clone a reference type.
        /// This method clones IList and IEnumerables and other reference types (classes)
        /// Arrays are not yet supported (ex. string[])
        /// </summary>
        /// <param name="generator">IL generator to emit code to.</param>
        /// <param name="cloneVar">Local store wheren the clone object is located. (or child of)</param>
        /// <param name="field">Field definition of the reference type to clone.</param>
        private static void CopyReferenceType(ILGenerator generator, LocalBuilder cloneVar, FieldInfo field)
        {
            if (field.FieldType.IsSubclassOf(typeof(Delegate)))
            {
                return;
            }
            LocalBuilder lbTempVar = generator.DeclareLocal(field.FieldType);

            if (field.FieldType.GetInterface("IEnumerable") != null && field.FieldType.GetInterface("IList") != null)
            {
                if (field.FieldType.IsGenericType)
                {
                    Type argumentType = field.FieldType.GetGenericArguments()[0];
                    Type genericTypeEnum = Type.GetType("System.Collections.Generic.IEnumerable`1[" + argumentType.FullName + "]");

                    ConstructorInfo ci = field.FieldType.GetConstructor(new Type[] { genericTypeEnum });
                    if (ci != null && GetCloneTypeForField(field) == CloneType.ShallowCloning)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, field);
                        generator.Emit(OpCodes.Newobj, ci);
                        generator.Emit(OpCodes.Stloc, lbTempVar);
                        generator.Emit(OpCodes.Ldloc, cloneVar);
                        generator.Emit(OpCodes.Ldloc, lbTempVar);
                        generator.Emit(OpCodes.Stfld, field);
                    }
                    else
                    {
                        ci = field.FieldType.GetConstructor(Type.EmptyTypes);
                        if (ci != null)
                        {
                            generator.Emit(OpCodes.Newobj, ci);
                            generator.Emit(OpCodes.Stloc, lbTempVar);
                            generator.Emit(OpCodes.Ldloc, cloneVar);
                            generator.Emit(OpCodes.Ldloc, lbTempVar);
                            generator.Emit(OpCodes.Stfld, field);
                            CloneList(generator, field, argumentType, lbTempVar);
                        }
                    }
                }
            }
            else
            {
                ConstructorInfo cInfo = field.FieldType.GetConstructor(new Type[] { });
                generator.Emit(OpCodes.Newobj, cInfo);
                generator.Emit(OpCodes.Stloc, lbTempVar);
                generator.Emit(OpCodes.Ldloc, cloneVar);
                generator.Emit(OpCodes.Ldloc, lbTempVar);
                generator.Emit(OpCodes.Stfld, field);
                foreach (FieldInfo fi in field.FieldType.GetFields(System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public))
                {
                    if (fi.FieldType.IsValueType || fi.FieldType == typeof(string))
                    {
                        generator.Emit(OpCodes.Ldloc_1);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, field);
                        generator.Emit(OpCodes.Ldfld, fi);
                        generator.Emit(OpCodes.Stfld, fi);
                    }
                }
            }
        }

        /// <summary>
        /// Makes a deep copy of an IList of IEnumerable
        /// Creating new objects of the list and containing objects. (using default constructor)
        /// And by invoking the deepclone method defined above. (recursive)
        /// </summary>
        /// <param name="generator">IL generator to emit code to.</param>
        /// <param name="listField">Field definition of the reference type of the list to clone.</param>
        /// <param name="typeToClone">Base-type to clone (argument of List<T></param>
        /// <param name="cloneVar">Local store wheren the clone object is located. (or child of)</param>
        private static void CloneList(ILGenerator generator, FieldInfo listField, Type typeToClone, LocalBuilder cloneVar)
        {
            Type genIEnumeratorTyp = Type.GetType("System.Collections.Generic.IEnumerator`1[" + typeToClone.FullName + "]");
            Type genIEnumeratorTypLocal = Type.GetType(listField.FieldType.Namespace + "." + listField.FieldType.Name + "+Enumerator[[" + typeToClone.FullName + "]]");
            LocalBuilder lbEnumObject = generator.DeclareLocal(genIEnumeratorTyp);
            LocalBuilder lbCheckStatement = generator.DeclareLocal(typeof(bool));
            Label checkOfWhile = generator.DefineLabel();
            Label startOfWhile = generator.DefineLabel();
            MethodInfo miEnumerator = listField.FieldType.GetMethod("GetEnumerator");
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, listField);
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
            MethodInfo miAdd = listField.FieldType.GetMethod("Add");
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
        /// Returns the type of cloning to apply on a certain field when in custom mode.
        /// Otherwise the main cloning method is returned.
        /// You can invoke custom mode by invoking the method Clone(T obj)
        /// </summary>
        /// <param name="field">Field to examine</param>
        /// <returns>Type of cloning to use for this field.</returns>
        private static CloneType GetCloneTypeForField(FieldInfo field)
        {
            object[] attributes = field.GetCustomAttributes(typeof(CloneAttribute), true);
            if (attributes == null || attributes.Length == 0)
            {
                //if (!_globalCloneType.HasValue)
                    return CloneType.ShallowCloning;
                //else
                //    return _globalCloneType.Value;
            }
            return (attributes[0] as CloneAttribute).CloneType;
        }

        #endregion
    }
}
