// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NET5_0_OR_GREATER

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using NuGet.Packaging.Signing;
using NuGet.Test.Utility;
using Xunit;

namespace NuGet.Packaging.Test.SigningTests
{
    public class FallbackCtlX509ChainCreatorTests : IClassFixture<CertificatesFixture>
    {
        private readonly CertificatesFixture _fixture;

        public FallbackCtlX509ChainCreatorTests(CertificatesFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public void Constructor_WhenArgumentIsNull_Throws()
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => new FallbackCtlX509ChainCreator(file: null));

            Assert.Equal("file", exception.ParamName);
        }

        [Fact]
        public void Constructor_WhenFileExtensionIsInvalid_Throws()
        {
            using (TestDirectory directory = TestDirectory.Create())
            {
                var invalidFile = new FileInfo(Path.Combine(directory.Path, "certificate.cer"));

                ArgumentException exception = Assert.Throws<ArgumentException>(
                    () => new FallbackCtlX509ChainCreator(invalidFile));

                Assert.StartsWith($"The file at '{invalidFile.FullName}' is invalid.  The file must be a valid PKCS #7 file with a '.p7b' extension.", exception.Message);
                Assert.Equal("file", exception.ParamName);
            }
        }

        [Fact]
        public void Constructor_WhenFileDoesNotExist_Throws()
        {
            using (TestDirectory directory = TestDirectory.Create())
            {
                var nonexistentFile = new FileInfo(Path.Combine(directory.Path, "certificates.p7b"));

                FileNotFoundException exception = Assert.Throws<FileNotFoundException>(
                    () => new FallbackCtlX509ChainCreator(nonexistentFile));

                Assert.StartsWith($"The file at '{nonexistentFile.FullName}' was not found.", exception.Message);
                Assert.Equal(nonexistentFile.FullName, exception.FileName);
            }
        }

        [Fact]
        public void Create_Always_CreatesFallbackX509Chain()
        {
            using (TestDirectory directory = TestDirectory.Create())
            using (X509Certificate2 certificate = _fixture.GetDefaultCertificate())
            {
                FileInfo p7bFile = CreateP7bFile(directory, certificate);
                var creator = new FallbackCtlX509ChainCreator(p7bFile);

                using (X509Chain chain = creator.Create())
                {
                    Assert.Equal(X509ChainTrustMode.CustomRootTrust, chain.ChainPolicy.TrustMode);
                    Assert.Equal(1, chain.ChainPolicy.CustomTrustStore.Count);
                    Assert.Equal(certificate.Thumbprint, chain.ChainPolicy.CustomTrustStore[0].Thumbprint);
                }
            }
        }

        private static FileInfo CreateP7bFile(TestDirectory directory, X509Certificate2 certificate)
        {
            var certificates = new X509Certificate2Collection();

            certificates.Add(certificate);

            var p7bFile = new FileInfo(Path.Combine(directory.Path, "certificates.p7b"));

            File.WriteAllBytes(p7bFile.FullName, certificates.Export(X509ContentType.Pkcs7));

            return p7bFile;
        }
    }
}
#endif
