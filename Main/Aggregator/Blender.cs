namespace Broos.Monitor.LogAggregator.Aggregator {
    using System;
    using System.Collections.Generic;
    using Entity;

    /// <summary>
    /// "Will it blend?": http://en.wikipedia.org/wiki/Will_It_Blend
    /// Combines the three components necessary to parse a log file:
    ///     1> a log file descriptor object
    ///     2> a parser object that can parse the log file text
    ///     3> a list of lines from the log file
    /// Subscribers are optional. They respond to the events produced during the parsing
    /// process. The events are:
    ///     1> LineParsed
    ///     2> NoParse
    ///     3> ParseEnded, and
    ///     4> ParseStarted
    /// </summary>
    public class Blender {
        /// <summary>
        /// Initializes a new instance of the <see cref="Blender"/> class.
        /// </summary>
        /// <param name="parser">The parser used to parse the log text.</param>
        /// <param name="subscribers">The subscribers.</param>
        /// <param name="logContent">All lines of text, contained in the log, to be parsed.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when "log" or "parser" or "logContent" are null.
        /// </exception>
        public Blender(BaseParser parser, IList<IParseListener> subscribers, IList<string> logContent) {
            BaseParser = parser;
            this.Subscribers = subscribers;
            this.LogContent = logContent;
            if (BaseParser == null) {
                throw new ArgumentNullException("parser");
            }

            if (this.LogContent == null) {
                throw new ArgumentNullException("logContent");
            }

            this.SubscribeListeners();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blender"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="subscribers">The subscribers.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when "parser" or "log" are null.
        /// </exception>
        public Blender(LogFile log, AssemblyClass parser, IEnumerable<AssemblyClass> subscribers) {
            BaseParser = Loader.LoadParser(parser.AssemblyName, parser.ClassName, log);
            this.Subscribers = Loader.LoadListeners(subscribers);
            this.LogContent = Loader.LoadLogContent(log.Location);
            if (BaseParser == null) {
                throw new ArgumentNullException("parser");
            }

            if (this.LogContent == null) {
                throw new ArgumentNullException("log");
            }
            
            this.SubscribeListeners();
        }

        /// <summary>
        /// Occurs when the log file is already parsed <see cref="IsParsed"/>.
        /// </summary>
        public event EventHandler<NoParseEventArgs> NoParse;

        /// <summary>
        /// Occurs when all parsing activity has stopped.
        /// </summary>
        public event EventHandler<LogEventArgs> ParseEnded;

        /// <summary>
        /// Occurs when the parsing process has started.
        /// </summary>
        public event EventHandler<LogEventArgs> ParseStarted;

        /// <summary>
        /// Gets or sets all lines of text in the log.
        /// </summary>
        /// <value>
        /// The contents of the log.
        /// </value>
        public IList<string> LogContent { get; set; }

        /// <summary>
        /// Gets the base parser for the log. This is used to kick off
        /// the process by calling "Parse." <seealso cref="Parse"/>
        /// </summary>
        /// <value>
        /// The base parser.
        /// </value>
        public BaseParser BaseParser { get; private set; }

        /// <summary>
        /// Gets the subscribers, or listeners, that wish to respond to the events
        /// produced by this object and the Parser object. This object produces NoParse,
        /// ParseEnded, and ParseStarted events.
        /// </summary>
        /// <value>
        /// The subscribers.
        /// </value>
        public IList<IParseListener> Subscribers { get; private set; }

        /// <summary>
        /// Parses the log file text.
        /// </summary>
        /// <returns>
        /// A list of LogEntry objects.
        /// </returns>
        public IList<LogEntry> Parse() {
            this.OnParseStarted(new LogEventArgs(BaseParser.Log));
            if (this.IsParsed()) {
                this.OnNoParse(new NoParseEventArgs(BaseParser.Log));
                this.OnParseEnded(new LogEventArgs(BaseParser.Log));

                return new List<LogEntry>();
            }

            IList<LogEntry> logEntries = BaseParser.Parse(this.LogContent);
            this.OnParseEnded(new LogEventArgs(BaseParser.Log));

            return logEntries;
        }

        /// <summary>
        /// Raises the <see><cref>E:NoParse</cref></see> event.
        /// </summary>
        /// <param name="e">The <see cref="NoParseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNoParse(NoParseEventArgs e) {
            EventHandler<NoParseEventArgs> eventHandler = this.NoParse;
            if (eventHandler == null) {
                return;
            }

            eventHandler(this, e);
        }

        /// <summary>
        /// Raises the <see><cref>E:ParseEnded</cref></see> event.
        /// </summary>
        /// <param name="e">The <see cref="LogEventArgs"/> instance containing the event data.</param>
        protected virtual void OnParseEnded(LogEventArgs e) {
            EventHandler<LogEventArgs> eventHandler = this.ParseEnded;
            if (eventHandler == null) {
                return;
            }

            eventHandler(this, e);
        }

        /// <summary>
        /// Raises the <see><cref>E:ParseStarted</cref></see> event.
        /// </summary>
        /// <param name="e">The <see cref="LogEventArgs"/> instance containing the event data.</param>
        protected virtual void OnParseStarted(LogEventArgs e) {
            EventHandler<LogEventArgs> eventHandler = this.ParseStarted;
            if (eventHandler == null) {
                return;
            }

            eventHandler(this, e);
        }

        /// <summary>
        /// Determines whether this instance is parsed by comparing the passed in Log text in this.LogContent
        /// to the previous LineCount and LastLineParsed of the last parse. The last parse values are saved
        /// in the LogFile object and tucked away in the BaseParser object created in the constructor.
        /// </summary>
        /// <returns>True if the LogFile for the passed in Parser was already parsed, other False.</returns>
        private bool IsParsed() {
            return this.LogContent[this.LogContent.Count - 1] == BaseParser.Log.LastParsedLine &&
                   this.LogContent.Count == BaseParser.Log.LastLineCount;
        }

        /// <summary>
        /// Subscribes the all listeners to the Parse (BaseParser) object. <seealso cref="BaseParser"/>
        /// </summary>
        private void SubscribeListeners() {
            foreach (IParseListener parseListener in this.Subscribers) {
                BaseParser.LineParsed += parseListener.OnLineParsed;
                this.NoParse += parseListener.OnNoParse;
                this.ParseEnded += parseListener.OnParseEnded;
                this.ParseStarted += parseListener.OnParseStarted;
            }
        }
    }
}