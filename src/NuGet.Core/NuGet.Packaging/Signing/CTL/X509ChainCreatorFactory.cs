// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using NuGet.Common;

namespace NuGet.Packaging.Signing
{
    internal static class X509ChainCreatorFactory
    {
        internal const string TrustedRootsFilePath = "NUGET_TRUSTED_ROOTS_FILE_PATH";

        internal static IX509ChainCreator Create(IEnvironmentVariableReader environmentVariableReader)
        {
            if (environmentVariableReader is null)
            {
                throw new ArgumentNullException(nameof(environmentVariableReader));
            }

            string value = environmentVariableReader.GetEnvironmentVariable(TrustedRootsFilePath);

#if NET5_0_OR_GREATER
            if (string.IsNullOrWhiteSpace(value))
            {
                return new DefaultCtlX509ChainCreator();
            }
#else
            return new DefaultCtlX509ChainCreator();
#endif
#if NET5_0_OR_GREATER

            var file = new FileInfo(value);

            return new FallbackCtlX509ChainCreator(file);
#endif
        }
    }
}
