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
using Xunit;

namespace NuGet.Commands.Test
{
    public class ExceptionSerializationTests
    {
        [Fact]
        public void RestoreSpecException_Serialization_successful()
        {

            // Arrange
            RestoreSpecException exception = RestoreSpecException.Create(message: "message", files: new string[] { "file1", "file2" });

            // Act
            // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                // "Save" object state
                formatter.Serialize(ms, exception);

                // Re-use the same stream for de-serialization
                ms.Seek(0, 0);

                // Replace the original exception with de-serialized one
                RestoreSpecException serializeAndDeserializedException = (RestoreSpecException)formatter.Deserialize(ms);

                // Assert
                Assert.Equal(serializeAndDeserializedException.Message, exception.Message);
                Assert.Equal(serializeAndDeserializedException.Files.Count, exception.Files.Count);
                for (int i = 0; i < serializeAndDeserializedException.Files.Count; i++)
                {
                    Assert.Equal(serializeAndDeserializedException.Files[i], exception.Files[i]);
                }
            }

            
        }
    }
}
