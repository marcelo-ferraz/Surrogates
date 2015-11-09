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
            public class SubClass 
            {
                public int Value { get; set; }
            }

            public int Int { get; set; }
            public string String { get; set; }
            [Clone(CloneType.ShallowCloning)]
            public List<int> Numbers { get; set; }

            public List<SubClass> SubClasses { get; set; }
        }

        public class InheritedClass : BaseClass
        {
            [Clone(Aliases= new [] { "Data" })]
            public DateTime Date { get; set; }

        }

        public class DifferentClass
        {
            public class DifferentSubClass
            {
                public int Value { get; set; }
            }

            public string String { get; set; }
            public DateTime Data { get; set; }

            public List<int> Numbers { get; set; }

            [Clone(CloneType.DeepCloning)]
            public List<DifferentSubClass> SubClasses { get; set; }
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
                CloneHelper.Clone<BaseClass>(@base);

            Assert.AreEqual(@base.String, newValue.String);
            Assert.AreEqual(@base.Int, newValue.Int);
            Assert.AreNotEqual(@base.GetHashCode(), newValue.GetHashCode());            
        }

        [Test]
        public void ClonningWithNullParameter()
        {
            Assert.IsNull(CloneHelper.Clone<BaseClass>(null));
            Assert.IsNull(CloneHelper.Clone<BaseClass>(null, CloneType.ShallowCloning));
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

        [Test]
        public void CloneDifferentObjects()
        {
            var date =
                DateTime.Now;

            var inherited = new InheritedClass 
            {
                Int = 1, 
                String = "something", 
                Date = date, 
                Numbers = new List<int> { 1, 2, 3 }, 
                SubClasses = new List<BaseClass.SubClass> { new BaseClass.SubClass() } 
            };

            var inheritedHashCode =
                inherited.GetHashCode();

            var newValue =
                CloneHelper.Clone<DifferentClass>(inherited);

            Assert.IsNotNullOrEmpty(newValue.String);
            Assert.AreEqual(inherited.String, newValue.String);
            Assert.AreEqual(date, newValue.Data);
            
            
        }
    }
}
