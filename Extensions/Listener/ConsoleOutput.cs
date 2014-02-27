namespace Gator.Extensions.Listener {
    using System;
    using Broos.Monitor.LogAggregator.Aggregator;

    /// <summary>
    /// Sample class that captures the output of each line from the parsing code and
    /// spits parts of it back out on the command line.
    /// </summary>
    public class ConsoleOutput : IParseListener {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object,System.Object)", Justification = "Reviewed")]
        public void OnLineParsed(object sender, LogEntryItemEventArgs e) {
            if (e == null) {
                throw new ArgumentNullException("e");
            }

            Console.WriteLine("Log ID: {0}\tTimestamp: {1}\tMessage: {2}", e.Entry.Source.Location, e.Entry.Timestamp, e.Entry.Message);
        }

        /// <summary>
        /// Called if file is *not* parsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NoParseEventArgs" /> instance containing the event data.</param>
        public void OnNoParse(object sender, NoParseEventArgs e) {
        }

        /// <summary>
        /// Called when file parsing has stoped.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogEventArgs" /> instance containing the event data.</param>
        public void OnParseEnded(object sender, LogEventArgs e) {
        }

        /// <summary>
        /// Called when file parsing has begun.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogEventArgs" /> instance containing the event data.</param>
        public void OnParseStarted(object sender, LogEventArgs e) {
        }
    }
}
