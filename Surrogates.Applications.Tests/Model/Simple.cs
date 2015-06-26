using System;
using System.Collections.Generic;
using System.Threading;

namespace Surrogates.Applications.Tests
{
    [Serializable]
    public class Simpleton
    {
        public Simpleton() { }
        public Simpleton(List<int> list)
        {
            this.List = list;
        }
        public virtual string Text { get; set; }

        public virtual List<int> List { get; set; }

        public virtual int GetFromList(int index)
        {
            return List[index];
        }

        public virtual void Add2List(int val)
        {
            List.Add(val);
        }

        public virtual void Set(string text)
        {
            this.Text = text;
        }

        public virtual string GetDomainName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }
        public virtual string GetThreadName()
        {
            return Thread.CurrentThread.Name;
        }

        Random _rnd = new Random();
        public virtual int GetRandom()
        {
            return _rnd.Next();
        }

        public virtual object GetNewObject()
        {
            return new object();
        }


        public int _fieldValue;
        public object _fieldRef;

        public virtual int PropValue { get; set; }
        public virtual object PropRef { get; set; }
        public virtual IList<int> PropListVal { get; set; }
        public virtual IList<Dummy> PropListRef { get; set; }
    }

    public class Dummy
    {
        public int _fieldValue;

        public object _fieldRef;

        public int PropValue { get; set; }
        public object PropRef { get; set; }
        public IList<int> PropListVal { get; set; }
        public IList<Dummy> PropListRef { get; set; }
    }
}
