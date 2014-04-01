##Log Aggregator##

A tool for combining log files into a common format useful for troubleshooting.

###Using tree-collector###

The project is divided into two solutions:

1. The main solution that "drives" the parsing code, and
2. The sample projects providing the plug-in assemblies used in the main solution

You will also find a directory named build-working-sample that builds both solutions, creates a sample json control file, and copies all of the files (including a sample log file) into the product subdirectory. The sample is nothing special, it just serves as a way to prove that you have everything and that it works on your installation.

How do you use the aggregator? Simple; use C# to create two classes:
    
1. a parser class with a parse method that creates one LogEntry object for each logical "log line."
2. a listener, or subscriber, class that responds to a line parsed event produced by the parse method mentioned above...this class can then write to screen, save stuff to a database, send emails, or whatever else you might dream up.

Note that a listener is not actually required. The parse method signature shows that it returns a list of LogEntry objects obtained from parsing the entire log file. You can return an empty list and just respond to the LineParsed events, just return the list of parsed LogEntry objects, or do both; it's up to you.

###Examples###

Let's start with some sample log data:

```
2/5/2000 01:45 Testing Database Connection
2/5/2000 01:45 Attempting to Log Activity in Application Log Database
2/5/2000 01:45 Retrieving Last Pending Item Activity
2/5/2000 01:45 Last Trade Date is 20100204
2/5/2000 01:45 Retrieving Accounting System User Id & Password
2/5/2000 01:45 Establishing Accounting System Connectivity
```

...this is the sample used to drive this maiden voyage of Gator.

This is a pretty simple example; a log file with a date and time followed by a (hopefully) meaningful message. To parse, place the first two items in a group as a date/time data item. The remainder is the message part of the log entry. The goal is to transform this into a LogEntry object (see the LogEntry class in the Entity project and shown immediately below). The sample code handling this job in the TestParser class located in the Parser project ("Extensions" solution).

```C#
public class LogEntry {
    public int Id { get; set; }

    public string Extensions { get; set; }

    public int Index { get; set; }

    public string Message { get; set; }

    public LogFile Source { get; set; }

    public System.DateTime Timestamp { get; set; }
}
```

Stare at the above class definition long enough and you will notice a field named "Extensions." I use this to capture any other parts of interest from the log file. My thought is to put searchable content here as JSON data but your imagination might find better possible uses.

####The Sample Parser####

The TestParser class uses a regular expression on each line to create the two items mentioned above. The DataEntry object also contains a reference to the LogFile class containing information about the log providing the parsing data.

This snippet from the TestParser class shows how the relevant parts of each line of the log are pulled out and placed into a new LogEntry object.

```C#
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
```

####The Sample Listener####

The output, or listener, piece of this action just dumps each line of all the LogEntries created above as a tab delimited dump to the screen.

Note the last line of the foreach loop from the Parse method above:

```C#
this.OnLineParsed(this, new LogEntryItemEventArgs(logEntry, logLines[i]));
```

This line raises the LineParsed event and notifies any listeners that a new LogEntry object was created. In the example, the only listener is ConsoleOutput. It does only one thing: print the LogEntry to the screen; specifically, it writes the log ID, timestamp, and message to the console. This is seen in the OnLineParsed event handler of the ConsoleOutput class:

```C#
Console.WriteLine("Log ID: {0}\tTimestamp: {1}\tMessage: {2}", e.Entry.Source.Location, e.Entry.Timestamp, e.Entry.Message);
```

####The JSON data file####

GatorData.json is used to provide the information needed to create the parser object, any listeners, and finally create the minimum required for a useful LogFile entity object. Let's take a look at the sample included with the sample code:

```JSON
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
```

The file contains the minimum information we need to create a Parser object, add any Listener objects, and the whereabouts of the actual log. The ID field is there for future database purposes; use it as you see fit.

###Licenses and Supporting Cast###
####Dependencies####

Build scripts: psake
Unit and Integration Testing: nUnit and Moq

####Licenses####

MIT: LogAggregator (this program) and psake
nUnit: NUnit License
Moq: BSD License

All license terms are contained in the Licensing file included with this software.

You will probably see references to ReSharper scattered throughout the code...don't be alarmed; I don't like to worry about mundane details if I can get away with it. It's easier to let ReSharper handle these chores while I focus on the bigger picture.

Also, another "like it or leave it": the existence of any GlobalSuppression.cs files show that I also use StyleCcp from Microsoft.

###Final Words###

This is my first project on GitHub so I'm definitely finding my way around. If I missed something, or put things in the wrong place, please let me know; and, if possible, please point me to the information needed to avoid it in the future.

Also, further explanations and project notes are available via blog entries at http://www.oblander.net/orchard/dog-lab .