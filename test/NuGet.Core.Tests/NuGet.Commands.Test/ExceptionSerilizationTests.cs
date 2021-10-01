// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Signing;
using Xunit;

namespace NuGet.Commands.Test
{
    public class ExceptionSerilizationTests
    {
        [Fact]
        public void RestoreSpecException_Serialzation_Successful()
        {
            RestoreSpecException exception = RestoreSpecException.Create("message1", new List<string> { "file1", "file2", "file3" });

#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();

            //serialization
            FileStream s = new FileStream("dummy.txt", FileMode.Create);
            formatter.Serialize(s, exception);
            s.Close();

            //deserialization
            s = new FileStream("dummy.txt", FileMode.Open);
            RestoreSpecException exception2 = (RestoreSpecException)formatter.Deserialize(s);
            Assert.NotNull(exception2);
            Assert.Equal(exception.Message, exception2.Message);
            Assert.Equal(exception.Files.Count, exception2.Files.Count);
            for (int i = 0; i < exception.Files.Count; i++)
            {
                Assert.Equal(exception.Files[i], exception2.Files[i]);
            }
#pragma warning restore SYSLIB0011
        }

        [Fact]
        public void RestoreSpecExceptionWithInnerException_Serialzation_Successful()
        {
            var exception = RestoreSpecException.Create("message1", new List<string> { "file1", "file2", "file3" }, new Exception("Inner Exception"));

#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();

            //serialization
            FileStream s = new FileStream("dummy.txt", FileMode.Create);
            formatter.Serialize(s, exception);
            s.Close();

            //deserialization
            s = new FileStream("dummy.txt", FileMode.Open);
            RestoreSpecException exception2 = (RestoreSpecException)formatter.Deserialize(s);
            Assert.NotNull(exception2);
            Assert.Equal(exception.Message, exception2.Message);
            Assert.Equal(exception.Files.Count, exception2.Files.Count);
            for (int i = 0; i < exception.Files.Count; i++)
            {
                Assert.Equal(exception.Files[i], exception2.Files[i]);
            }
            Assert.NotNull(exception2.InnerException);
            Assert.Equal(exception.InnerException.Message, exception2.InnerException.Message);
#pragma warning restore SYSLIB0011
        }

        [Fact]
        public void RestoreCommandException_Serialzation_Successful()
        {
            RestoreLogMessage restoreLogMessage = RestoreLogMessage.CreateError(
                code: NuGetLogCode.NU1003,
                message: "message1",
                libraryId: "libraryId1",
                targetGraphs: new string[] {"Graph1", "Graph2" });
            var exception = new RestoreCommandException(restoreLogMessage);

#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();

            //serialization
            FileStream s = new FileStream("dummy.txt", FileMode.Create);
            formatter.Serialize(s, exception);
            s.Close();

            //deserialization
            s = new FileStream("dummy.txt", FileMode.Open);
            RestoreCommandException exception2 = (RestoreCommandException)formatter.Deserialize(s);
            Assert.NotNull(exception2);
            Assert.Equal(exception.Message, exception2.Message);
            Assert.Equal(exception.AsLogMessage().Level, exception2.AsLogMessage().Level);
            Assert.Equal(exception.AsLogMessage().WarningLevel, exception2.AsLogMessage().WarningLevel);
            Assert.Equal(exception.AsLogMessage().Code, exception2.AsLogMessage().Code);
            Assert.Equal(exception.AsLogMessage().Message, exception2.AsLogMessage().Message);
            Assert.Equal(exception.AsLogMessage().ProjectPath, exception2.AsLogMessage().ProjectPath);
            Assert.Equal(exception.AsLogMessage().Time, exception2.AsLogMessage().Time);
#pragma warning restore SYSLIB0011
        }

        [Fact]
        public void SignCommandException_Serialzation_Successful()
        {
            SignatureLog signatureLog = SignatureLog.Issue(fatal: true, code: NuGetLogCode.NU3005, message: "message1");
            var exception = new SignCommandException(signatureLog);

#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();

            //serialization
            FileStream s = new FileStream("dummy.txt", FileMode.Create);
            formatter.Serialize(s, exception);
            s.Close();

            //deserialization
            s = new FileStream("dummy.txt", FileMode.Open);
            SignCommandException exception2 = (SignCommandException)formatter.Deserialize(s);
            Assert.NotNull(exception2);
            Assert.Equal(exception.Message, exception2.Message);
            Assert.Equal(exception.AsLogMessage().Level, exception2.AsLogMessage().Level);
            Assert.Equal(exception.AsLogMessage().WarningLevel, exception2.AsLogMessage().WarningLevel);
            Assert.Equal(exception.AsLogMessage().Code, exception2.AsLogMessage().Code);
            Assert.Equal(exception.AsLogMessage().Message, exception2.AsLogMessage().Message);
            Assert.Equal(exception.AsLogMessage().ProjectPath, exception2.AsLogMessage().ProjectPath);
            Assert.Equal(exception.AsLogMessage().Time, exception2.AsLogMessage().Time);
#pragma warning restore SYSLIB0011
        }

        [Fact]
        public void CommandLineArgumentCombinationException_Serialzation_Successful()
        {
            var exception = new CommandLineArgumentCombinationException("message1");

#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();

            //serialization
            FileStream s = new FileStream("dummy.txt", FileMode.Create);
            formatter.Serialize(s, exception);
            s.Close();

            //deserialization
            s = new FileStream("dummy.txt", FileMode.Open);
            CommandLineArgumentCombinationException exception2 = (CommandLineArgumentCombinationException)formatter.Deserialize(s);
            Assert.NotNull(exception2);
            Assert.Equal(exception.Message, exception2.Message);
            Assert.Equal(exception.AsLogMessage().Level, exception2.AsLogMessage().Level);
            Assert.Equal(exception.AsLogMessage().WarningLevel, exception2.AsLogMessage().WarningLevel);
            Assert.Equal(exception.AsLogMessage().Code, exception2.AsLogMessage().Code);
            Assert.Equal(exception.AsLogMessage().Message, exception2.AsLogMessage().Message);
            Assert.Equal(exception.AsLogMessage().ProjectPath, exception2.AsLogMessage().ProjectPath);
            Assert.Equal(exception.AsLogMessage().Time, exception2.AsLogMessage().Time);
#pragma warning restore SYSLIB0011
        }
        
    }
}
