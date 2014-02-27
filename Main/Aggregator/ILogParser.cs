namespace Broos.Monitor.LogAggregator.Aggregator {
    using System;
    using System.Collections.Generic;
    using Entity;

    /// <summary>
    /// Defines a contract for parsing a file.
    /// </summary>
    public interface ILogParser {
        /// <summary>
        /// Code inheriting this class should invoke this event via OnLineParsed when each line is parsed.
        /// This is automatically handled if using the Blender class in the Aggregator sub project.
        /// </summary>
        event EventHandler<LogEntryItemEventArgs> LineParsed;

        /// <summary>
        /// Code inheriting this class should invoke this event via OnNoParse when each the log is *not* parsed.
        /// </summary>
        event EventHandler<NoParseEventArgs> NoParse;

        /// <summary>
        /// Code inheriting this class should invoke this event via OnParseEnded when log parsing activity ends.
        /// This is automatically handled if using the Blender class in the Aggregator sub project.
        /// </summary>
        event EventHandler<LogEventArgs> ParseEnded;

        /// <summary>
        /// Code inheriting this class should invoke this event via OnParseStarted when parsing activity begins for the log.
        /// This is automatically handled if using the Blender class in the Aggregator sub project.
        /// </summary>
        event EventHandler<LogEventArgs> ParseStarted;

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        LogFile Log { get; }

        /// <summary>
        /// Parses, or transforms, the Log File passed in as a list of strings. At this point
        /// the log contents are read and checked to ensure there is something new to parse.
        /// Examples of how this works are shown in the Blender class of the Aggregator sub project.
        /// </summary>
        /// <param name="logLines">All lines of the log contained in an IList of strings.</param>
        /// <returns>
        /// All parsed lines in a generic IList of LogEntry objects.
        /// </returns>
        IList<LogEntry> Parse(IList<string> logLines);
    }
}
