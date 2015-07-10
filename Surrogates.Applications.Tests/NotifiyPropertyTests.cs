
using NUnit.Framework;
using System;
using Surrogates.Applications;
using System.Collections.Generic;
using Surrogates.Applications.Tests.Model;
using Surrogates.Applications.NotifyChanges;
using Surrogates.Utilities;

namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class NotifiyPropertyTests : AppTestsBase
    {
        private string[] _values = new string[3];

        private void Listen1(SimpletonList list, Simpleton item, object value)
        {
            var index = 
                int.Parse(item.Text);

            _values[index] = (string) value;
        }

        private void TextsAreEqual(Simpleton item, object value)
        {
            Assert.AreEqual(item.Text, value);
        }

        [Test]
        public void SimpleNotifyBeforeTest()
        {
            Container.Map(m => m
                .From<SimpletonList>()
                .Apply
                .NotifyChanges<SimpletonList, Simpleton>(before: this.Listen1))
            .Save();

            var proxyList = Container
                .Invoke<SimpletonList>();

            var simpleton =
                new Simpleton() { Text = "0", _fieldValue = 258 };

            proxyList.Add(simpleton);
            proxyList.Add(null);
            proxyList.Add(null);

            proxyList[1] = new Simpleton() { Text = "1" };
            proxyList.Insert(2, new Simpleton() { Text = "2" });

            proxyList[0].Text = "Index zero, changed!";
            proxyList[1].Text = "Index one, changed!";
            proxyList[2].Text = "Index two, changed!";

            Assert.IsNotNull(_values[0]);
            Assert.AreEqual(_values[0], "Index zero, changed!");
            Assert.IsNotNull(_values[1]);
            Assert.AreEqual(_values[1], "Index one, changed!");
            Assert.IsNotNull(_values[2]);
            Assert.AreEqual(_values[2], "Index two, changed!");
        }

        [Test]
        public void SimpleNotifyAfterTest()
        {
            Container.Map(m => m
                .From<SimpletonList>()
                .Apply
                .NotifyChanges<SimpletonList, Simpleton>(after: (l, i, v) => TextsAreEqual(i, v)))
            .Save();

            var proxyList = Container
                .Invoke<SimpletonList>();

            var simpleton =
                new Simpleton() { Text = "0", _fieldValue = 258 };

            proxyList.Add(simpleton);
            proxyList.Add(null);
            proxyList.Add(null);

            proxyList[1] = new Simpleton() { Text = "1" };
            proxyList.Insert(2, new Simpleton() { Text = "2" });

            proxyList[0].Text = "Index zero, changed!";
            proxyList[1].Text = "Index one, changed!";
            proxyList[2].Text = "Index two, changed!";
        }
    }
}
