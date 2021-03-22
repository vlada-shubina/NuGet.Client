// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NuGet.Commands;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.ProjectManagement;
using NuGet.ProjectModel;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.VisualStudio.Internal.Contracts;

namespace NuGet.VisualStudio.ServiceHub
{
    internal class RestoreService : IRestoreService
    {
        public async Task<string> RestoreAsync(string dgSpecJson, string solutionRoot, CancellationToken cancellationToken)
        {
            DependencyGraphSpec dgSpec;
            using (var stringReader = new StringReader(dgSpecJson))
            {
                dgSpec = DependencyGraphSpec.Load(stringReader);
            }

            ISolutionManager solutionManager = null;

            ISettings settings = NuGet.Configuration.Settings.LoadDefaultSettings(solutionRoot);

            var cacheContext = new DependencyGraphCacheContext(NullLogger.Instance, settings);

            var providerCache = new RestoreCommandProvidersCache();
            Action<SourceCacheContext> cacheModifier = (context) => { };

            var packageSources = SettingsUtility.GetEnabledSources(settings).ToList();
            var sources = new List<SourceRepository>(packageSources.Count);
            for (int i = 0; i < packageSources.Count; i++)
            {
                var sourceRepository = Repository.Factory.GetCoreV3(packageSources[i]);
                sources.Add(sourceRepository);
            }

            Guid operationId = Guid.NewGuid();
            bool forceRestore = false;
            bool isRestoreOriginalAction = false;
            IReadOnlyList<IAssetsLogMessage> additionalMessages = null;

            IReadOnlyList<RestoreSummary> restoreSummaries =
                await DependencyGraphRestoreUtility.RestoreAsync(
                    solutionManager,
                    dgSpec,
                    cacheContext,
                    providerCache,
                    cacheModifier,
                    sources,
                    operationId,
                    forceRestore,
                    isRestoreOriginalAction,
                    additionalMessages,
                    NullLogger.Instance,
                    cancellationToken);

            var result = JsonConvert.SerializeObject(restoreSummaries);

            return result;
        }
    }
}
