<h2>Log Aggregator</h2>
<p>A tool for combining disparate log files into a common format useful for troubleshooting.</p>
<h3>History</h3>
<p>
    A previous employer had a large number of small utilities that ran at least once a day up to tens of times a day. All
    of these programs produced some sort of log file. When problems arose, it became quite an exercise sifting through
    all of the log files, windows event files, and other data trying to determine the cause of the problem and which
    program was broken. I had high motivation for organizing this mess since I was usually the one responsible for
    finding an answer when a failure occurred. Out of this need, the idea of aggregating all of these log files to
    query them around specific times of day and server names was born. Of course, it was "easier said than done."
</p>
<p>
    Each log file had it's own format and layout, for example:
</p>
<ul>
    <li>A log could have only a month and day for timestamps.</li>
    <li>Log files might be circular and overwrite content during the day, or</li>
    <li>A single log entry might be spread over several lines</li>
</ul>
<p>
    It was easy to see that a different parser was needed for each log!
</p>
<p>
    Out of this chaos the LogAggregator program design evolved to incorporate just that idea: assignable parsers for
    each log file. In short, the program allows for "plug-ins" to handle the various log formats to produce a
    standardized log structure that I could more easily filter, search, and analyze results related to solving the
    "problem du jour."
</p>
<p>
    The program originally used SQL server tables to provide things like Log file paths, DLL paths, and class names
    to control reading of log files and loading of the necessary code to parse that file. The output also went back
    to SQL server as a "parsed" object that broke each log entry down into a row containing an index (e.g. log "line
    number"), a timestamp, and a message (the "text" of the log entry).
</p>
<p>
    Finally, during development it became clear that a notification mechanism, based on log content, was a
    desirable feature. This was really how the whole "plug-in" thing started. How so? Some log files
    contain entries for errors that don't necessarily flow back to the UI, or Console.
    The errors may be important but not severe enough to halt processing. With this scenario the first
    plug-in was conceived: an email was sent whenever certain error text was found in a log entry;
    in other words, look for "magic words" in the log file.
</p>

<h3>This Version</h3>
<p>
    LogAggregator has already gone through a few iterations. This version is another rewrite with an objective
    to simplify. I'm also using this opportunity to remove some of the "heavier" dependencies I had like
    relying on SQL server to drive inputs and outputs and using Entity Framework for database access.
    Also, I used this opportunity to simplify the design to where most inputs and outputs in the program are
    handled by developer selected plug-ins. For example, writing output to the screen, or saving standardized
    log entries to a database, are handled by plug-ins.
</p>
<p>
    SQL options will reappear later as I continue the rewrite; they will materialize as various
    storage options like standard SQL server storage, in memory databases, NoSQL databases,
    or as combinations of these options. For now, the only storage option, JSON, is used to supply the parser
    and listener DLL paths and class names; other vital information like log file location and name
    also go along for the ride.
</p>

<h3>Using Gator</h3>
<p>
    Yes, another program named "Gator"...I'm lazy...It's just easier to say "Gator" than stringing together
    those "Log" and "Aggregator" words all the time...anyway:<br />
    The project is divided into two solutions:
</p>

1. The main solution that "drives" the parsing code, and
2. The sample projects providing the plug-in assemblies used in the main solution

<p>
    You will also find a directory named build-working-sample that builds both solutions, creates a sample json
    control file, and copies all of the files (including a sample log file) into the product subdirectory. The
    sample is nothing special, it just serves as a way to prove that you have everything and that it works
    on your installation.
</p>
<p>
    How do you use the aggregator? Simple; use C# to create two classes:
</p>

1. a parser class with a parse method that creates one LogEntry object for each logical "log line."
2. a listener, or subscriber, class that responds to a line parsed event produced by the parse method
    mentioned above...this class can then write to screen, save stuff to a database, send emails,
    or whatever else you might dream up.

<p>
    Note that a listener is not actually required. The parse method signature shows that it returns a
    list of LogEntry objects obtained from parsing the entire log file. You can return an empty list and
    just respond to the LineParsed events, just return the list of parsed LogEntry objects, or do
    both; it's up to you.
</p>

<h3>Examples</h3>
<p>
    Let's start with some sample log data:
</p>
<pre>
2/5/2000 01:45 Testing Database Connection
2/5/2000 01:45 Attempting to Log Activity in Application Log Database
2/5/2000 01:45 Retrieving Last Pending Item Activity
2/5/2000 01:45 Last Trade Date is 20100204
2/5/2000 01:45 Retrieving Accounting System User Id & Password
2/5/2000 01:45 Establishing Accounting System Connectivity
</pre>
<p>
    ...this is the sample used to drive this maiden voyage of Gator.
</p>
<p>
    This is a pretty simple example; a log file with a date and time followed by a (hopefully) meaningful message.
    To parse, place the first two items in a group as a date/time data item. The remainder is the message part
    of the log entry. The goal is to transform this into a LogEntry object (see the LogEntry class in the
    Entity project and shown immediately below). The sample code handling this job in the TestParser class
    located in the Parser project ("Extensions" solution).
</p>
<pre>
    public class LogEntry {
        public int Id { get; set; }

        public string Extensions { get; set; }

        public int Index { get; set; }

        public string Message { get; set; }

        public LogFile Source { get; set; }

        public System.DateTime Timestamp { get; set; }
    }

</pre>
<p>
    Stare at the above class definition long enough and you will notice a field named "Extensions." I use this to
    capture any other parts of interest from the log file. My thought is to put searchable content here as
    JSON data but your imagination might find better possible uses.
</p>

<h4>The Sample Parser</h4>
<p>
    The TestParser class uses a regular expression on each line to create the two items mentioned above.
    The DataEntry object also contains a reference to the LogFile class containing information about the
    log providing the parsing data.
</p>
<p>
    This snippet from the TestParser class shows how the relevant parts of each line of the log are pulled out
    and placed into a new LogEntry object.
</p>
<pre>
for (int i = startingIndex; i < logLines.Count; i++) {
    MatchCollection matches = Regex.Matches(logLines[i], @"^(.+?\ .+?)\ (.+?)$");

    int j = i;
    foreach (
       var logEntry in
       from Match match in matches
       select match.Groups into groups
       where
          (groups.Count > 0)
       select new LogEntry {
           Index = j,
           Message = groups[2].Value,
           Source = Log,
           Timestamp = DateTime.Parse(groups[1].Value, CultureInfo.InvariantCulture)
       }
    ) {
        list.Add(logEntry);
        this.OnLineParsed(this, new LogEntryItemEventArgs(logEntry, logLines[i]));
    }
}
</pre>

<h4>The Sample Listener</h4>
<p>
    The output, or listener, piece of this action just dumps each line of all the LogEntries created above
    as a tab delimited dump to the screen.
</p>
<p>
    Note the last line of the foreach loop from the Parse method above:
</p>
<pre>
this.OnLineParsed(this, new LogEntryItemEventArgs(logEntry, logLines[i]));
</pre>
<p>
    This line raises the LineParsed event and notifies any listeners that a new LogEntry object was created. In the
    example, the only listener is ConsoleOutput. It does only one thing: print the LogEntry to the screen;
    specifically, it writes the log ID, timestamp, and message to the console. This is seen in the OnLineParsed
    event handler of the ConsoleOutput class:
</p>
<pre>
Console.WriteLine("Log ID: {0}\tTimestamp: {1}\tMessage: {2}", e.Entry.Source.Location, e.Entry.Timestamp, e.Entry.Message);
</pre>

<h4>The JSON data file</h4>
<p>
    GatorData.json is used to provide the information needed to create the parser object, any listeners, and finally
    create the minimum required for a useful LogFile entity object. Let's take a look at the sample included with
    the sample code:
</p>
<pre>
[
    {
        'Id': 1,
        'Location': 'c:\\users\\bruce\\skydrive\\projects\\logaggregator\\sample files\\Test.log',
        'Parser': {
            'AssemblyName': 'C:\\Users\\Bruce\\SkyDrive\\Projects\\LogAggregator\\Extensions\\build\\product\\Parser.dll',
            'ClassName': 'Gator.Extensions.Parser.TestParser'
        },
        'Listeners': [
            {
            'AssemblyName': 'C:\\Users\\Bruce\\SkyDrive\\Projects\\LogAggregator\\Extensions\\build\\product\\Listener.dll',
            'ClassName': 'Gator.Extensions.Listener.ConsoleOutput'
            }
        ]
    }
]
</pre>
<p>
    The file contains the minimum information we need to create a Parser object, add any Listener objects, and the
    whereabouts of the actual log. The ID field is there for future database purposes use it as you see fit.
</p>

<h3>Licenses and Supporting Cast</h3>
<h4>Dependencies</h4>
<p>
    Build scripts: psake<br />
    Unit and Integration Testing: nUnit and Moq
</p>

<h4>Licenses</h4>
<p>
    MIT: LogAggregator (this program) and psake<br />
    nUnit: NUnit License<br />
    Moq: BSD License
</p>
<p>
    All license terms are contained in the Licensing file included with this software.
</p>
<p>
You will probably see references to ReSharper scattered throughout the code...don't be alarmed; I don't like
to worry about mundane details if I can get away with it. It's easier to let ReSharper handle these chores
while I focus on the bigger picture.
</p>
<p>
Also, another "like or leave it": the existence of any GlobalSuppression.cs files show that I also use
StyleCcp from Microsoft.
</p>

<h3>Final Words</h3>
<p>
    This is my first project on GitHub so I'm definitely finding my way around. If I missed something, or put
    things in the wrong place, please let me know; at the same time, please point me to the information needed to
    correct it.
</p>
<p>
    There are certainly more ways to handle this problem. In fact, I'm evaluating many other ideas right now.
    On the other hand, the project will grow, in its current form, on several fronts; some of them include
    more log parsing examples, SQL Server data, No-SQL data, in memory data, and more. Other ideas need further
    refinement before rolling them in. Stay tuned...
</p>
