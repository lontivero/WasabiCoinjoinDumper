using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WasabiRealFeeCalc
{
    internal enum ServiceState{
        None,
        Running,
        Stopped,
        Stopping
    }

    abstract class ServiceBase
    {
        private ServiceState _state;

        protected ServiceBase()
        {
            _state = ServiceState.None; 
        }

        public virtual void Start()
        {
            _state = ServiceState.Running;
            Task.Run(async () => await ProcessAsync());
        }

        private async Task ProcessAsync()
        {
            try
            {
                while(_state == ServiceState.Running)
                {
                    await DoProcess();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _state = ServiceState.Stopped;
            }
        }

        public abstract Task DoProcess();
    }
}