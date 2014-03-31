namespace Broos.Monitor.LogAggregator.Aggregator {
    using System;
    using Entity;

    /// <summary>
    /// An event happens and a new log entry is presented into the world and
    /// this event provides all the information you need to deal with it:
    ///     1> the "new" transformed data as a LogEntry object, and
    ///     2> the original log "line" that made up the pre-transformed version
    /// </summary>
    public class LogEntryItemEventArgs : EventArgs {
        /// <summary>
        /// The Log Entry object behind the firing of this event.
        /// </summary>
        private LogEntry _logEntry;

        /// <summary>
        /// The unparsed line that goes with the Log Entry object.
        /// </summary>
        private string _rawLine = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryItemEventArgs"/> class.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="rawLine">The raw line.</param>
        public LogEntryItemEventArgs(LogEntry logEntry, string rawLine) {
            this.Entry = logEntry;
            this.RawLine = rawLine;
        }

        /// <summary>
        /// Gets or sets the log entry object.
        /// </summary>
        /// <value>
        /// The entry.
        /// </value>
        public LogEntry Entry {
            get { return _logEntry; }

            set { _logEntry = value; }
        }

        /// <summary>
        /// Gets or sets the raw line that supplied the data for the LogEntry.
        /// </summary>
        /// <value>
        /// The raw line.
        /// </value>
        public string RawLine {
            get { return _rawLine; }

            set { _rawLine = value; }
        }
    }
}