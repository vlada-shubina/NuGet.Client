// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NuGet.Versioning;

namespace NuGet.VisualStudio.Internal.Contracts
{
    public class TransitiveVersionContextInfo : VersionInfoContextInfo
    {
        public TransitiveVersionContextInfo(NuGetVersion version, long? downloadCount)
            : base(version, downloadCount)
        {
        }

        public override PackageDeprecationMetadataContextInfo? PackageDeprecationMetadata { get => null; internal set { } }

        public static TransitiveVersionContextInfo Create(VersionInfoContextInfo version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            return new TransitiveVersionContextInfo(version.Version, version.DownloadCount)
            {
                PackageSearchMetadata = version.PackageSearchMetadata, // TODO: Do I need to make this transitive
            };
        }
    }
}
