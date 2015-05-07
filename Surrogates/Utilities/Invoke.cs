using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Utilities
{
    public class Invoke
    {        
        /// <summary>
        /// Just a lazy implementation for the Activator.CreateInstance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object From(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }
    }
}
