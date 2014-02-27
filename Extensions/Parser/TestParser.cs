namespace Gator.Extensions.Parser {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Broos.Monitor.LogAggregator.Aggregator;
    using Broos.Monitor.LogAggregator.Entity;

    /// <summary>
    /// Test Parser than handles log entries like:
    /// @"1/2/2000 02:01:01 Testing Database Connection",
    /// @"1/2/2000 02:01:02 Attempting to Log Activity in Log Database",
    /// @"1/2/2000 02:01:04 Retrieving Last Pending Item Activity"
    /// 
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
        /// <param name="logLines">The log lines.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">logLines</exception>
        public override IList<LogEntry> Parse(IList<string> logLines) {
            if (logLines == null) {
                throw new ArgumentNullException("logLines");
            }

            var list = new List<LogEntry>();
            int startingIndex = Extras.FindStartingIndex(base.Log, logLines);
            for (int i = startingIndex; i < logLines.Count; i++) {
                if (string.IsNullOrEmpty(logLines[i])) {
                    continue;
                }

                MatchCollection matches = Regex.Matches(logLines[i], @"^(.+?\ .+?)\ (.+?)$");
                if (matches.Count <= 0) {
                    continue;
                }

                int j = i;
                foreach (
                   var logEntry in
                   from Match match in matches
                   select match.Groups into groups
                   where
                      (groups.Count > 0)
                   select new LogEntry {
                       Index = j,
                       Message = groups[2].Value,
                       Source = Log,
                       Timestamp = DateTime.Parse(groups[1].Value, CultureInfo.InvariantCulture)
                   }
                ) {
                    list.Add(logEntry);
                    this.OnLineParsed(this, new LogEntryItemEventArgs(logEntry, logLines[i]));
                }
            }

            base.Log.LastLineCount = logLines.Count();
            base.Log.LastParsedLine = logLines[startingIndex];

            return list;
        }
    }
}