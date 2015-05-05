using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.Tests
{
    [Serializable]
    public class Simple
    {
        public Simple() { }
        public Simple(List<int> list)
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

        public virtual string GetDomainName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }
    }
}
