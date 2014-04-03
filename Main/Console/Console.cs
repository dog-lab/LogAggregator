namespace Broos.Monitor.LogAggregator.Console {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Aggregator;
    using Newtonsoft.Json;

    /// <summary>
    /// Command Line version of the Log Aggregator
    /// </summary>
    public static class GatorConsole {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">The array of arguments passed in on the command line.</param>
        /// <exception cref="System.ArgumentNullException">command line arguments: args</exception>
        public static void Main(string[] args) {
            ConsoleArguments arg = ConsoleArguments.Create(args);
            string loaderData = arg.In;

            if (!File.Exists(loaderData)) {
                Console.WriteLine(Resource.FileNotFound, loaderData);
                Environment.Exit(1);
            }

            IList<GatorData> sources = JsonConvert.DeserializeObject<IList<GatorData>>(
                File.ReadAllText(loaderData)
            );

            foreach (GatorData source in sources) {
                new Blender(
                    Loader.LoadParser(
                        source.Parser.AssemblyName,
                        source.Parser.ClassName,
                        source
                    ),
                    Loader.LoadListeners(source.Listeners),
                    Loader.LoadLogContent(source.Location)
                ).Parse();
            }
        }
    }
}