namespace Broos.Monitor.LogAggregator.Entity {
    /// <summary>
    /// The output object from the parsed log file. This is the whole point
    /// of the project...finding and creating a useful common ground for
    /// log file data. We can query this data to help us find,
    /// and hopefully solve, problems or perhaps find a
    /// trend or two...
    /// </summary>
    public class LogEntry {
        /// <summary>
        /// Gets or sets the unique identifier for this entry. If
        /// originated from a database, the "ID field."
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets any extended information contained in the log
        /// text "line" or entry. This is meant to break out data we wish
        /// to further analyze. One could use this, perhaps, to store
        /// data further parsed in, let's say, JSON or some other
        /// format. In other words, this is a free form data field so
        /// have a "field day" with it.
        /// </summary>
        /// <value>
        /// The extended data.
        /// </value>
        public string Extensions { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the message part of the log text "line" or entry.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the LogFile object that owns this LogEntry.
        /// </summary>
        /// <value>
        /// The LogFile source.
        /// </value>
        public LogFile Source { get; set; }

        /// <summary>
        /// Gets or sets the timestamp part of the log text "line" or entry
        /// (if a time stamp exists).
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public System.DateTime Timestamp { get; set; }
    }
}
