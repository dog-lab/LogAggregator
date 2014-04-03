namespace Gator.Extensions.Parser {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Text.RegularExpressions;
    using Broos.Monitor.LogAggregator.Aggregator;
    using Broos.Monitor.LogAggregator.Entity;

    /// <summary>
    /// Parses a legacy windows scheduled task parser. This version is pretty much a hack
    /// ...need to revisit and clean up the logic (needs a rewrite).
    /// simple completion entry:
    /// state: New
    /// Job name (executable) - state: Message
    /// Message - state: Result
    /// Result - state: Complete
    /// @"""50_Archive_Data_Files.job"" (Archive_Data_Files.bat)",
    /// @"      Finished 2/5/2010 11:00:01 PM",
    /// @"      Result: The task completed with an exit code of (0).",
    /// warning entry:
    /// state: New
    /// Job name (executable) date/time message type - state: Message
    /// Message - state: SpecificError
    /// SpecificError - state: Error
    /// Error - state: Complete
    /// @"""15_Gateway_Start_TRN_New.job"" (Start_Service.bat) 2/22/2010 5:48:33 AM ** WARNING **",
    /// @"      Unable to update the task.",
    /// @"      The specific error is:",
    /// @"      0x00000020: The process cannot access the file because it is being used by another process.",
    /// simple started entry:
    /// state: New
    /// Job name (executable - state: Message
    /// Message - state: Complete
    /// @"""50_Move_Accounting_Logs.job"" (Move_Accounting_Logs.bat)",
    /// @"      Started 2/6/2010 2:30:00 PM",
    /// @"""50_Move_Accounting_Logs.job"" (Move_Accounting_Logs.bat)",
    /// @"      Finished 2/6/2010 2:30:00 PM",
    /// @"      Result: The task completed with an exit code of (1).",
    /// @"[ ***** Most recent entry is above this line ***** ]",
    /// @"",
    /// @"",
    /// @"""17-App-MiddlewareCheck.job"" (MiddlewareCheck.exe)",
    /// @"      Finished 2/5/2010 12:41:01 PM",
    /// @"      Result: The task completed with an exit code of (0).",
    /// @"""06_Post_Fund_Transactions.job"" (FiatScheduler.exe)",
    /// @"      Finished 2/5/2010 12:42:32 PM",
    /// @"      Result: The task completed with an exit code of (0).",
    /// error entry:
    /// state: New
    /// Job name (executable) date/time message type - state: Message
    /// Message - state: SpecificError
    /// SpecificError - state: Error
    /// Error - state: Message2
    /// Message2 - state: Complete
    /// @"""18_Gateway_Stop_TRN.job"" (Stop_Service.bat) 2/23/2010 2:15:00 PM ** ERROR **",
    /// @"      The attempt to log on to the account associated with the task failed, therefore, the task did not run.",
    /// @"      The specific error is:",
    /// @"      0x8007052e: Logon failure: unknown user name or bad password.",
    /// @"      Verify that the task's Run-as name and password are valid and try again."
    /// Pass in Line, ExpectedRex, LogEntry
    /// Return LogEntry, ExpectedRex
    /// 1&gt; TryParse line w/ExpectedRex(s)
    /// 2&gt; If OK
    /// 3&gt;
    /// Parse w/Rex(s)
    /// Return LogEntry and new ExpectedRex
    /// 4&gt; Otherwise
    /// Return LogEntry and Exception
    /// ParseNew (Parse New/New2)
    /// ParseMessage (Started/Finished/Plain)
    /// ParseResult
    /// ParseSpecificError
    /// ParseError
    /// ParseMessage2
    /// ParseComplete
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Underscore already signals a local field.")]
    public class WinTaskParser : BaseParser {
        /// <summary>
        /// The log error string Regular Expression
        /// </summary>
        private const string ErrorRex = @"^\s*(.+?):\ (.+?)$";

        /// <summary>
        /// The finished Regular Expression
        /// </summary>
        private const string FinishedRex = @"^\s*Finished\ (.+?)$";
        
        /// <summary>
        /// The basic new job Regular Expression
        /// </summary>
        private const string NewJobRex = @"^""(.+?)\.job""\ \((.+?)\)";
        
        /// <summary>
        /// The warning and error job Regular Expression
        /// </summary>
        private const string NewJobRex2 = @"^""(.+?)\.job""\ \((.+?)\)\ (.+?\ .+?\ .+?)\ (.+?)$";
        
        /// <summary>
        /// The job result Regular Expression
        /// </summary>
        private const string ResultRex = @"^\s*Result:\ The\ task\ completed\ with\ an\ exit\ code\ of\ \((.+?)\)\.$";
        
        /// <summary>
        /// The started Regular Expression
        /// </summary>
        private const string StartedRex = @"^\s*Started\ (.+?)$";

        /// <summary>
        /// Stores additional data as a JSON string
        /// </summary>
        private StringBuilder _extensions = new StringBuilder();

        /// <summary>
        /// The Log Entry index
        /// </summary>
        private int _index = -1;

        /// <summary>
        /// The Log Entry built while parsing the log
        /// </summary>
        private LogEntry _logEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinTaskParser" /> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public WinTaskParser(LogFile log) : base(log) {
        }

        /// <summary>
        /// State descriptor for the expected log entry Regular Expression (see regular expression constants above).
        /// </summary>
        private enum ExpectedRex {
            /// <summary>
            /// The new
            /// </summary>
            New,

            /// <summary>
            /// The complete
            /// </summary>
            Complete,
            
            /// <summary>
            /// The message
            /// </summary>
            Message,
            
            /// <summary>
            /// The result
            /// </summary>
            Result,
            
            /// <summary>
            /// The specific error
            /// </summary>
            SpecificError,
            
            /// <summary>
            /// The error
            /// </summary>
            Error,
            
            /// <summary>
            /// The message2
            /// </summary>
            Message2
        }

        /// <summary>
        /// Parses the specified log lines.
        /// </summary>
        /// <param name="logLines">The log lines.</param>
        /// <returns>A list of zero, or more, LogEntry objects.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the logLines argument is null.</exception>
        public override IList<LogEntry> Parse(IList<string> logLines) {
            if (logLines == null) {
                throw new ArgumentNullException("logLines");
            }

            if (logLines.Count == 0) {
                return new List<LogEntry>();
            }

            // rearrange the log so that the oldest entries are at the top of the list
            var lineArray = new string[logLines.Count];
            logLines.CopyTo(lineArray, 0);
            var lines = new List<string>(NormalizeLog(string.Join("\r\n", lineArray).Replace("\r\n\t", " ").Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)));

            var list = new List<LogEntry>();
            int startingIndex = Extras.FindStartingIndex(Log, lines);

            var expectedRex = ExpectedRex.New;
            for (int i = startingIndex; i < lines.Count; i++) {
                if (string.IsNullOrEmpty(lines[i].Trim())) {
                    continue;
                }

                // New - create (reinitialize) new log entry object
                if (expectedRex == ExpectedRex.New) {
                    _logEntry = new LogEntry();                    
                }

                // Complete - add log entry to list and setup for new log entry
                if (expectedRex == ExpectedRex.Complete) {
                    if (_extensions.Length > 0) {
                        _extensions.Append("}");
                        _logEntry.Extensions = _extensions.ToString();
                        _extensions = new StringBuilder();
                    }

                    // White space at the beginning of the line? Not quite done yet...
                    var regex = new Regex(@"^\s");
                    if (regex.IsMatch(lines[i])) {
                        expectedRex = ExpectedRex.Message2;
                    } else {
                        list.Add(_logEntry);
                        expectedRex = ExpectedRex.New;
                    }

                    this.OnLineParsed(this, new LogEntryItemEventArgs(_logEntry, lines[i]));
                }

                // continue parsing until ExpectedRex.Complete
                expectedRex = ParseLine(lines[i], expectedRex);
            }

            // the last log entry falls out of the for loop before it's 
            // saved...so grab any extension data, post it to the log entry
            // and save the log entry to the list.
            if (_extensions.Length > 0) {
                _extensions.Append("}");
                _logEntry.Extensions = _extensions.ToString();
            }

            list.Add(_logEntry);

            return list;
        }

        /// <summary>
        /// Normalizes the log file structure.
        /// </summary>
        /// <param name="lines">An array of strings.</param>
        /// <returns>An array of strings meant to replace the passed in array.</returns>
        private static IEnumerable<string> NormalizeLog(string[] lines) {
            for (int i = 0; i < lines.Length; i++) {
                MatchCollection matches = Regex.Matches(lines[i], @"^\[ \*\*\*\*\* Most recent entry is above this line \*\*\*\*\* \]$");
                if (i == lines.Length - 1) {
                    return lines;
                }

                if (matches.Count > 0) {
                    var newLines = new List<string>();

                    for (int j = i + 1; j < lines.Length; j++) {
                        if (string.IsNullOrEmpty(lines[j])) {
                            continue;
                        }

                        newLines.Add(lines[j]);
                    }

                    for (int j = 0; j < i; j++) {
                        if (string.IsNullOrEmpty(lines[j])) {
                            continue;
                        }

                        newLines.Add(lines[j]);
                    }

                    return newLines.ToArray();
                }
            }

            return lines;
        }

        /// <summary>
        /// Parses the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="expectedRex">The expected regular expression.</param>
        /// <returns>The new expected Regular Expression descriptor.</returns>
        private ExpectedRex ParseLine(string line, ExpectedRex expectedRex) {
            switch (expectedRex) {
                case ExpectedRex.New:
                    return ParseNew(line);
                case ExpectedRex.Message:
                    return ParseMessage(line);
                case ExpectedRex.Result:
                    return ParseResult(line);
                case ExpectedRex.SpecificError:
                    return ExpectedRex.Error;
                case ExpectedRex.Message2:
                    return ExpectedRex.Complete;
                case ExpectedRex.Error:
                    return ParseError(line);
            }

            return ExpectedRex.Complete;
        }

        /// <summary>
        /// Parses the error.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The new expected Regular Expression descriptor.</returns>
        private ExpectedRex ParseError(string line) {
            MatchCollection matches = Regex.Matches(line, ErrorRex);
            Debug.Assert(_logEntry != null, "logEntry != null");
            _extensions.Append(string.Format(@", ""ErrorCode"": ""{0}""", matches[0].Groups[1].Value));
            _extensions.Append(string.Format(@", ""ErrorMessage"": ""{0}""", matches[0].Groups[2].Value));

            return ExpectedRex.Complete;            
        }

        /// <summary>
        /// Parses the message.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The new expected Regular Expression descriptor.</returns>
        private ExpectedRex ParseMessage(string line) {
            if (Regex.IsMatch(line, FinishedRex)) {
                MatchCollection matches = Regex.Matches(line, FinishedRex);
                Debug.Assert(_logEntry != null, "logEntry != null");
                _logEntry.Timestamp = DateTime.Parse(matches[0].Groups[1].Value);

                return ExpectedRex.Result;
            }

            if (Regex.IsMatch(line, StartedRex)) {
                MatchCollection matches = Regex.Matches(line, StartedRex);
                Debug.Assert(_logEntry != null, "logEntry != null");
                _logEntry.Timestamp = DateTime.Parse(matches[0].Groups[1].Value);

                return ExpectedRex.Complete;
            }

            _logEntry.Message = line;
            return ExpectedRex.SpecificError;
        }

        /// <summary>
        /// Parses the new.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The new expected Regular Expression descriptor.</returns>
        private ExpectedRex ParseNew(string line) {
            if (Regex.IsMatch(line, NewJobRex)) {
                if (Regex.IsMatch(line, NewJobRex2)) {
                    MatchCollection matches = Regex.Matches(line, NewJobRex2);

                    _logEntry = new LogEntry {
                        Index = _index++,
                        Source = Log,
                        Timestamp = DateTime.Parse(matches[0].Groups[3].Value)
                    };

                    _extensions.Append(string.Format(@"{{ ""Job"": ""{0}""", matches[0].Groups[1].Value));
                    _extensions.Append(
                        string.Format(
                            @", ""MessageType"": ""{0}""",
                            matches[0].Groups[4].Value.Replace('*', ' ').Trim()
                        )
                    );
                } else {
                    MatchCollection matches = Regex.Matches(line, NewJobRex);

                    _logEntry = new LogEntry {
                        Index = _index++,
                        Source = Log
                    };

                    _extensions.Append(string.Format(@"{{ ""Job"": ""{0}""", matches[0].Groups[1].Value));
                }
            }

            return ExpectedRex.Message;
        }

        /// <summary>
        /// Parses the result.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The new expected Regular Expression descriptor.</returns>
        private ExpectedRex ParseResult(string line) {
            MatchCollection matches = Regex.Matches(line, ResultRex);
            Debug.Assert(_logEntry != null, "logEntry != null");
            _extensions.Append(string.Format(@", ""ExitCode"": ""{0}""", matches[0].Groups[1].Value));

            return ExpectedRex.Complete;
        }
    }
}
