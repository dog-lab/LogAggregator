namespace Broos.Monitor.LogAggregator.Aggregator {
    using System;
    using System.Collections.Generic;
    using Entity;

    /// <summary>
    /// The starting point for parsing log files. This provides a base for firing events and
    /// capturing the LogFile information for the log file to be parsed. All parsers must inherit
    /// from this file and, at a minimum (if using the Aggregator class), override the "Parse"
    /// method.
    /// </summary>
    public abstract class BaseParser : ILogParser {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseParser"/> class. The log file descriptor
        /// object is passed in to more easily assign log file information for each log entry (each line)
        /// parsed.
        /// </summary>
        /// <param name="log">The log.</param>
        protected BaseParser(LogFile log) {
            this.Log = log;
        }

        /// <summary>
        /// Code inheriting this class should invoke this event via OnLineParsed when each line is parsed.
        /// This is automatically handled if using the Blender class in the Aggregator sub project.
        /// </summary>
        public event EventHandler<LogEntryItemEventArgs> LineParsed;

        /// <summary>
        /// Code inheriting this class should invoke this event via OnNoParse if the log is *not* parsed.
        /// </summary>
        public event EventHandler<NoParseEventArgs> NoParse;

        /// <summary>
        /// Code inheriting this class should invoke this event via OnParseEnded when log parsing activity ends.
        /// This is automatically handled if using the Blender class in the Aggregator sub project.
        /// </summary>
        public event EventHandler<LogEventArgs> ParseEnded;

        /// <summary>
        /// Code inheriting this class should invoke this event via OnParseStarted when parsing activity begins for the log.
        /// This is automatically handled if using the Blender class in the Aggregator sub project.
        /// </summary>
        public event EventHandler<LogEventArgs> ParseStarted;

        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public LogFile Log { get; protected set; }

        /// <summary>
        /// Parses, or transforms, the Log File lines passed in the IList of strings. At this point
        /// the log contents are read and checked to ensure there is something new to parse.
        /// Examples of how this works are shown in the Blender class of the Aggregator sub project.
        /// </summary>
        /// <param name="logLines">The log lines.</param>
        /// <returns>
        /// An IList of LogEntry objects.
        /// </returns>
        public abstract IList<LogEntry> Parse(IList<string> logLines);

        /// <summary>
        /// Raises the <see>
        /// <cref>E:LineParsed</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogEntryItemEventArgs" /> instance containing the event data.</param>
        protected virtual void OnLineParsed(object sender, LogEntryItemEventArgs e) {
            EventHandler<LogEntryItemEventArgs> eventHandler = this.LineParsed;
            if (eventHandler == null) {
                return;
            }

            eventHandler(this, e);
        }

        /// <summary>
        /// Raises the <see>
        /// <cref>E:NoParse</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NoParseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNoParse(object sender, NoParseEventArgs e) {
            EventHandler<NoParseEventArgs> eventHandler = this.NoParse;
            if (eventHandler == null) {
                return;
            }

            eventHandler(this, e);
        }

        /// <summary>
        /// Raises the <see>
        /// <cref>E:ParseEnded</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogEventArgs" /> instance containing the event data.</param>
        protected virtual void OnParseEnded(object sender, LogEventArgs e) {
            EventHandler<LogEventArgs> eventHandler = this.ParseEnded;
            if (eventHandler == null) {
                return;
            }

            eventHandler(this, e);
        }

        /// <summary>
        /// Raises the <see>
        /// <cref>E:ParseStarted</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LogEventArgs" /> instance containing the event data.</param>
        protected virtual void OnParseStarted(object sender, LogEventArgs e) {
            EventHandler<LogEventArgs> eventHandler = this.ParseStarted;
            if (eventHandler == null) {
                return;
            }

            eventHandler(this, e);
        }
    }
}