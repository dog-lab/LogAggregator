namespace Broos.Monitor.LogAggregator.Entity {
    using System;

    /// <summary>
    /// The original data source...the main input into this program.
    /// The LogFile tells us where we found the data (a location)
    /// and provides information useful in finding if anything has
    /// changed in the file since the last time it was parsed
    /// (last line count, last timestamp, and last parse line).
    /// </summary>
    public class LogFile {
        /// <summary>
        /// The unique identifier of this Log File object.
        /// </summary>
        private int _id;

        /// <summary>
        /// The computer host name that produced the log.
        /// </summary>
        private string _hostName = string.Empty;
        
        /// <summary>
        /// The count, or index, into the log file the last time it was parsed.
        /// </summary>
        private int _lastLineCount;
        
        /// <summary>
        /// The last parsed line for this log file.
        /// </summary>
        private string _lastParsedLine = string.Empty;
        
        /// <summary>
        /// The last time and date entry for this log file.
        /// </summary>
        private DateTime? _lastTimeStamp;
        
        /// <summary>
        /// The full path and file name to this log file.
        /// </summary>
        private string _location = string.Empty;
        
        /// <summary>
        /// The log name.
        /// </summary>
        private string _name = string.Empty;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id {
            get { return _id; }

            set { _id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        public string HostName {
            get { return _hostName; }

            set { _hostName = value; }
        }

        /// <summary>
        /// Gets or sets the last line count.
        /// </summary>
        /// <value>
        /// The last line count.
        /// </value>
        public int LastLineCount {
            get { return _lastLineCount; }

            set { _lastLineCount = value; }
        }

        /// <summary>
        /// Gets or sets the last parsed line.
        /// </summary>
        /// <value>
        /// The last parsed line.
        /// </value>
        public string LastParsedLine {
            get { return _lastParsedLine; }

            set { _lastParsedLine = value; }
        }

        /// <summary>
        /// Gets or sets the last timestamp.
        /// </summary>
        /// <value>
        /// The last timestamp.
        /// </value>
        public DateTime? LastTimestamp {
            get { return _lastTimeStamp; }

            set { _lastTimeStamp = value; }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location {
            get { return _location; }

            set { _location = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name {
            get { return _name; }

            set { _name = value; }
        }
    }
}