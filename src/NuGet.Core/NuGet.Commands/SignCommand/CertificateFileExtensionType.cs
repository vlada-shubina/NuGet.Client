// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Commands
{
    internal enum CertificateFileExtensionType
    {
        /// <summary>
        /// According to https://github.com/dotnet/runtime/issues/51123#issuecomment-817955763, the X509Certificate2 constructor doesn't support .pem with a private key.
        /// It only pays attention to certificate part. If we want to use the private key, we have to use x509certificate2.createfrompemfile method.
        /// </summary>
        Pem = 0,

        /// <summary>
        /// Other certificate file extension types that X509Certificate2 constructor supports.
        /// </summary>
        Others = 1
    }
}
