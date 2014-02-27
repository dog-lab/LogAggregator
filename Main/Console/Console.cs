namespace Broos.Monitor.LogAggregator.Console {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
            string loaderData = "gator-console.json";
            if (args != null) {
                if (args.Length > 0) {
                    loaderData = args[0];
                }
            }

            var sources = JsonConvert.DeserializeObject<IList<GatorData>>(
                File.ReadAllText(loaderData)
            );

            foreach (
                var blender in sources.Select(
                    source => new Blender(
                        Loader.LoadParser(
                            source.Parser.AssemblyName,
                            source.Parser.ClassName,
                            source
                        ),
                        Loader.LoadListeners(source.Listeners),
                        Loader.LoadLogContent(source.Location)
                    )
                )
            ) {
                blender.Parse();
            }
        }
    }
}