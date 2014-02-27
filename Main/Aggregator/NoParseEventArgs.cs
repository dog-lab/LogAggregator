namespace Broos.Monitor.LogAggregator.Aggregator {
    using System;
    using Entity;

    /// <summary>
    /// No parsing done here! The information here is part of
    /// the NoParse event just to warn you that "nothing" was done.
    /// </summary>
    public class NoParseEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoParseEventArgs"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public NoParseEventArgs(LogFile log) {
            this.Log = log;
        }

        /// <summary>
        /// Gets or sets the log file object that had "nothing" happening to it!
        /// </summary>
        /// <value>
        /// The log file object.
        /// </value>
        public LogFile Log { get; set; }
    }
}