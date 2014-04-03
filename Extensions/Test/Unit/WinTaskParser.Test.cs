namespace Gator.Extensions.Test.Unit {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Broos.Monitor.LogAggregator.Entity;
    using NUnit.Framework;
    using Parser;

    /// <summary>
    /// Parser for windows scheduled task logs (SchedLgU.txt).
    /// </summary>
    // ReSharper disable InconsistentNaming
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "K&R Style is a valid code format.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1122:UseStringEmptyForEmptyStrings", Justification = "Blank lines are valid in sample data.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Underscore already signals a local field.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "ShedLgU is a filename...it is spelled correctly.")]
    [TestFixture]
    public class When_a_win_task_parser_is_used {
        /// <summary>
        /// Sample Log data
        /// </summary>
        private List<string> _logData = new List<string> {
            @"""50_Archive_Data_Files.job"" (Archive_Data_Files.bat)",
            @"      Finished 2/5/2010 11:00:01 PM",
            @"      Result: The task completed with an exit code of (0).",
            @"""15_Gateway_Start_TRN_New.job"" (Start_Service.bat) 2/22/2010 5:48:33 AM ** WARNING **",
            @"      Unable to update the task.",
            @"      The specific error is:",
            @"      0x00000020: The process cannot access the file because it is being used by another process.",
            @"""50_Move_Accounting_Logs.job"" (Move_Accounting_Logs.bat)",
            @"      Started 2/6/2010 2:30:00 PM",
            @"""50_Move_Accounting_Logs.job"" (Move_Accounting_Logs.bat)",
            @"      Finished 2/6/2010 2:30:00 PM",
            @"      Result: The task completed with an exit code of (1).",
            @"[ ***** Most recent entry is above this line ***** ]",
            @"",
            @"",                                        
            @"""17-App-MiddlewareCheck.job"" (MiddlewareCheck.exe)",
            @"      Finished 2/5/2010 12:41:01 PM",
            @"      Result: The task completed with an exit code of (0).",
            @"""06_Post_Fund_Transactions.job"" (FiatScheduler.exe)",
            @"      Finished 2/5/2010 12:42:32 PM",
            @"      Result: The task completed with an exit code of (0).",
            @"""18_Gateway_Stop_TRN.job"" (Stop_Service.bat) 2/23/2010 2:15:00 PM ** ERROR **",
            @"      The attempt to log on to the account associated with the task failed, therefore, the task did not run.",
            @"      The specific error is:",
            @"      0x8007052e: Logon failure: unknown user name or bad password.",
            @"      Verify that the task's Run-as name and password are valid and try again."
        };

        /// <summary>
        /// It_notifies_for_each_line_parseds this instance.
        /// </summary>
        [Test]
        public void It_notifies_for_each_line_parsed() {
            var parser = new WinTaskParser(GetSource());
            var lines = 0;
            parser.LineParsed += (o, e) => lines += 1;
            parser.Parse(_logData);

            Assert.IsTrue(lines == 7);
        }

        /// <summary>
        /// It_returns_a_list_of_s the log entries_for_the_entire_log.
        /// </summary>
        [Test]
        public void It_returns_a_list_of_LogEntries_for_the_entire_log() {
            var parser = new WinTaskParser(GetSource());
            IList<LogEntry> entries = parser.Parse(_logData);

            Assert.IsTrue(entries.Count.Equals(7));
        }

        /*
        [Test]
        public void It_returns_correct_number_of_entries_given_a_starting_index() {
            var log = GetSource();
            log.LastParsedLine = @"1/2/2000 02:01:01 Testing Database Connection";
            var parser = new TestParser(log);
            IList<LogEntry> entries = parser.Parse(_logData);

            // indexes are zero based - remember to add one to the starting index to get a correct count
            Assert.IsTrue(entries.Count.Equals(_logData.Count - Extras.FindStartingIndex(log, _logData) + 1));
        }
        */

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <returns>A test LogFile object.</returns>
        private static LogFile GetSource() {
            return new LogFile {
                Id = 1,
                HostName = "ComputerName",
                LastParsedLine = string.Empty,
                LastTimestamp = DateTime.Now,
                LastLineCount = 0,
                Location = string.Empty,
                Name = "Win Task Log"
            };
        }
    }
}
