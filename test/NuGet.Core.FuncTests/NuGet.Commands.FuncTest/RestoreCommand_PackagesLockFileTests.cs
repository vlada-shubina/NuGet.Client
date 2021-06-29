// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using FluentAssertions;
using NuGet.Commands.Test;
using NuGet.ProjectModel;
using NuGet.Test.Utility;
using Test.Utility.Commands;
using Xunit;
using static NuGet.Frameworks.FrameworkConstants;

namespace NuGet.Commands.FuncTest
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class RestoreCommand_PackagesLockFileTests
    {
        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenAPackageReferenceIsRemoved_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();

        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA);

        //        var projectName = "TestProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var packageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46", "net47" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageA.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Preconditions.
        //        var result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger)).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        packageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            packageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        PackageSpecOperations.RemoveDependency(packageSpec, packageA.Identity.Id);
        //        logger.Clear();
        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain("The package references have changed for net46. Lock file's package references: a:[1.0.0, ), project's package references: None.");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenATargetFrameworkIsAdded_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();

        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA);

        //        var projectName = "TestProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var packageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46", "net47" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageA.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Preconditions.
        //        var result = await (new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger))).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        packageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            packageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        PackageSpecOperationsUtility.AddTargetFramework(packageSpec, "net48");
        //        logger.Clear();
        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain("Lock file target frameworks: net46,net47,net48. Project target frameworks net46,net47.");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenATargetFrameworkIsUpdated_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();

        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA);

        //        var projectName = "TestProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var packageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46", "net47" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageA.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Preconditions.
        //        var result = await (new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger))).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        packageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            packageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        PackageSpecOperationsUtility.RemoveTargetFramework(packageSpec, "net47");
        //        PackageSpecOperationsUtility.AddTargetFramework(packageSpec, "net48");
        //        logger.Clear();
        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain("The project target framework net48 was not found in the lock file");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenADependecyIsAdded_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();

        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA,
        //            packageB);

        //        var projectName = "TestProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var packageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageA.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Preconditions.
        //        var result = await (new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger))).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        packageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            packageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageB.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        logger.Clear();
        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain("The package references have changed for net46. Lock file's package references: a:[1.0.0, ), project's package references: a:[1.0.0, ), b:[1.0.0, )");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenADependecyIsUpdated_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();

        //        var packageA100 = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageA200 = new SimpleTestPackageContext("a", "2.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA100,
        //            packageB);

        //        var projectName = "TestProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var packageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageA100.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageB.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Preconditions.
        //        var result = await (new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger))).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        packageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            packageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageA200.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        logger.Clear();
        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain("The package reference a version has changed from [1.0.0, ) to [2.0.0, )");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenADependencyIsReplaced_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();

        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");
        //        var packageC = new SimpleTestPackageContext("c", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA,
        //            packageB);

        //        var projectName = "TestProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var packageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageA.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageB.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Preconditions.
        //        var result = await (new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger))).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        packageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            packageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        PackageSpecOperations.RemoveDependency(packageSpec, packageB.Identity.Id);
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageC.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        logger.Clear();
        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain("A new package reference was found c for the project target framework net46.");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenARuntimeIdentifierIsAdded_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();

        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA);

        //        var projectName = "TestProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var packageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46", "net47" })
        //            .WithRuntimeIdentifiers(runtimeIdentifiers: new string[] { "win", "unix" }, runtimeSupports: new string[] { })
        //            .WithPackagesLockFile()
        //            .Build();

        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(packageSpec, packageA.Identity, packageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Preconditions.
        //        var result = await (new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger))).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        packageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            packageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        packageSpec.RuntimeGraph = ProjectTestHelpers.GetRuntimeGraph(runtimeIdentifiers: new string[] { "win", "unix", "ios" }, runtimeSupports: new string[] { });
        //        logger.Clear();
        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(packageSpec, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain("Project's runtime identifiers: ios;unix;win, lock file's runtime identifiers unix;win.");
        //    }
        //}


       [GlobalSetup]
        public async Task GlobalSetup()
        {
            await Setup1_DuplicateId();
            await Setup2_Normal();
        }

        private SimpleTestPathContext _pathContext1;
        private TestLogger _logger1;
        private PackageSpec _mainPackageSpec1;
        private List<PackageSpec> _allPackageSpecs1;
        private TestRestoreRequest _restoreRequest1;
        private async Task Setup1_DuplicateId()
        {
            _pathContext1 = new SimpleTestPathContext();
            _logger1 = new TestLogger();
            var packageA = new SimpleTestPackageContext("projectA", "1.0.0");

            await SimpleTestPackageUtility.CreateFolderFeedV3Async(
                _pathContext1.PackageSource,
                packageA);

            var targetFramework = CommonFrameworks.Net46;
            _allPackageSpecs1 = new List<PackageSpec>();

            var projectName = "RootProject";
            var projectDirectory = Path.Combine(_pathContext1.SolutionRoot, projectName);
            _mainPackageSpec1 = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
                .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
                .WithPackagesLockFile()
                .Build();
            _allPackageSpecs1.Add(_mainPackageSpec1);

            projectName = "IntermediateProject1";
            projectDirectory = Path.Combine(_pathContext1.SolutionRoot, projectName);
            var intermediateProject1PackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
                .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
                .Build();
            _allPackageSpecs1.Add(intermediateProject1PackageSpec);

            projectName = "projectA";
            projectDirectory = Path.Combine(_pathContext1.SolutionRoot, projectName);
            var projectAPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
                .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
                .Build();
            _allPackageSpecs1.Add(projectAPackageSpec);

            // Add the dependency to all frameworks
            PackageSpecOperations.AddOrUpdateDependency(intermediateProject1PackageSpec, packageA.Identity, intermediateProject1PackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
            PackageSpecOperationsUtility.AddProjectReference(_mainPackageSpec1, intermediateProject1PackageSpec, targetFramework);
            PackageSpecOperationsUtility.AddProjectReference(_mainPackageSpec1, projectAPackageSpec, targetFramework);

            _restoreRequest1 = ProjectTestHelpers.CreateRestoreRequest(_mainPackageSpec1, _allPackageSpecs1, _pathContext1, _logger1);
        }


        private SimpleTestPathContext _pathContext2;
        private TestLogger _logger2;
        private PackageSpec _mainPackageSpec2;
        private List<PackageSpec> _allPackageSpecs2;
        private TestRestoreRequest _restoreRequest2;
        private async Task Setup2_Normal()
        {
            _pathContext2 = new SimpleTestPathContext();
            _logger2 = new TestLogger();
            var packageA = new SimpleTestPackageContext("randomA", "1.0.0");

            await SimpleTestPackageUtility.CreateFolderFeedV3Async(
                _pathContext2.PackageSource,
                packageA);

            var targetFramework = CommonFrameworks.Net46;
            _allPackageSpecs2 = new List<PackageSpec>();

            var projectName = "RootProject";
            var projectDirectory = Path.Combine(_pathContext2.SolutionRoot, projectName);
            _mainPackageSpec2
 = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
                .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
                .WithPackagesLockFile()
                .Build();
            _allPackageSpecs2.Add(_mainPackageSpec2);

            projectName = "IntermediateProject1";
            projectDirectory = Path.Combine(_pathContext2.SolutionRoot, projectName);
            var intermediateProject1PackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
                .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
                .Build();
            _allPackageSpecs2.Add(intermediateProject1PackageSpec);

            projectName = "projectA";
            projectDirectory = Path.Combine(_pathContext2.SolutionRoot, projectName);
            var projectAPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
                .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
                .Build();
            _allPackageSpecs2.Add(projectAPackageSpec);

            // Add the dependency to all frameworks
            PackageSpecOperations.AddOrUpdateDependency(intermediateProject1PackageSpec, packageA.Identity, intermediateProject1PackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
            PackageSpecOperationsUtility.AddProjectReference(_mainPackageSpec2, intermediateProject1PackageSpec, targetFramework);
            PackageSpecOperationsUtility.AddProjectReference(_mainPackageSpec2, projectAPackageSpec, targetFramework);

            _restoreRequest2 = ProjectTestHelpers.CreateRestoreRequest(_mainPackageSpec2, _allPackageSpecs2, _pathContext2, _logger2);
        }

        [Benchmark(Description = "Test0:DuplicateId", Baseline = true)]
        [Fact]
        public async Task RestoreCommand_PackagesLockFile_DuplicateId()
        {
            // Preconditions.
            await new RestoreCommand(_restoreRequest1).ExecuteAsync();
        }

        [Benchmark(Description = "Test0:Normal")]
        [Fact]
        public async Task RestoreCommand_PackagesLockFile_Normal()
        {
            // Preconditions.
            await new RestoreCommand(_restoreRequest2).ExecuteAsync();
        }

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenANewTransitiveProjectReferenceIsAdded_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();
        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");
        //        var packageC = new SimpleTestPackageContext("c", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA,
        //            packageB,
        //            packageC);

        //        var targetFramework = CommonFrameworks.Net46;
        //        var allPackageSpecs = new List<PackageSpec>();

        //        var projectName = "RootProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var rootPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .WithPackagesLockFile()
        //            .Build();
        //        allPackageSpecs.Add(rootPackageSpec);

        //        projectName = "IntermediateProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var intermediatePackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(intermediatePackageSpec);

        //        projectName = "LeafProject1";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var leafProject1PackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(leafProject1PackageSpec);

        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(rootPackageSpec, packageA.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(intermediatePackageSpec, packageB.Identity, intermediatePackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(leafProject1PackageSpec, packageC.Identity, leafProject1PackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperationsUtility.AddProjectReference(rootPackageSpec, intermediatePackageSpec, targetFramework);
        //        PackageSpecOperationsUtility.AddProjectReference(intermediatePackageSpec, leafProject1PackageSpec, targetFramework);

        //        // Preconditions.
        //        var result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        rootPackageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            rootPackageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        logger.Clear();

        //        projectName = "LeaftProject2";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var leafProject2PackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        PackageSpecOperationsUtility.AddProjectReference(intermediatePackageSpec, leafProject2PackageSpec, targetFramework);
        //        allPackageSpecs.Add(leafProject2PackageSpec);

        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain($"The project reference intermediateproject has changed. Current dependencies: b,LeafProject1,LeaftProject2 lock file's dependencies: b,LeafProject1.");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenANewDirectProjectReferenceChangesFramework_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();
        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA);

        //        var targetFramework = CommonFrameworks.Net46;
        //        var allPackageSpecs = new List<PackageSpec>();

        //        var projectName = "RootProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var rootPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        allPackageSpecs.Add(rootPackageSpec);

        //        projectName = "IntermediateProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var projectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(projectReferenceSpec);

        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(rootPackageSpec, packageA.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperationsUtility.AddProjectReference(rootPackageSpec, projectReferenceSpec, targetFramework);

        //        // Preconditions.
        //        var result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        rootPackageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            rootPackageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        logger.Clear();

        //        PackageSpecOperationsUtility.RemoveTargetFramework(projectReferenceSpec, "net46");
        //        PackageSpecOperationsUtility.AddTargetFramework(projectReferenceSpec, "net47");

        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain($"The project IntermediateProject has no compatible target framework.");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenANewTransitiveProjectReferenceChangesFramework_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();
        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA);

        //        var targetFramework = CommonFrameworks.Net46;
        //        var allPackageSpecs = new List<PackageSpec>();

        //        var projectName = "RootProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var rootPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        allPackageSpecs.Add(rootPackageSpec);

        //        projectName = "IntermediateProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var intermediateProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(intermediateProjectReferenceSpec);


        //        projectName = "LeafProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var leafProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(leafProjectReferenceSpec);

        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(rootPackageSpec, packageA.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperationsUtility.AddProjectReference(rootPackageSpec, intermediateProjectReferenceSpec, targetFramework);
        //        PackageSpecOperationsUtility.AddProjectReference(intermediateProjectReferenceSpec, leafProjectReferenceSpec, targetFramework);

        //        // Preconditions.
        //        var result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        rootPackageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            rootPackageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        logger.Clear();

        //        PackageSpecOperationsUtility.RemoveTargetFramework(leafProjectReferenceSpec, "net46");
        //        PackageSpecOperationsUtility.AddTargetFramework(leafProjectReferenceSpec, "net47");

        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain($"The project LeafProject has no compatible target framework.");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenADirectProjectReferenceChangesDependencies_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();
        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");
        //        var packageC = new SimpleTestPackageContext("c", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA,
        //            packageB,
        //            packageC);

        //        var targetFramework = CommonFrameworks.Net46;
        //        var allPackageSpecs = new List<PackageSpec>();

        //        var projectName = "RootProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var rootPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        allPackageSpecs.Add(rootPackageSpec);

        //        projectName = "IntermediateProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var intermediateProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(intermediateProjectReferenceSpec);


        //        projectName = "LeafProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var leafProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(leafProjectReferenceSpec);

        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(rootPackageSpec, packageA.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(intermediateProjectReferenceSpec, packageB.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperationsUtility.AddProjectReference(rootPackageSpec, intermediateProjectReferenceSpec, targetFramework);
        //        PackageSpecOperationsUtility.AddProjectReference(intermediateProjectReferenceSpec, leafProjectReferenceSpec, targetFramework);

        //        // Preconditions.
        //        var result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        rootPackageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            rootPackageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        logger.Clear();

        //        PackageSpecOperations.AddOrUpdateDependency(intermediateProjectReferenceSpec, packageC.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain($"The project reference intermediateproject has changed. Current dependencies: b,c,LeafProject lock file's dependencies: b,LeafProject.");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenATransitiveProjectReferenceChangesDependencies_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();
        //        var packageA = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");
        //        var packageC = new SimpleTestPackageContext("c", "1.0.0");
        //        var packageD = new SimpleTestPackageContext("d", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA,
        //            packageB,
        //            packageC,
        //            packageD);

        //        var targetFramework = CommonFrameworks.Net46;
        //        var allPackageSpecs = new List<PackageSpec>();

        //        var projectName = "RootProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var rootPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { "net46" })
        //            .WithPackagesLockFile()
        //            .Build();
        //        allPackageSpecs.Add(rootPackageSpec);

        //        projectName = "IntermediateProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var intermediateProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(intermediateProjectReferenceSpec);


        //        projectName = "LeafProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var leafProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(leafProjectReferenceSpec);

        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(rootPackageSpec, packageA.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(intermediateProjectReferenceSpec, packageB.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(leafProjectReferenceSpec, packageC.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperationsUtility.AddProjectReference(rootPackageSpec, intermediateProjectReferenceSpec, targetFramework);
        //        PackageSpecOperationsUtility.AddProjectReference(intermediateProjectReferenceSpec, leafProjectReferenceSpec, targetFramework);

        //        // Preconditions.
        //        var result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        rootPackageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            rootPackageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        logger.Clear();

        //        PackageSpecOperations.AddOrUpdateDependency(leafProjectReferenceSpec, packageD.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain($"The project reference leafproject has changed. Current dependencies: c,d lock file's dependencies: c");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenADirectProjectReferenceUpdatesDependency_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();
        //        var packageA100 = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageA200 = new SimpleTestPackageContext("a", "2.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");
        //        var packageC = new SimpleTestPackageContext("c", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA100,
        //            packageA200,
        //            packageB,
        //            packageC);

        //        var targetFramework = CommonFrameworks.Net46;
        //        var allPackageSpecs = new List<PackageSpec>();

        //        var projectName = "RootProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var rootPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .WithPackagesLockFile()
        //            .Build();
        //        allPackageSpecs.Add(rootPackageSpec);

        //        projectName = "IntermediateProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var intermediateProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(intermediateProjectReferenceSpec);


        //        projectName = "LeafProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var leafProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(leafProjectReferenceSpec);

        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(rootPackageSpec, packageB.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(intermediateProjectReferenceSpec, packageA100.Identity, intermediateProjectReferenceSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperationsUtility.AddProjectReference(rootPackageSpec, intermediateProjectReferenceSpec, targetFramework);
        //        PackageSpecOperationsUtility.AddProjectReference(intermediateProjectReferenceSpec, leafProjectReferenceSpec, targetFramework);

        //        // Preconditions.
        //        var result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        rootPackageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            rootPackageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        logger.Clear();

        //        PackageSpecOperations.AddOrUpdateDependency(intermediateProjectReferenceSpec, packageA200.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));

        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain($"The project references intermediateproject whose dependencies has changed.");
        //    }
        //}

        //[Fact]
        //public async Task RestoreCommand_PackagesLockFile_InLockedMode_WhenADirectProjectReferenceAddsNewProjectReference_FailsWithNU1004()
        //{
        //    // Arrange
        //    using (var pathContext = new SimpleTestPathContext())
        //    {
        //        var logger = new TestLogger();
        //        var packageA100 = new SimpleTestPackageContext("a", "1.0.0");
        //        var packageB = new SimpleTestPackageContext("b", "1.0.0");
        //        var packageC = new SimpleTestPackageContext("c", "1.0.0");

        //        await SimpleTestPackageUtility.CreateFolderFeedV3Async(
        //            pathContext.PackageSource,
        //            packageA100,
        //            packageB,
        //            packageC);

        //        var targetFramework = CommonFrameworks.Net46;
        //        var allPackageSpecs = new List<PackageSpec>();

        //        var projectName = "RootProject";
        //        var projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var rootPackageSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .WithPackagesLockFile()
        //            .Build();
        //        allPackageSpecs.Add(rootPackageSpec);

        //        projectName = "IntermediateProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var intermediateProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(intermediateProjectReferenceSpec);


        //        projectName = "LeafProject";
        //        projectDirectory = Path.Combine(pathContext.SolutionRoot, projectName);
        //        var leafProjectReferenceSpec = PackageReferenceSpecBuilder.Create(projectName, projectDirectory)
        //            .WithTargetFrameworks(new string[] { targetFramework.GetShortFolderName() })
        //            .Build();
        //        allPackageSpecs.Add(leafProjectReferenceSpec);

        //        // Add the dependency to all frameworks
        //        PackageSpecOperations.AddOrUpdateDependency(rootPackageSpec, packageB.Identity, rootPackageSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperations.AddOrUpdateDependency(intermediateProjectReferenceSpec, packageA100.Identity, intermediateProjectReferenceSpec.TargetFrameworks.Select(e => e.FrameworkName));
        //        PackageSpecOperationsUtility.AddProjectReference(rootPackageSpec, intermediateProjectReferenceSpec, targetFramework);

        //        // Preconditions.
        //        var result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();
        //        await result.CommitAsync(logger, CancellationToken.None);
        //        result.Success.Should().BeTrue();

        //        // Enable locked mode, remove a dependency and clear the logger;
        //        rootPackageSpec.RestoreMetadata.RestoreLockProperties = new RestoreLockProperties(
        //            restorePackagesWithLockFile: "true",
        //            rootPackageSpec.RestoreMetadata.RestoreLockProperties.NuGetLockFilePath,
        //            restoreLockedMode: true);
        //        logger.Clear();

        //        PackageSpecOperations.RemoveDependency(intermediateProjectReferenceSpec, packageA100.Identity.Id);
        //        PackageSpecOperationsUtility.AddProjectReference(intermediateProjectReferenceSpec, leafProjectReferenceSpec, targetFramework);
        //        // Act.
        //        result = await new RestoreCommand(ProjectTestHelpers.CreateRestoreRequest(rootPackageSpec, allPackageSpecs, pathContext, logger)).ExecuteAsync();

        //        // Assert.
        //        result.Success.Should().BeFalse();
        //        logger.ErrorMessages.Count.Should().Be(1);
        //        logger.ErrorMessages.Single().Should().Contain("NU1004");
        //        logger.ErrorMessages.Single().Should().Contain($"The project references intermediateproject whose dependencies has changed.");
        //    }
        //}
    }
}
