using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Utilities.WhizzoDev
{
    //public static class CloneOrMergeMixins
    //{
    //    public static R[] ToArrayOf<R>(this T[] array)
    //       where R : class
    //    {
    //        if (array == null) { return null; }

    //        var result = (R[])Array.CreateInstance(
    //            typeof(R), array.Length);

    //        for (int i = 0; i < array.Length; i++)
    //        {
    //            result[i] = CloneTo<R>(array[i]);
    //        }

    //        return result;
    //    }

    //    public static R[] ToArrayOf<R>(this IEnumerable<T> enumerable)
    //        where R : class
    //    {
    //        if (enumerable == null) { return null; }

    //        var result = new List<R>();

    //        foreach (var item in enumerable)
    //        {
    //            result.Add(CloneTo<R>(item));
    //        }

    //        return result.ToArray();
    //    }

    //    public static IList<R> ToListOf<R>(this T[] array)
    //        where R : class
    //    {
    //        if (array == null) { return null; }

    //        var result = new List<R>(array.Length);

    //        for (int i = 0; i < array.Length; i++)
    //        {
    //            result.Add(CloneTo<R>(array[i]));
    //        }

    //        return result;
    //    }

    //    public static IList<R> ToListOf<R>(this IEnumerable<T> enumerable)
    //        where R : class
    //    {
    //        if (enumerable == null)
    //        { return null; }

    //        var result = new List<R>();

    //        foreach (var item in enumerable)
    //        {
    //            result.Add(CloneTo<R>(item));
    //        }

    //        return result.ToArray();
    //    }

    //    /// <summary>
    //    /// Clone an object with Deep Cloning or with a custom strategy 
    //    /// such as Shallow and/or Deep combined (use the CloneAttribute)
    //    /// </summary>
    //    /// <param name="obj">Object to perform cloning on.</param>
    //    /// <returns>Cloned object.</returns>
    //    public static T CloneTo<T>(this object obj)
    //        where T : class
    //    {
    //        return (T)Engine.CloneObjectWithILDeep(typeof(T), obj);
    //    }


    //    /// <summary>
    //    /// Clone an object with Deep Cloning or with a custom strategy 
    //    /// such as Shallow and/or Deep combined (use the CloneAttribute)
    //    /// </summary>
    //    /// <param name="obj">Object to perform cloning on.</param>
    //    /// <returns>Cloned object.</returns>
    //    public static object CloneTo(this Type returnType, object obj)
    //    {
    //        return Engine.CloneObjectWithILDeep(returnType, obj);
    //    }

    //    /// <summary>
    //    /// Clone an object with one strategy (DeepClone or ShallowClone)
    //    /// </summary>
    //    /// <param name="obj">Object to perform cloning on.</param>
    //    /// <param name="cloneType">Type of cloning</param>
    //    /// <returns>Cloned object.</returns>
    //    /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
    //    public static T CloneTo<T>(this object obj, CloneType cloneType = CloneType.Deep)
    //        where T : class
    //    {
    //        return (cloneType == CloneType.Shallow) ?
    //            (T)Engine.CloneObjectWithILShallow(typeof(T), obj) :
    //            (T)Engine.CloneObjectWithILDeep(typeof(T), obj);
    //    }

    //    /// <summary>
    //    /// Clone an object with one strategy (DeepClone or ShallowClone)
    //    /// </summary>
    //    /// <param name="obj">Object to perform cloning on.</param>
    //    /// <param name="cloneType">Type of cloning</param>
    //    /// <returns>Cloned object.</returns>
    //    /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
    //    public static object CloneTo(this Type returnType, object obj, CloneType cloneType = CloneType.Deep)
    //    {
    //        return (cloneType == CloneType.Shallow) ?
    //            Engine.CloneObjectWithILShallow(returnType, obj) :
    //            Engine.CloneObjectWithILDeep(returnType, obj);
    //    }

    //    public static T MergeTo<T>(this T source, T destination)
    //    {
    //        return (T)Engine.MergeWithILDeep(source, destination);
    //    }

    //    public static object MergeTo(this object source, object destination)
    //    {
    //        return CloneOrMerge<object>.MergeTo(source, destination);
    //    }
    //}

    public static class CloneOrMerge<T>
        where T : class
    {
        public static R[] ToArrayOf<R>(T[] array)
            where R : class
        {
            if (array == null) { return null; }

            var result = (R[])Array.CreateInstance(
                typeof(R), array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = CloneTo<R>(array[i]);
            }

            return result;
        }

        public static R[] ToArrayOf<R>(IEnumerable<T> enumerable)
            where R : class
        {
            if (enumerable == null) { return null; }

            var result = new List<R>();

            foreach (var item in enumerable)
            {
                result.Add(CloneTo<R>(item));
            }

            return result.ToArray();
        }

        public static IList<R> ToListOf<R>(T[] array)
            where R : class
        {
            if (array == null) { return null; }

            var result = new List<R>(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                result.Add(CloneTo<R>(array[i]));
            }

            return result;
        }

        public static IList<R> ToListOf<R>(IEnumerable<T> enumerable)
            where R : class
        {
            if (enumerable == null)
            { return null; }

            var result = new List<R>();

            foreach (var item in enumerable)
            {
                result.Add(CloneTo<R>(item));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as Shallow and/or Deep combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static R CloneTo<R>(object obj)
        {
            return (R) Engine.CloneObjectWithILDeep(typeof(R), obj);
        }


        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as Shallow and/or Deep combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static object CloneTo(Type returnType, object obj)
        {
            return Engine.CloneObjectWithILDeep(returnType, obj);
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static R CloneTo<R>(object obj, CloneType cloneType = CloneType.Deep)
        {
            return (cloneType == CloneType.Shallow) ?
                (R)Engine.CloneObjectWithILShallow(typeof(T), obj) :
                (R)Engine.CloneObjectWithILDeep(typeof(T), obj);
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static object CloneTo(Type returnType, object obj, CloneType cloneType = CloneType.Deep)
        {
            return (cloneType == CloneType.Shallow) ?
                Engine.CloneObjectWithILShallow(returnType, obj) :
                Engine.CloneObjectWithILDeep(returnType, obj);
        }
        
        public static T MergeTo(T source, T destination)
        {
            return (T)Engine.MergeWithILDeep(source, destination);
        }

        public static R MergeTo<R>(T source, R destination)
        {
            return (R)Engine.MergeWithILDeep(source, destination);
        }

        public static object MergeTo(object source, object destination)
        {
            return Engine.MergeWithILDeep(source, destination);
        }
    }
}
