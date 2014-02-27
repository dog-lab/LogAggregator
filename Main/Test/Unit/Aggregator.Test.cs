namespace Broos.Monitor.LogAggregator.Test.Unit {
    using System;
    using Aggregator;
    using Entity;
    using Moq;
    using NUnit.Framework;

    // when a parser loader is used
    //  it assigns all listeners to the IParseListener events
    //  it throws an exception on an invalid stream argument
    //  it throws an exception if no parser is provided

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class When_an_aggregator_is_used {
        private System.Collections.Generic.List<string> _logData = new System.Collections.Generic.List<string> {
            @"Info Jan 02 2000 02:00 Testing Database Connection",
            @"Info Jan 02 2000 02:00 Attempting to Log Activity in Log Database",
            @"Info Jan 02 2000 02:00 Retrieving Last Pending Item Activity"
        };

        [Test]
        public void It_notifies_when_the_log_is_not_parsed() {
            var parser = new Mock<BaseParser>(GetParsedSource());
            var listeners = new Mock<System.Collections.Generic.List<IParseListener>>();
            // ReSharper disable once ObjectCreationAsStatement
            // new Mock<System.Collections.Generic.List<string>>();
            var wasCalled = false;

            var aggregator = new Blender(parser.Object, listeners.Object, _logData);
            aggregator.NoParse += (o, e) => wasCalled = true;
            aggregator.Parse();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void It_throws_an_exception_on_a_null_parser() {
            var listeners = new Mock<System.Collections.Generic.List<IParseListener>>();
            // ReSharper disable once ObjectCreationAsStatement
            // new Mock<System.Collections.Generic.List<string>>();
            var ex = Assert.Throws<ArgumentNullException>(() => new Blender(null, listeners.Object, _logData));
            Assert.That(ex.ParamName, Is.EqualTo("parser"));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Broos.Monitor.LogAggregator.Aggregator.Blender"), Test]
        public void It_throws_an_exception_on_null_log_content() {
            var parser = new Mock<BaseParser>(GetUnparsedSource());
            var listeners = new Mock<System.Collections.Generic.List<IParseListener>>();
            // ReSharper disable once ObjectCreationAsStatement
            // new Mock<System.Collections.Generic.List<string>>();
            var ex = Assert.Throws<ArgumentNullException>(() => new Blender(parser.Object, listeners.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("logContent"));
        }

        [Test]
        public void It_notifies_that_parsing_has_started() {
            var parser = new Mock<BaseParser>(GetUnparsedSource());
            var listeners = new Mock<System.Collections.Generic.List<IParseListener>>();
            // ReSharper disable once ObjectCreationAsStatement
            // new Mock<System.Collections.Generic.List<string>>();
            var wasCalled = false;

            var aggregator = new Blender(parser.Object, listeners.Object, _logData);
            aggregator.ParseStarted += (o, e) => wasCalled = true;
            aggregator.Parse();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void It_notifies_that_parsing_has_ended() {
            var parser = new Mock<BaseParser>(GetUnparsedSource());
            var listeners = new Mock<System.Collections.Generic.List<IParseListener>>();
            // ReSharper disable once ObjectCreationAsStatement
            // new Mock<System.Collections.Generic.List<string>>();
            var wasCalled = false;

            var aggregator = new Blender(parser.Object, listeners.Object, _logData);
            aggregator.ParseEnded += (o, e) => wasCalled = true;
            aggregator.Parse();

            Assert.IsTrue(wasCalled);
        }

        private static LogFile GetParsedSource() {
            return new LogFile {
                Id = 1,
                HostName = "TestHostName",
                LastParsedLine = @"Info Jan 02 2000 02:00 Retrieving Last Pending Item Activity",
                LastTimestamp = DateTime.Now,
                LastLineCount = 3,
                Location = string.Empty,
                Name = "Test LogSource"
            };
        }

        private static LogFile GetUnparsedSource() {
            return new LogFile {
                Id = 1,
                HostName = "TestHostName",
                LastParsedLine = string.Empty,
                LastTimestamp = DateTime.Now,
                LastLineCount = 0,
                Location = string.Empty,
                Name = "Test LogSource"
            };
        }
    }
}
