// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio.Implementation.Resources;

namespace NuGet.VisualStudio
{
    internal class ExtensionManagerShim
    {
        private readonly IVsExtensionManager _extensionManager;

        public ExtensionManagerShim(object extensionManager, Action<string> errorHandler)
        {
            _extensionManager = extensionManager as IVsExtensionManager ?? Package.GetGlobalService(typeof(IVsExtensionManager)) as IVsExtensionManager;

            if (_extensionManager == null)
            {
                errorHandler?.Invoke(VsResources.ExtensionManagerShim_CouldNotLoadService);
            }
        }

        public bool TryGetExtensionInstallPath(string extensionId, out string installPath)
        {
            installPath = null;

            IInstalledExtension ext = _extensionManager?.GetInstalledExtension(extensionId);
            if (ext != null)
            {
                installPath = ext.InstallPath;
                return true;
            }

            return false;
        }
    }
}
