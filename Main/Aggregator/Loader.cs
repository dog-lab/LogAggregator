namespace Broos.Monitor.LogAggregator.Aggregator {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Entity;

    /// <summary>
    /// A helper class for loading parser instances, subscriber instances, and log file text into,
    /// respectively, a base parser instance, a list of subscribers (listeners), and list of strings.
    /// </summary>
    public class Loader {
        /// <summary>
        /// Loads the listener.
        /// </summary>
        /// <param name="assemblyName">Path to the assembly containing the listener class.</param>
        /// <param name="className">Name of the listener class in the assembly.</param>
        /// <returns>A new Parse Listener object.</returns>
        /// <exception cref="ParseCodeException">
        /// Thrown when a valid BaseParser object cannot be created.
        /// </exception>
        public static IParseListener LoadListener(string assemblyName, string className) {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyName));
            if (assembly == null) {
                throw new ParseCodeException(string.Format("Assembly {0} not loaded", assemblyName));
            }

            Type type = assembly.GetType(className, true);
            if (type == null) {
                throw new ParseCodeException(string.Format("{0} not found in assembly: {1}", className, assemblyName));
            }

            var parseListener = Activator.CreateInstance(type) as IParseListener;
            if (parseListener == null) {
                throw new ParseCodeException(string.Format("{0} does not implement IParseListener", type.Name));
            }

            return parseListener;
        }

        /// <summary>
        /// Loads the listeners that wish to respond to the events produced by the Aggregator and Parser objects.
        /// </summary>
        /// <param name="subscribers">The subscribers to events produced by the Aggregator and Parser objects.</param>
        /// <returns>A list of Parse Listener objects.</returns>
        public static IList<IParseListener> LoadListeners(IEnumerable<AssemblyClass> subscribers) {
            return subscribers.Select(subscriber => LoadListener(subscriber.AssemblyName, subscriber.ClassName)).ToList();
        }

        /// <summary>
        /// Loads the content of the log file into a list of strings.
        /// </summary>
        /// <param name="fileName">Path to the log file.</param>
        /// <returns>All of the LogFile text as a list of strings. One list item for each line in the log file.</returns>
        public static IList<string> LoadLogContent(string fileName) {
            return File.ReadAllLines(fileName).Where(s => s.Trim() != string.Empty).ToList();
        }

        /// <summary>
        /// Loads the parser object described in the assembly path and class name.
        /// </summary>
        /// <param name="assemblyName">Path to the assembly containing the listener class.</param>
        /// <param name="className">Name of the listener class in the assembly.</param>
        /// <param name="log">The log file descriptor object.</param>
        /// <returns>A new log Parser object.</returns>
        /// <exception cref="ParseCodeException">
        /// Thrown when a valid IParseListener object cannot be created.
        /// </exception>
        public static BaseParser LoadParser(string assemblyName, string className, LogFile log) {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyName));
            if (assembly == null) {
                throw new ParseCodeException(string.Format("Assembly {0} not loaded", assemblyName));
            }
            
            Type type = assembly.GetType(className, true);
            if (type == null) {
                throw new ParseCodeException(string.Format("{0} not found in assembly: {1}", className, assemblyName));
            }

            var baseParser = Activator.CreateInstance(type, new object[] { log }) as BaseParser;
            if (baseParser == null) {
                throw new ParseCodeException(string.Format("{0} does not inherit from BaseParser", type.Name));
            }

            return baseParser;
        }
    }
}