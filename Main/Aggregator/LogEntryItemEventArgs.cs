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
        public LogEntry Entry { get; set; }

        /// <summary>
        /// Gets or sets the raw line that supplied the data for the LogEntry.
        /// </summary>
        /// <value>
        /// The raw line.
        /// </value>
        public string RawLine { get; set; }
    }
}