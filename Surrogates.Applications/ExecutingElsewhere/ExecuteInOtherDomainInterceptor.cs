using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace Surrogates.Applications.ExecutingElsewhere
{
    public class ExecuteInOtherDomainState
    {
        public SecurityZone SecurityZone { get; set; }

        public IPermission[] Permissions { get; set; }

        public string Name { get; set; }
    }

    public class ExecuteInOtherDomainInterceptor<R>
    {

        [Serializable]
        public class Worker : MarshalByRefObject
        {
            public Delegate Action { get; set; }

            public object[] Params { get; set; }

            public object Result { get; set; }

            public void Execute()
            {
                this.Result = this.Action.DynamicInvoke(this.Params);
            }
        }

        private AppDomain _thisDomain;

        private void TryInitDomain(ExecuteInOtherDomainState state)
        {
            if (_thisDomain != null) { return; }

            var evidence = new Evidence(
                new[] { new Zone(state.SecurityZone) },
                null);

            var setup =
                new AppDomainSetup
                {
                    ApplicationBase = Environment.CurrentDirectory
                };

            var permissionSet = 
                new PermissionSet(PermissionState.Unrestricted);

            for (int i = 0; i < state.Permissions.Length; i++)
            {
                permissionSet.AddPermission(state.Permissions[i]);
            }

            _thisDomain = AppDomain
                .CreateDomain(state.Name, evidence, setup, permissionSet);
        }

        public R Execute(ExecuteInOtherDomainState s_State, Delegate s_method)
        {
            TryInitDomain(s_State);

            var worker = (Worker)this._thisDomain.CreateInstanceAndUnwrap(
                this.GetType().Assembly.FullName,
                typeof(Worker).FullName);
            //add serializable attribute somehow
            worker.Action = s_method;

            worker.Execute();

            return (R)worker.Result;
        }
    }
}