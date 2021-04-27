// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NuGet.Commands
{
    /// <summary>
    /// DG v2 related validation error.
    /// </summary>
    [Serializable]
    public class RestoreSpecException : Exception
    {
        public IEnumerable<string> Files { get; }

        private RestoreSpecException(string message, IEnumerable<string> files, Exception innerException)
                : base(message, innerException)
        {
            Files = files;
        }

        // This protected constructor is used for deserialization.
        protected RestoreSpecException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
            List<string> filesList = (List<string>)info.GetValue("RestoreSpecException.Files", typeof(List<string>));
            Files = filesList.AsEnumerable();
        }

        public static RestoreSpecException Create(string message, IEnumerable<string> files)
        {
            return Create(message, files, innerException: null);
        }

        // GetObjectData performs a custom serialization.
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            //Convert the Files into List<string> type, so it's serializable.
            List<string> filesList = Files.ToList();

            info.AddValue("RestoreSpecException.Files", filesList, filesList.GetType());
            base.GetObjectData(info, context);
        }

        public static RestoreSpecException Create(string message, IEnumerable<string> files, Exception innerException)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            files = files.Where(path => !string.IsNullOrEmpty(path)).Distinct(StringComparer.Ordinal).ToList();

            string completeMessage = null;

            if (files.Any())
            {
                completeMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    Strings.InvalidRestoreInputWithFiles,
                    message,
                    string.Join(", ", files));
            }
            else
            {
                completeMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    Strings.InvalidRestoreInput,
                    message);
            }

            return new RestoreSpecException(completeMessage, files, innerException);
        }
    }
}
