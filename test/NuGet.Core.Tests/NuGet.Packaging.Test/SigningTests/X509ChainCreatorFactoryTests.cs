// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Moq;
using NuGet.Common;
using NuGet.Packaging.Signing;
using NuGet.Test.Utility;
using Xunit;

namespace NuGet.Packaging.Test
{
    public class X509ChainCreatorFactoryTests : IClassFixture<CertificatesFixture>
    {
        private readonly CertificatesFixture _fixture;

        public X509ChainCreatorFactoryTests(CertificatesFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public void Create_WhenArgumentIsNull_Throws()
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => X509ChainCreatorFactory.Create(environmentVariableReader: null));

            Assert.Equal("environmentVariableReader", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_WhenEnvironmentVariableIsInvalid_ReturnsDefaultCreator(string value)
        {
            Mock<IEnvironmentVariableReader> reader = CreateMockEnvironmentVariableReader(value);

            IX509ChainCreator creator = X509ChainCreatorFactory.Create(reader.Object);

            Assert.IsType<DefaultCtlX509ChainCreator>(creator);

            reader.VerifyAll();
        }

        [Fact]
        public void Create_WhenEnvironmentVariableIsValid_ReturnsCorrectCreator()
        {
            using (TestDirectory directory = TestDirectory.Create())
            {
                FileInfo p7bFile = CreateP7bFile(directory);
                Mock<IEnvironmentVariableReader> reader = CreateMockEnvironmentVariableReader(p7bFile.FullName);

                IX509ChainCreator creator = X509ChainCreatorFactory.Create(reader.Object);

#if NET5_0_OR_GREATER
                Assert.IsType<FallbackCtlX509ChainCreator>(creator);
#else
                Assert.IsType<DefaultCtlX509ChainCreator>(creator);
#endif

                reader.VerifyAll();
            }
        }

        private static Mock<IEnvironmentVariableReader> CreateMockEnvironmentVariableReader(
            string trustedRootsFilePath)
        {
            var reader = new Mock<IEnvironmentVariableReader>(MockBehavior.Strict);

            reader.Setup(r => r.GetEnvironmentVariable(X509ChainCreatorFactory.TrustedRootsFilePath))
                .Returns(trustedRootsFilePath);

            return reader;
        }

        private FileInfo CreateP7bFile(TestDirectory directory)
        {
            using (X509Certificate2 certificate = _fixture.GetDefaultCertificate())
            {
                var certificates = new X509Certificate2Collection();

                certificates.Add(certificate);

                var p7bFile = new FileInfo(Path.Combine(directory.Path, "certificates.p7b"));

                File.WriteAllBytes(p7bFile.FullName, certificates.Export(X509ContentType.Pkcs7));

                return p7bFile;
            }
        }
    }
}
