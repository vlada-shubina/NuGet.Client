// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace NuGet.Common
{
    [Serializable]
    public class CommandLineArgumentCombinationException : Exception, ILogMessageException
    {
        private readonly ILogMessage _logMessage;

        public CommandLineArgumentCombinationException(string message)
            : base(message)
        {
            _logMessage = LogMessage.CreateError(NuGetLogCode.NU1000, message);
        }

        protected CommandLineArgumentCombinationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _logMessage = (ILogMessage)info.GetValue(nameof(_logMessage), typeof(ILogMessage));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(_logMessage), _logMessage);
        }

        public virtual ILogMessage AsLogMessage()
        {
            return _logMessage;
        }
    }
}
