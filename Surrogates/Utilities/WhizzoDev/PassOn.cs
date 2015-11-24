using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Utilities.WhizzoDev
{
    public static class PassOn
    {        
        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as Shallow and/or Deep combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static object To(Type returnType, object obj)
        {
            return PassOnEngine.CloneObjectWithILDeep(returnType, obj);
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static object To(Type returnType, object obj, CloneType cloneType = CloneType.Deep)
        {
            return (cloneType == CloneType.Shallow) ?
                PassOnEngine.CloneObjectWithILShallow(returnType, obj) :
                PassOnEngine.CloneObjectWithILDeep(returnType, obj);
        }

        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as Shallow and/or Deep combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static R To<R>(object obj)
        {
            return (R)PassOnEngine.CloneObjectWithILDeep(typeof(R), obj);
        }

        public static object To(object source, object destination)
        {
            return PassOnEngine.MergeWithILDeep(source, destination);
        }
        
        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static R To<R>(object obj, CloneType cloneType = CloneType.Deep)
        {
            return (cloneType == CloneType.Shallow) ?
                (R)PassOnEngine.CloneObjectWithILShallow(typeof(R), obj) :
                (R)PassOnEngine.CloneObjectWithILDeep(typeof(R), obj);
        }
    }

    public static class PassOn<T>        
    {
        public static R[] ToAnArrayOf<R>(T[] array)
            where R : class
        {
            if (array == null) { return null; }

            var result = (R[])Array.CreateInstance(
                typeof(R), array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].To<R>();
            }

            return result;
        }

        public static R[] ToAnArrayOf<R>(IEnumerable<T> enumerable)            
        {
            if (enumerable == null) { return null; }

            var result = new List<R>();

            foreach (var item in enumerable)
            {
                result.Add(item.To<R>());
            }

            return result.ToArray();
        }

        public static IList<R> ToAListOf<R>(T[] array)
            where R : class
        {
            if (array == null) { return null; }

            var result = new List<R>(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                result.Add(array[i].To<R>());
            }

            return result;
        }

        public static IList<R> ToAListOf<R>(IEnumerable<T> enumerable)            
        {
            if (enumerable == null)
            { return null; }

            var result = new List<R>();

            foreach (var item in enumerable)
            {
                result.Add(item.To<R>());
            }

            return result.ToArray();
        }

        public static T To(T source, T destination)
        {
            return (T)PassOnEngine.MergeWithILDeep(source, destination);
        }


        public static R To<R>(T source, R destination)
        {
            return (R)PassOnEngine.MergeWithILDeep(source, destination);
        }
    }
}
