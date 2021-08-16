// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET5_0_OR_GREATER

using System.Security.Cryptography.X509Certificates;
using NuGet.Packaging.Signing;
using Xunit;

namespace NuGet.Packaging.Test
{
    public class DefaultCtlX509ChainCreatorTests
    {
        [Fact]
        public void Create_Always_CreatesDefaultX509Chain()
        {
            var creator = new DefaultCtlX509ChainCreator();

            using (X509Chain chain = creator.Create())
            {
                Assert.Equal(X509ChainTrustMode.System, chain.ChainPolicy.TrustMode);
            }
        }
    }
}
#endif
