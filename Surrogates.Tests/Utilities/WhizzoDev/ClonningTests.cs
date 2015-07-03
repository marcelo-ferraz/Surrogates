using NUnit.Framework;
using Surrogates.Utilities.WhizzoDev;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Utilities.WhizzoDev
{
    [TestFixture]
    public class ClonningTests
    {
        public class BaseClass
        {
            public int Int { get; set; }
            public string String { get; set; }
        }

        public class InheritedClass : BaseClass
        {
            public DateTime Date { get; set; }
        }

        [Test]
        public void MergingTest()
        { 
            var date = 
                DateTime.Now;

            var @base = 
                new BaseClass { Int = 1, String = "something" };
            
            var inherited = 
                new InheritedClass { Date = date };

            var inheritedHashCode =
                inherited.GetHashCode();

            Assert.IsNullOrEmpty(inherited.String);

            var newValue = 
                CloneHelper.Merge(@base, inherited);

            Assert.IsNotNullOrEmpty(inherited.String);
            Assert.AreEqual(1, inherited.Int);
            Assert.AreEqual(date, inherited.Date);
            Assert.AreEqual(inheritedHashCode, inherited.GetHashCode());
            Assert.AreEqual(inheritedHashCode, newValue.GetHashCode());
        }

        [Test]
        public void ClonningTest()
        {
            var @base =
                new BaseClass { Int = 1, String = "something" };

            var newValue =
                CloneHelper.Clone(@base);

            Assert.AreEqual(@base.String, newValue.String);
            Assert.AreEqual(@base.Int, newValue.Int);
            Assert.AreNotEqual(@base.GetHashCode(), newValue.GetHashCode());            
        }

        [Test]
        public void ClonningWithNullParameter()
        {
            Assert.IsNull(CloneHelper.Clone(null));
            Assert.IsNull(CloneHelper.Clone(null, CloneType.ShallowCloning));
        }

        [Test]
        public void MergingWithNullParameter()
        {
            var date =
                DateTime.Now;

            var @base =
                new BaseClass { Int = 1, String = "something" };

            var inherited =
                new InheritedClass { Date = date };

            Assert.IsNull(CloneHelper.Merge(null, null));
            Assert.AreEqual(CloneHelper.Merge(null, inherited).GetHashCode(), inherited.GetHashCode());

            var mergedValue = CloneHelper.Merge(@base, null);

            Assert.AreEqual(@base.String, mergedValue.String);
            Assert.AreEqual(@base.Int, mergedValue.Int);
            Assert.AreEqual(@base.GetHashCode(), mergedValue.GetHashCode());
        }
    }
}
