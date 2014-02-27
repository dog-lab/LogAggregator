namespace Gator.Extensions.Parser {
    using System.Collections.Generic;
    using Broos.Monitor.LogAggregator.Entity;

    /// <summary>
    /// Methods that might prove useful in log parsing classes
    /// </summary>
    public class Extras {
        /// <summary>
        /// Finds the LogContent index to begin parsing the log file. This is necessary for cases
        /// when the log file had content added since the last parse or for circular log files.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logContent">Content of the log.</param>
        /// <returns>
        /// The starting index to begin the parse, otherwise zero to begin parsing from the beginning of the log.
        /// </returns>
        public static int FindStartingIndex(LogFile log, IList<string> logContent) {
            for (int index = 0; index < logContent.Count; index++) {
                if (logContent[index] == log.LastParsedLine) {
                    return index + 1;
                }
            }

            return 0;
        }
    }
}
