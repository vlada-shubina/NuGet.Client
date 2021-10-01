// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;
using NuGet.Common;

namespace NuGet.Commands
{
    /// <summary>
    /// Holds an <see cref="IRestoreLogMessage"/> and returns the message for the exception.
    /// </summary>
    [Serializable]
    public class RestoreCommandException : Exception, ILogMessageException
    {
        private readonly IRestoreLogMessage _logMessage;

        public RestoreCommandException(IRestoreLogMessage logMessage)
            : base(logMessage?.Message)
        {
            _logMessage = logMessage ?? throw new ArgumentNullException(nameof(logMessage));
        }

        protected RestoreCommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _logMessage = (IRestoreLogMessage)info.GetValue(nameof(_logMessage), typeof(IRestoreLogMessage));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(_logMessage), _logMessage);
        }

        public ILogMessage AsLogMessage()
        {
            return _logMessage;
        }
    }
}
