namespace Broos.Monitor.LogAggregator.Aggregator {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using Entity;

    /// <summary>
    /// Exceptions thrown when parsing user created Parse code.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Underscore already signals a local field.")]
    public class ParseCodeException : Exception {
        /// <summary>
        /// LogFile object involved in the exception.
        /// </summary>
        private readonly LogFile _file;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseCodeException"/> class.
        /// </summary>
        public ParseCodeException() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseCodeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ParseCodeException(string message) : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseCodeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="file">The log file.</param>
        public ParseCodeException(string message, LogFile file) : base(message) {
            _file = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseCodeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ParseCodeException(string message, Exception innerException) : base(message, innerException) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseCodeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected ParseCodeException(SerializationInfo info, StreamingContext context) : base(info, context) {
            if (info != null) {
                Type type = Type.GetType("Broos.Monitor.LogAggregator.Entity.LogFile");
                if (type != null) {
                    _file = (LogFile) info.GetValue("LogFile", type);
                }
            }
        }

        /// <summary>
        /// Gets the exception log file.
        /// </summary>
        /// <value>The exception log file.</value>
        public LogFile ExceptionLogFile {
            get { return _file; }
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("LogFile", _file);
       }
    }
}