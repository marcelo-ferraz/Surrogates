using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.Tests.Model
{
    public class SimpletonList : List<Simpleton>
    {
        public new virtual Simpleton this[int i]
        {
            get { return base[i]; }
            set { base[i] = value; }
        }

        public new virtual void Add(Simpleton item)
        {
            base.Add(item);
        }

        public new virtual void Insert(int i, Simpleton item)
        {
            base.Insert(i, item);
        }
    }
}
