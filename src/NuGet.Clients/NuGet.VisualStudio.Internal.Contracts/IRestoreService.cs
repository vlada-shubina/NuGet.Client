// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace NuGet.VisualStudio.Internal.Contracts
{
    public interface IRestoreService
    {
        public Task<string> RestoreAsync(string dgSpecJson, string solutionRoot, CancellationToken cancellationToken);
    }
}
