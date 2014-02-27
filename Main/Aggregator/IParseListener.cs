namespace Broos.Monitor.LogAggregator.Aggregator {
    /// <summary>
    /// Defines what a parse listener, or subscriber, must do to subscribe to 
    /// a parsers event calls
    /// </summary>
    public interface IParseListener {
        /// <summary>
        /// Called when [line parsed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogEntryItemEventArgs"/> instance containing the event data.</param>
        void OnLineParsed(object sender, LogEntryItemEventArgs e);

        /// <summary>
        /// Called when [no parse].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NoParseEventArgs"/> instance containing the event data.</param>
        void OnNoParse(object sender, NoParseEventArgs e);

        /// <summary>
        /// Called when [parse ended].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogEventArgs"/> instance containing the event data.</param>
        void OnParseEnded(object sender, LogEventArgs e);

        /// <summary>
        /// Called when [parse started].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogEventArgs"/> instance containing the event data.</param>
        void OnParseStarted(object sender, LogEventArgs e);
    }
}