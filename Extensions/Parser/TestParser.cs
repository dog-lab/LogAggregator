namespace Gator.Extensions.Parser {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Broos.Monitor.LogAggregator.Aggregator;
    using Broos.Monitor.LogAggregator.Entity;

    /// <summary>
    /// Test Parser than handles log entries like:
    /// @"1/2/2000 02:01:01 Testing Database Connection",
    /// @"1/2/2000 02:01:02 Attempting to Log Activity in Log Database",
    /// @"1/2/2000 02:01:04 Retrieving Last Pending Item Activity"
    /// </summary>
    public class TestParser : BaseParser {
       /// <summary>
        /// Initializes a new instance of the <see cref="TestParser"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public TestParser(LogFile log) : base(log) {
        }

        /// <summary>
        /// Parses the specified log lines and fires an OnLineParsed event for each line parsed.
        /// </summary>
        /// <param name="logLines">Lines of text from the log.</param>
        /// <returns>A list of LogEntry objects containing data from the parsed log lines.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the logLines argument is null.</exception>
        public override IList<LogEntry> Parse(IList<string> logLines) {
            if (logLines == null) {
                throw new ArgumentNullException("logLines");
            }

            List<LogEntry> list = new List<LogEntry>();
            int startingIndex = Extras.FindStartingIndex(Log, logLines);
            for (int i = startingIndex; i < logLines.Count; i++) {
                if (string.IsNullOrEmpty(logLines[i])) {
                    continue;
                }

                // parse out date/time and message parts of the log
                MatchCollection matches = Regex.Matches(logLines[i], @"^(.+?\ .+?)\ (.+?)$");
                if (matches.Count <= 0) {
                    continue;
                }

                // only capture entries that match the regular expression and add them to the LogEntry list.
                foreach (Match match in matches) {
                    if (match.Groups.Count > 0) {
                        LogEntry logEntry = new LogEntry();
                        logEntry.Index = i;
                        logEntry.Message = match.Groups[2].Value;
                        logEntry.Source = Log;
                        logEntry.Timestamp = DateTime.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                        list.Add(logEntry);
                        this.OnLineParsed(this, new LogEntryItemEventArgs(logEntry, logLines[i]));
                    }
                }
            }

            // capture data thats useful on subsequent runs over the same log file.
            Log.LastLineCount = logLines.Count;
            Log.LastParsedLine = logLines[startingIndex];

            return list;
        }
    }
}