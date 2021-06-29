using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Dotnet.Integration.Test;
using NuGet.Commands.FuncTest;
using NuGet.PackageManagement.VisualStudio.Test;

namespace DotnetBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<RestoreCommand_PackagesLockFileTests>();
        }
    }
}
