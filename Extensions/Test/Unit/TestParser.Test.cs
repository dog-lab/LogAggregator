namespace Gator.Extensions.Test.Unit {
    using System;
    using System.Collections.Generic;
    using Broos.Monitor.LogAggregator.Entity;
    using NUnit.Framework;
    using Parser;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class When_a_test_parser_is_used {
        private List<string> _logData = new List<string> {
            @"1/2/2000 02:01:01 Testing Database Connection",
            @"1/2/2000 02:01:02 Attempting to Log Activity in Log Database",
            @"1/2/2000 02:01:04 Retrieving Last Pending Item Activity"
        };
            
        [Test]
        public void It_notifies_for_each_line_parsed() {
            var parser = new Gator.Extensions.Parser.TestParser(GetUnparsedSource());
            var lines = 0;
            parser.LineParsed += (o, e) => lines += 1;
            parser.Parse(_logData);

            Assert.IsTrue(lines == 3);
        }

        [Test]
        public void It_returns_a_list_of_LogEntries_for_the_entire_log() {
            var parser = new TestParser(GetUnparsedSource());
            IList<LogEntry> entries = parser.Parse(_logData);

            Assert.IsTrue(entries.Count.Equals(3));            
        }

        [Test]
        public void It_returns_correct_number_of_entries_given_a_starting_index() {
            var log = GetUnparsedSource();
            log.LastParsedLine = @"1/2/2000 02:01:01 Testing Database Connection";
            var parser = new TestParser(log);
            IList<LogEntry> entries = parser.Parse(_logData);

            // indexes are zero based - remember to add one to the starting index to get a correct count
            Assert.IsTrue(entries.Count.Equals(_logData.Count - Extras.FindStartingIndex(log, _logData) + 1));
        }
    
        private static LogFile GetUnparsedSource() {
            return new LogFile {
                Id = 1,
                HostName = "ComputerName",
                LastParsedLine = string.Empty,
                LastTimestamp = DateTime.Now,
                LastLineCount = 0,
                Location = string.Empty,
                Name = "Test Log"
            };
        }
    }
}
