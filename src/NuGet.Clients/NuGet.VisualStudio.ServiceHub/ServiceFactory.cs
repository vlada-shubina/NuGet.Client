using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceHub.Framework;
using Microsoft.ServiceHub.Framework.Services;
using NuGet.VisualStudio.Internal.Contracts;

namespace NuGet.VisualStudio.ServiceHub
{
    public class ServiceFactory : IMultiVersionedServiceFactory
    {
        public ServiceFactory()
        {
            //Debugger.Launch();
            AppContext.SetSwitch("Switch.System.IO.UseLegacyPathHandling", false);
            AppContext.SetSwitch("Switch.System.IO.BlockLongPaths", false);
        }

        public Task<object> CreateAsync(IServiceProvider hostProvidedServices,
            ServiceMoniker serviceMoniker,
            ServiceActivationOptions serviceActivationOptions,
            IServiceBroker serviceBroker,
            AuthorizationServiceClient authorizationServiceClient,
            CancellationToken cancellationToken)
        {
            if (serviceMoniker.Name == NuGetServices.RestoreService.Moniker.Name &&
                serviceMoniker.Version == NuGetServices.RestoreService.Moniker.Version)
            {
                return Task.FromResult<object>(new RestoreService());
            }

            throw new NotSupportedException("The requested service is not supported");
        }

        public ServiceRpcDescriptor GetServiceDescriptor(ServiceMoniker serviceMoniker)
        {
            if (serviceMoniker == NuGetServices.RestoreService.Moniker)
            {
                return NuGetServices.RestoreService;
            }
            else if (serviceMoniker.Name == NuGetServices.RestoreService.Moniker.Name)
            {
                if (serviceMoniker.Version == NuGetServices.RestoreService.Moniker.Version)
                {
                    return NuGetServices.RestoreService;
                }
            }

            throw new NotSupportedException("The requested service is not supported");
        }
    }
}
