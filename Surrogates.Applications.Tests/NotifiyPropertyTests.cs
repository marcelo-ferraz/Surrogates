
using NUnit.Framework;
using System;
using Surrogates.Applications;
using System.Collections.Generic;
using Surrogates.Applications.Tests.Model;

namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class NotifiyPropertyTests : AppTestsBase
    {
        private void Listen(SimpletonList arg1, Simpleton arg2, object arg3)
        {
            System.Diagnostics.Debugger.Break();
        }

        [Test]
        public void SimpleNotifyTest()
        {
            Container.Map(m => m
                .From<SimpletonList>()
                .Apply
                .NotifyChanges<SimpletonList, Simpleton>(this.Listen))
            .Save();

            var proxyList = Container
                .Invoke<SimpletonList>();

            proxyList.Add(new Simpleton() { Text = "0", _fieldValue = 258 });
            proxyList[1] = new Simpleton() { Text = "1" };
            proxyList.Insert(2, new Simpleton() { Text = "2" });
        }





        public void Merge1(Simpleton source, Simpleton destination)
        {           
            destination._fieldRef = source._fieldRef;

            destination.PropListRef = source.PropListRef;
        }
    }
}
