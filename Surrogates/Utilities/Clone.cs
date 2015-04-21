
namespace Surrogates.Utilities
{
    public static class Clone
    {
        private static WhizzoDev.Cloning _replicator;

        static Clone() 
        {
            _replicator = new WhizzoDev.Cloning();
        }

        /// <summary>
        /// Generic cloning method that clones an object using IL.
        /// Only the first call of a certain type will hold back performance.
        /// After the first call, the compiled IL is executed.
        /// </summary>
        /// <typeparam name="T">Type of object to clone</typeparam>
        /// <param name="obj">Object to clone</param>
        /// <returns>Cloned object</returns>
        public static T This<T>(T obj)
        {
            return _replicator.CloneObjectWithILDeep<T>(obj);
        }
        
        public static T IntoTheSecond<T>(T source, T receiver)
        {
            return _replicator.CloneObjectWithILDeep<T>(source, receiver);
        }
    }
}
