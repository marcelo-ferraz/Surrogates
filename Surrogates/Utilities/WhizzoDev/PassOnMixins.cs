using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Utilities.WhizzoDev
{
    public static class PassOnMixins
    {
        public static R[] ToAnArrayOf<T, R>(this T[] array)
            where R : class
            where T : class
        {
            return PassOn<T>.ToAnArrayOf<R>(array);
        }

        public static R[] ToAnArrayOf<T, R>(this IEnumerable<T> enumerable)
            where R : class
            where T : class
        {
            return PassOn<T>.ToAnArrayOf<R>(enumerable);
        }

        public static IList<R> ToAListOf<T, R>(this T[] array)
            where R : class
            where T : class
        {
            return PassOn<T>.ToAListOf<R>(array);
        }

        public static IList<R> ToAListOf<T, R>(this IEnumerable<T> enumerable)
            where R : class
            where T : class
        {
            return PassOn<T>.ToAListOf<R>(enumerable);
        }

        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as Shallow and/or Deep combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static R To<R>(this object obj)
            where R : class
        {
            return PassOn.To<R>(obj);
        }


        /// <summary>
        /// Clone an object with Deep Cloning or with a custom strategy 
        /// such as Shallow and/or Deep combined (use the CloneAttribute)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <returns>Cloned object.</returns>
        public static object To(this object obj, Type returnType)
        {
            return PassOn<object>.To(obj, returnType);            
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static R To<R>(this object obj, CloneType cloneType = CloneType.Deep)
            where R : class
        {
            return PassOn.To<R>(obj, cloneType); 
        }

        /// <summary>
        /// Clone an object with one strategy (DeepClone or ShallowClone)
        /// </summary>
        /// <param name="obj">Object to perform cloning on.</param>
        /// <param name="cloneType">Type of cloning</param>
        /// <returns>Cloned object.</returns>
        /// <exception cref="InvalidOperationException">When a wrong enum for cloningtype is passed.</exception>
        public static object To(this object obj, Type returnType, CloneType cloneType = CloneType.Deep)
        {
            return PassOn.To(returnType, obj, cloneType); 
        }

        public static T To<T>(this T source, T destination)
            where T : class
        {
            return PassOn<T>.To<T>(source, destination);
        }

        public static R To<T, R>(this T source, R destination)
            where T : class
            where R : class
        {
            return PassOn<T>.To<R>(source, destination);
        }

        public static object To(this object source, object destination)
        {
            return PassOn.To(source, destination);
        }
    }
}
