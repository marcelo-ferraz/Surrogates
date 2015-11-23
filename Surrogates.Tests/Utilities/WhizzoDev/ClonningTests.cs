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
            [Clone(CloneType.Shallow)]
            public List<int> Numbers { get; set; }

            public List<SubClass> List { get; set; }
            public List<SubClass> List2Array { get; set; }
            public SubClass[] Array { get; set; }
            public SubClass[] Array2List { get; set; }
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

            public List<DifferentSubClass> List { get; set; }
            public DifferentSubClass[] List2Array { get; set; }
            public DifferentSubClass[] Array { get; set; }
            public List<DifferentSubClass> Array2List { get; set; }
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
                PassOn<BaseClass>.To(@base, inherited);

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
            //Engine.SaveMethod(typeof(BaseClass), @base);
            var newValue =
                PassOn.To<BaseClass>(@base);

            Assert.AreEqual(@base.String, newValue.String);
            Assert.AreEqual(@base.Int, newValue.Int);
            Assert.AreNotEqual(@base.GetHashCode(), newValue.GetHashCode());            
        }

        [Test]
        public void ClonningWithNullParameter()
        {
            Assert.IsNull(PassOn.To<BaseClass>(null));
            Assert.IsNull(PassOn.To<BaseClass>(null, CloneType.Shallow));
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

            Assert.IsNull(PassOn<BaseClass>.To(null, null));
            Assert.AreEqual(PassOn<BaseClass>.To(null, inherited).GetHashCode(), inherited.GetHashCode());

            var mergedValue = PassOn<BaseClass>.To<InheritedClass>(@base, null);

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
                List = new List<BaseClass.SubClass> { new BaseClass.SubClass() { Value = 1 } },
                List2Array = new List<BaseClass.SubClass> { new BaseClass.SubClass() { Value = 2 } },
                Array = new BaseClass.SubClass[] { new BaseClass.SubClass() { Value = 3 } },
                //Array2List = new BaseClass.SubClass[] { new BaseClass.SubClass() { Value = 4 } }, 
            };

            var inheritedHashCode =
                inherited.GetHashCode();

            var diffValue =
                PassOn.To<DifferentClass>(inherited);

            Assert.IsNotNullOrEmpty(diffValue.String);
            Assert.AreEqual(inherited.String, diffValue.String);
            Assert.AreEqual(date, diffValue.Data);
        }

        public static ClonningTests.DifferentClass Copy(ClonningTests.InheritedClass inheritedClass)
        {
            var differentClass = new ClonningTests.DifferentClass();
            string heu = "heeey";

            if (inheritedClass.Date != default(DateTime))
            {
                differentClass.Data = inheritedClass.Date;
            }

            if (inheritedClass.String != default(string))
            {
                differentClass.String = inheritedClass.String;
            }

            differentClass.Numbers = inheritedClass.Numbers;
            //differentClass.SubClasses = Clone<BaseClass.SubClass>.ToArrayOf<DifferentClass.DifferentSubClass>(inheritedClass.SubClasses);

            return differentClass;
        }
    
    }
}
