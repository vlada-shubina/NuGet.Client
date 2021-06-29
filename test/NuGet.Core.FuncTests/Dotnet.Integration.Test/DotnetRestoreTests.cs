// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using FluentAssertions;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.ProjectModel;
using NuGet.Test.Utility;
using Test.Utility.Signing;
using Xunit;
using static NuGet.Frameworks.FrameworkConstants;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Dotnet.Integration.Test
{

    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    [Collection("Dotnet Integration Tests")]
    public class DotnetRestoreTests
    {
        private const string OptInPackageVerification = "DOTNET_NUGET_SIGNATURE_VERIFICATION";
        private const string OptInPackageVerificationTypo = "DOTNET_NUGET_SIGNATURE_VERIFICATIOn";

        private MsbuildIntegrationTestFixture _msbuildFixture;

        public DotnetRestoreTests(MsbuildIntegrationTestFixture fixture)
        {
            _msbuildFixture = fixture;
        }

        //[GlobalSetup]
        //public void GlobalSetup()
        //{
        //    data = new int[N]; // executed once per each N value
        //}


        [Benchmark(Description = "Test0:OldImplementation", Baseline = true)]
        [PlatformFact(Platform.Windows)]
        public async Task DotnetRestore_SameNameSameKeyProjectPackageReferencing_Succeeds()
        {
            using (var pathContext = _msbuildFixture.CreateSimpleTestPathContext())
            {
                // Set up solution, and project
                var solution = new SimpleTestSolutionContext(pathContext.SolutionRoot);
                var projFramework = FrameworkConstants.CommonFrameworks.Net462;
                var projectPackageName = "projectA";
                var projectA = SimpleTestProjectContext.CreateNETCore(
                   projectPackageName,
                   pathContext.SolutionRoot,
                   projFramework);
                var projectIntermed = SimpleTestProjectContext.CreateNETCore(
                   "projectIntermed",
                   pathContext.SolutionRoot,
                   projFramework);
                var projectMain = SimpleTestProjectContext.CreateNETCore(
                   "projectMain",
                   pathContext.SolutionRoot,
                   projFramework);

                //Setup packages and feed
                var packageA = new SimpleTestPackageContext()
                {
                    Id = projectPackageName,
                    Version = "1.0.0"
                };

                await SimpleTestPackageUtility.CreateFolderFeedV3Async(
                    pathContext.PackageSource,
                    PackageSaveMode.Defaultv3,
                    packageA);

                //add the packe to the project
                projectIntermed.AddPackageToAllFrameworks(packageA);
                projectMain.AddProjectToAllFrameworks(projectA);
                projectMain.AddProjectToAllFrameworks(projectIntermed);
                solution.Projects.Add(projectA);
                solution.Projects.Add(projectIntermed);
                solution.Projects.Add(projectMain);
                solution.Create(pathContext.SolutionRoot);

                // Act
                var args = $" --source \"{pathContext.PackageSource}\" ";
                var reader = new LockFileFormat();

                var projdir = Path.GetDirectoryName(projectA.ProjectPath);
                var projfilename = Path.GetFileNameWithoutExtension(projectA.ProjectName);
                _msbuildFixture.RestoreProject(projdir, projfilename, args);
                Assert.True(File.Exists(projectA.AssetsFileOutputPath));

                projdir = Path.GetDirectoryName(projectIntermed.ProjectPath);
                projfilename = Path.GetFileNameWithoutExtension(projectIntermed.ProjectName);
                _msbuildFixture.RestoreProject(projdir, projfilename, args);
                Assert.True(File.Exists(projectIntermed.AssetsFileOutputPath));
                var lockFile = reader.Read(projectIntermed.AssetsFileOutputPath);
                IList<LockFileTargetLibrary> libraries = lockFile.Targets[0].Libraries;
                Assert.True(libraries.Any(l => l.Type == "package" && l.Name == projectA.ProjectName));

                projdir = Path.GetDirectoryName(projectMain.ProjectPath);
                projfilename = Path.GetFileNameWithoutExtension(projectMain.ProjectName);
                _msbuildFixture.RestoreProject(projdir, projfilename, args);
                Assert.True(File.Exists(projectMain.AssetsFileOutputPath));
                lockFile = reader.Read(projectMain.AssetsFileOutputPath);
                var errors = lockFile.LogMessages.Where(m => m.Level == LogLevel.Error);
                var warnings = lockFile.LogMessages.Where(m => m.Level == LogLevel.Warning);
                Assert.Equal(0, errors.Count());
                Assert.Equal(0, warnings.Count());
                libraries = lockFile.Targets[0].Libraries;
                Assert.Equal(2, libraries.Count);
                Assert.True(libraries.Any(l => l.Type == "project" && l.Name == projectA.ProjectName));
                Assert.True(libraries.Any(l => l.Type == "project" && l.Name == projectIntermed.ProjectName));
            }
        }

        private static SimpleTestPackageContext CreateNetstandardCompatiblePackage(string id, string version)
        {
            var pkgX = new SimpleTestPackageContext(id, version);
            pkgX.Files.Clear();
            pkgX.AddFile($"lib/netstandard2.0/x.dll");
            return pkgX;
        }
    }
}
