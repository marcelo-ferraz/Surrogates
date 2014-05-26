using NUnit.Framework;
using Surrogates.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests._Properties
{
    [TestFixture]
    public class Property_ReplaceTests
    {
        [Test]
        public void WithVoid()
        {
            var container = 
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.Id)
                .Accessors(a => a
                    .Getter.Using<InterferenceObject>().ThisMethod<int, int>(d => d.Int_2_WithField)
                    //.And
                    //.Setter.Using<InterferenceObject>().ThisMethod<int>(d => d.Int_2_ParameterLess)
                    )
                ).Save() ;



         }
    }
}
