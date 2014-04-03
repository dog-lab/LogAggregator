namespace Broos.Monitor.LogAggregator.Console {
    using System.ComponentModel;
    using Ookii.CommandLine;

    public class ConsoleArguments {
        /// <summary>
        /// The display help indicator. 
        /// </summary>
        private bool _help;

        /// <summary>
        /// The _in argument value variable.
        /// </summary>
        private string _in;

        /// <summary>
        /// Gets or sets the in argument value.
        /// </summary>
        /// <value>
        /// The in argument value.
        /// </value>
        [CommandLineArgument(DefaultValue = "gator-console.json", IsRequired = false, ValueDescription = "Input file name. If not given \"gator-console.json\" is used.")]
        public string In {
            get { return _in; }

            set { _in = value; }
        }
    
        // This property defines a switch argument named "Help", with the alias "?".
        // For this argument, we handle the CommandLineParser.ArgumentParsed event to cancel
        // command line processing when this argument is supplied. That way, we can print usage regardless of what other arguments are
        // present. For more details, see the CommandLineParser.ArgumentParser event handler in Program.cs
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConsoleArguments"/> is help.
        /// </summary>
        /// <value>
        ///   <c>true</c> if help; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Comments above are taken directly from the Ookii SampleArguments example for the "Help" property.</remarks>
        [CommandLineArgument, Alias("?"), Description("Displays this help message.")]
        public bool Help {
            get { return _help; }

            set { _help = value; }
        }

        /// <summary>
        /// Creates a new "this" filled with parsed argument values and returns it.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>This class.</returns>
        /// <remarks>Comments above are taken directly from the Ookii SampleArguments.</remarks>
        public static ConsoleArguments Create(string[] args) {
            // Using a static creation function for a command line arguments class is not required, but it's a convenient
            // way to place all command-line related functionality in one place. To parse the arguments (eg. from the Main method)
            // you then only need to call this function.
            CommandLineParser consoleParser = new CommandLineParser(typeof(ConsoleArguments));
            // The ArgumentParsed event is used by this sample to stop parsing after the -Help argument is specified.
            consoleParser.ArgumentParsed += ArgumentParsed;
            try {
                // The Parse function returns null only when the ArgumentParsed event handler cancelled parsing.
                ConsoleArguments result = (ConsoleArguments)consoleParser.Parse(args);
                if (result != null) {
                    return result;
                }
            } catch (CommandLineArgumentException ex) {
                // We use the LineWrappingTextWriter to neatly wrap console output.
                using (LineWrappingTextWriter writer = LineWrappingTextWriter.ForConsoleError()) {
                    // Tell the user what went wrong.
                    writer.WriteLine(ex.Message);
                    writer.WriteLine();
                }
            }

            // If we got here, we should print usage information to the console.
            // By default, aliases and default values are not included in the usage descriptions; for this sample, I do want to include them.
            WriteUsageOptions options = new WriteUsageOptions();
            options.IncludeDefaultValueInDescription = true;
            options.IncludeAliasInDescription = true;
            // WriteUsageToConsole automatically uses a LineWrappingTextWriter to properly word-wrap the text.
            consoleParser.WriteUsageToConsole(options);

            return null;
        }

        /// <summary>
        /// Handles the "ArgumentsParsed" event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ArgumentParsedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Comments above are taken directly from the Ookii SampleArguments.</remarks>
        private static void ArgumentParsed(object sender, ArgumentParsedEventArgs e) {
            // When the -Help argument (or -? using its alias) is specified, parsing is immediately cancelled. That way, CommandLineParser.Parse will
            // return null, and the Create method will display usage even if the correct number of positional arguments was supplied.
            // Try it: just call the sample with "CommandLineSampleCS.exe foo bar -Help", which will print usage even though both the Source and Destination
            // arguments are supplied.
            if (e.Argument.ArgumentName == "Help") // The name is always Help even if the alias was used to specify the argument
                e.Cancel = true;
        }
    }
}
