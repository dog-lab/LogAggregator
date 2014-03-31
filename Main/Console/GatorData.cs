namespace Broos.Monitor.LogAggregator.Console {
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Entity;

    /// <summary>
    /// DTO for loading data needed to parse one, or more, logs
    /// </summary>
    public class GatorData : LogFile {
        /// <summary>
        /// AssemblyClass object that describes a Parser object.
        /// </summary>
        private AssemblyClass _parser;

        /// <summary>
        /// The list of _listener assembly paths and class names that will be created via reflection
        /// and assigned 
        /// </summary>
        private List<AssemblyClass> _listeners = new List<AssemblyClass>();

        /// <summary>
        /// Gets or sets the assembly name and class name needed to instantiate a parser.
        /// </summary>
        /// <value>
        /// An assembly name and class name that, when instantiated, creates a parser object
        /// that will handle the log file identified in the Log property (in the base class).
        /// </value>
        public AssemblyClass Parser {
            get { return _parser; }

            set { _parser = value; }
        }

        /// <summary>
        /// Gets the assembly name(s) and class name(s) need to instantiate
        /// any listeners attached to the parser object.
        /// </summary>
        /// <value>
        /// A list of Assembly Names and associated Class Names.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Underscore already signals a local field.")]
        public IList<AssemblyClass> Listeners {
            get {
                return _listeners;
            }
        }
    }
}
