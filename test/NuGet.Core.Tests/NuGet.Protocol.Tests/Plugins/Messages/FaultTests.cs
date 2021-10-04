// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace NuGet.Protocol.Plugins.Tests
{
    public class FaultTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_ThrowsForNullOrEmptyMessage(string message)
        {
            var exception = Assert.Throws<ArgumentException>(() => new Fault(message));

            Assert.Equal("message", exception.ParamName);
        }

        [Fact]
        public void Constructor_InitializesMessageProperty()
        {
            var fault = new Fault(message: "a");

            Assert.Equal("a", fault.Message);
        }

        [Fact]
        public void JsonSerialization_ReturnsCorrectJson()
        {
            var fault = new Fault(message: "a");
            var json = TestUtilities.Serialize(fault);

            Assert.Equal("{\"Message\":\"a\"}", json);
        }

        [Fact]
        public void JsonDeserialization_ReturnsCorrectObject()
        {
            var json = "{\"Message\":\"a\"}";
            var fault = JsonSerializationUtilities.Deserialize<Fault>(json);

            Assert.Equal("a", fault.Message);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"Message\":null}")]
        [InlineData("{\"Message\":\"\"}")]
        public void JsonDeserialization_ThrowsForNullOrEmptyMessage(string json)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => JsonSerializationUtilities.Deserialize<Fault>(json));

            Assert.Equal("message", exception.ParamName);
        }

        [Theory]
        [InlineData(MessageResponseCode.NotFound, null, null, null, "{\"ResponseCode\":\"NotFound\"}")]
        [InlineData(MessageResponseCode.Success, "a", "b", null, "{\"Password\":\"b\",\"ResponseCode\":\"Success\",\"Username\":\"a\"}")]
        [InlineData(MessageResponseCode.Success, "a", "b", new[] { "basic", "negotiate" }, "{\"Password\":\"b\",\"ResponseCode\":\"Success\",\"Username\":\"a\",\"AuthenticationTypes\":[\"basic\",\"negotiate\"]}")]
        public void JsonSerialization_BadTest(
            MessageResponseCode responseCode,
            string username,
            string password,
            string[] authTypes,
            string expectedJson)
        {
            var response = new GetCredentialsResponse(responseCode, username, password, authTypes);

            var actualJson = TestUtilities.Serialize(response);

            Assert.Equal(expectedJson, actualJson);
        }

    }
}
