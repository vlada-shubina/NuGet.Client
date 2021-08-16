// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Cryptography.X509Certificates;
using NuGet.Common;

namespace NuGet.Packaging.Signing
{
    /// <summary>
    /// Creates and performs cleanup on an <see cref="X509Chain" /> instance.
    /// </summary>
    /// <remarks>
    /// Certificates held by individual X509ChainElement objects should be disposed immediately after use to minimize
    /// finalizer impact.
    /// </remarks>
    public sealed class X509ChainHolder : IDisposable
    {
        private bool _isDisposed;

        public X509Chain Chain { get; }

        public X509ChainHolder()
        {
#if NET5_0_OR_GREATER
            IX509ChainCreator factory = X509ChainCreatorFactory.Create(EnvironmentVariableWrapper.Instance);

            Chain = factory.Create();
#else
            Chain = new X509Chain();
#endif
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
#if !NET45
                //.NET 4.6 added Dispose method to X509Certificate and X509Chain
                foreach (var chainElement in Chain.ChainElements)
                {
                    chainElement.Certificate.Dispose();
                }

                Chain.Dispose();
#endif
                GC.SuppressFinalize(this);

                _isDisposed = true;
            }
        }
    }
}
