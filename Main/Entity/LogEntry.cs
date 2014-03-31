namespace Broos.Monitor.LogAggregator.Entity {
    using System;

    /// <summary>
    /// The output object from the parsed log file. This is the whole point
    /// of the project...finding and creating a useful common ground for
    /// log file data. We can query this data to help us find,
    /// and hopefully solve, problems or perhaps find a
    /// trend or two...
    /// </summary>
    public class LogEntry {
        /// <summary>
        /// Identifier of this Log Entry object - like the ID field of a SQL server identity field.
        /// </summary>
        private int _id;
        
        /// <summary>
        /// Extended information for the Log Entry;
        /// </summary>
        private string _extensions = string.Empty;

        /// <summary>
        /// Line index into the log line list for this Log Entry object.
        /// </summary>
        private int _index;

        /// <summary>
        /// The message part of the Log Entry.
        /// </summary>
        private string _message = string.Empty;

        /// <summary>
        /// The source of the log.
        /// </summary>
        private LogFile _source;

        /// <summary>
        /// The Date and Time of the Log Entry.
        /// </summary>
        private DateTime _timestamp = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the unique identifier for this entry. If
        /// originated from a database, the "ID field."
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id {
            get { return _id; }

            set { _id = value; }
        }

        /// <summary>
        /// Gets or sets any extended information contained in the log
        /// text "line" or entry. This is meant to break out data we wish
        /// to further analyze. One could use this, perhaps, to store
        /// data further parsed in, let's say, JSON or some other
        /// format. In other words, this is a free form data field so
        /// have a "field day" with it.
        /// </summary>
        /// <value>
        /// The extended data.
        /// </value>
        public string Extensions {
            get { return _extensions;  }

            set { _extensions = value; }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index {
            get { return _index; }

            set { _index = value; }
        }

        /// <summary>
        /// Gets or sets the message part of the log text "line" or entry.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message {
            get { return _message; }

            set { _message = value; }
        }

        /// <summary>
        /// Gets or sets the LogFile object that owns this LogEntry.
        /// </summary>
        /// <value>
        /// The LogFile source.
        /// </value>
        public LogFile Source {
            get { return _source; }

            set { _source = value; }
        }

        /// <summary>
        /// Gets or sets the timestamp part of the log text "line" or entry
        /// (if a time stamp exists).
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp {
            get { return _timestamp; }

            set { _timestamp = value; }
        }
    }
}
