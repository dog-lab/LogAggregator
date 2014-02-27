namespace Broos.Monitor.LogAggregator.Aggregator {
    using System;
    using Entity;

    /// <summary>
    /// Here it is...the definitive answer to "what happened" with
    /// this log file? An event (this!) happens and the information
    /// for it is contained in the LogFile object generously passed
    /// to you here!
    /// </summary>
    public class LogEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventArgs"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public LogEventArgs(LogFile log) {
            this.Log = log;
        }

        /// <summary>
        /// Gets or sets the log file object.
        /// </summary>
        /// <value>
        /// The log file object thingy.
        /// </value>
        public LogFile Log { get; set; }
    }
}