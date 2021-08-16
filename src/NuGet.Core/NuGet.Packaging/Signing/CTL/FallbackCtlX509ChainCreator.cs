// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET5_0_OR_GREATER

using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace NuGet.Packaging.Signing
{
    internal sealed class FallbackCtlX509ChainCreator : IX509ChainCreator
    {
        private readonly FileInfo _file;

        internal FallbackCtlX509ChainCreator(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!string.Equals(".p7b", file.Extension, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        Strings.InvalidP7bFileExtension,
                        file.FullName),
                    nameof(file));
            }

            file.Refresh();

            if (!file.Exists)
            {
                throw new FileNotFoundException(
                    string.Format(CultureInfo.CurrentCulture,
                        Strings.P7bFileNotFound,
                        file.FullName),
                    file.FullName);
            }

            _file = file;
        }

        public X509Chain Create()
        {
            var certificates = new X509Certificate2Collection();

            certificates.Import(_file.FullName);

            var x509Chain = new X509Chain();

            x509Chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            x509Chain.ChainPolicy.CustomTrustStore.AddRange(certificates);

            return x509Chain;
        }
    }
}
#endif
