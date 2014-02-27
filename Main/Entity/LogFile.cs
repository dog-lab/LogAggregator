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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the last line count.
        /// </summary>
        /// <value>
        /// The last line count.
        /// </value>
        public int LastLineCount { get; set; }

        /// <summary>
        /// Gets or sets the last timestamp.
        /// </summary>
        /// <value>
        /// The last timestamp.
        /// </value>
        public DateTime? LastTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the last parsed line.
        /// </summary>
        /// <value>
        /// The last parsed line.
        /// </value>
        public string LastParsedLine { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}