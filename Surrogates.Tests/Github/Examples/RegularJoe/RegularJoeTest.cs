using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Surrogates.Tests.Github.Examples.RegularJoe
{
    [TestFixture]
    public class RegularJoeTest
    {
        private SurrogatesContainer _container = new SurrogatesContainer();

        [SetUp]
        public void Map()
        {
            _container.Map(m => m
                .Throughout<RegularJoe>()
                .Replace
                .ThisProperty(d => d.Age)
                .Accessors(a =>
                    a.Getter.Using<TwoKids>().ThisMethod<int>(d => () => 3))).Save();
        }

        [Test]
        public void Test()
        {
            var joeWithKids =
                _container.Invoke<RegularJoe>();

            var singleJoe = 
                new RegularJoe();

            joeWithKids.Age = 18;
            singleJoe.Age = 18;

            Assert.AreNotEqual(singleJoe.Age, joeWithKids.Age);
            Assert.AreEqual(18, singleJoe.Age);
            Assert.AreEqual(38, joeWithKids.Age);
        }
    }
}
