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

        public string Text { get; set; }

        Random _rnd = new Random();
        public virtual int GetRandom()
        {
            return _rnd.Next();
        }

        public virtual object GetNewObject()
        {
            return new object();
        }
    }
}
