using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.Validators
{
    public class Validators<T>
    {
        private PropertiesValidator<T> _propertyValidators; 

        private ParametersValidator<T> _paramsValidators; 

        public Validators()
        {
            _propertyValidators = new PropertiesValidator<T>(owner: this);
            _paramsValidators = new ParametersValidator<T>(owner: this);
        }

        public PropertiesValidator<T> Property { get { return _propertyValidators; } }

        public ParametersValidator<T> Parameters { get { return _paramsValidators; } }

        internal IDictionary<Tuple<Type, string>, Action<T>> Validation4Methods { get; set; }
        internal IDictionary<T, Action<T>> Validation4Properties { get; set; }
    }
}
