using NUnit.Framework;
using Surrogates.Applications.Interlocking;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class InterlockingTests : AppTests
    {
        [Test]
        public void SimpleLock()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .ReadAndWrite(s => (Func<int, int>)s.GetFromList, s => (Action<int>)s.Add2List))
                .Save();

            var max = 15000000;

            var proxy = Container
                .Invoke<Simple>(args: new List<int>(capacity: max));

            var simple = new Simple(new List<int>(capacity: max));


            Action simpleTask =
                () =>
                    Parallel.For(0, max, i => simple.Add2List(i));

            Action safeTask =
                () =>
                    Parallel.For(0, max, i => proxy.Add2List(i));

            var tasks =
                new[] {
                    Task.Run(simpleTask),
                    Task.Run(safeTask)
                };

            Task.WaitAll(tasks);

            int count = 0;

            simple.List.Sort();
            proxy.List.Sort();

            while (simple.List.Count >= count && proxy.List.Count >= count && simple.GetFromList(count) == proxy.GetFromList(count))
            { count++; }

            Assert.AreNotEqual(count, max - 1);
        }
    }
}