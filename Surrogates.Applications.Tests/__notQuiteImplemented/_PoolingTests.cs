using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surrogates.Applications.Tests
{
    public class Disposable : IDisposable
    {
        public virtual void Dispose()
        {

        }
    }

    public class PoolableDummy
    {
        public PoolableDummy()
        {
            HashCodes = new List<int>();
        }

        public List<int> HashCodes { get; set; }
        public virtual Disposable Disposable { get; set; }
    }

    public class _PoolingTests : AppTestsBase
    {
        [Test]
        public void SimplePooling()
        {
            Container.Map(m =>
                m.From<PoolableDummy>()
                .Apply
                .Pooling<PoolableDummy, Disposable>("Disposable")).Save();

            var proxy = Container
                .Invoke<PoolableDummy>();

            //Parallel.For(0, 10,
            //    i =>
            //    {
                    using (var disp = proxy.Disposable)
                    {
                        var hashCode =
                            disp.GetHashCode();

                        if (proxy.HashCodes.Contains(hashCode))
                        { proxy.HashCodes.Add(hashCode); }
                    }
               // });

            Assert.AreEqual(5, proxy.HashCodes.Count);
        }
    }
}
