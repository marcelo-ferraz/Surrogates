using NUnit.Framework;
using System;

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
                .From<RegularJoe>("JoeWithKids")
                .Replace
                .This(d => d.Age)
                .Accessors(a =>
                    a.Getter.Using<TwoKids>(d => (Func<int, int>) d.AddTo))                
                .And
                .Replace
                .Method("Calculate")
                .Using<TwoKids>("NewMethod"))
                .Save();
        }

        [Test]
        public void Test()
        {
            var joeWithKids = (RegularJoe)
                _container.Invoke("JoeWithKids");

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
