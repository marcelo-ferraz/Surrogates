using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace Surrogates.Applications.ExecutingElsewhere
{
    [Serializable]
    public class ExecuteInOtherDomain
    {
        [Serializable]
        public class State
        {
            public SecurityZone SecurityZone { get; set; }

            public IPermission[] Permissions { get; set; }

            public string Name { get; set; }
        }
        [Serializable]
        public class Interceptor
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

            private void TryInitDomain(ExecuteInOtherDomain.State state)
            {
                if (_thisDomain != null) { return; }

                var evidence = new Evidence(
                    new[] { new Zone(state.SecurityZone) },
                    null);

                var setup =
                    new AppDomainSetup
                    {
                        ApplicationBase = Environment.CurrentDirectory,
                        ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                        LoaderOptimization = LoaderOptimization.MultiDomainHost
                    };

                var permissionSet =
                    new PermissionSet(PermissionState.Unrestricted);

                for (int i = 0; i < state.Permissions.Length; i++)
                {
                    permissionSet.AddPermission(state.Permissions[i]);
                }

                _thisDomain = AppDomain
                    .CreateDomain(state.Name, evidence, setup, permissionSet);

                //_thisDomain.Load()
            }

            public object Execute(ExecuteInOtherDomain.State s_State, Delegate s_method)
            {
                TryInitDomain(s_State);

                var worker = (Worker)this._thisDomain.CreateInstanceAndUnwrap(
                    this.GetType().Assembly.FullName,
                    typeof(Worker).FullName);

                worker.Action = s_method;

                worker.Execute();

                return worker.Result;
            }
        }
    }
}