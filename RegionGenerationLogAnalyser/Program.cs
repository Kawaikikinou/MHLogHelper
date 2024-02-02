using GenerationRegionLogsAnalyzer.Enums;
using GenerationRegionLogsAnalyzer.LogModels;
using System;
using System.Reflection;

Type[] logModelTypes = Assembly.GetExecutingAssembly().GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(MarvelHeroesLog)))
        .ToArray();

List<LogGroup> logGroups = new();

string resultFile = Path.Combine(Directory.GetCurrentDirectory(), "AnalyseResult.txt");
using (File.Create(resultFile)) { }

string logsFolder = Path.Combine(Directory.GetCurrentDirectory(), "../../../LogsToCompare");

foreach (var file in Directory.GetFiles(logsFolder))
{
    List<MarvelHeroesLog> logs = new();
    LogGroup logGroup = GenerateLogGroupFromFilename(Path.GetFileNameWithoutExtension(file));

    if (logGroup == null)
        continue;

    foreach (var line in File.ReadAllLines(file))
    {
        foreach (Type logModelType in logModelTypes)
        {
            if (logModelType == null)
                continue;

            if (Activator.CreateInstance(logModelType) is MarvelHeroesLog logInstance && logInstance.TryMatch(line))
            {
                logs.Add(logInstance);
                break;
            }
        }
    }
    logGroup.AddLogs(logs);
    logGroups.Add(logGroup);
}

AnalyseResult();

LogGroup GenerateLogGroupFromFilename(string finename)
{
    // RegionName-S-Seed
    try
    {
        string[] tokens = finename.Split(new char[] { '-' });
        return new LogGroup(tokens[0], ulong.Parse(tokens[2]), tokens[1] == "S" ? LogType.ServerSide : LogType.ClientSide);
    }
    catch
    {
        return null;
    }
}

void AnalyseResult()
{
    LogGroup[] clientLogGroups = logGroups.Where(l => l.LogType == LogType.ClientSide).OrderBy(l => l.RegionName).ToArray();

    foreach (var clientLogGroup in clientLogGroups)
    {
        LogGroup serverLogGroup = logGroups.FirstOrDefault(
            l => l.LogType == LogType.ServerSide
            && l.RegionName == clientLogGroup.RegionName
            && l.Seed == clientLogGroup.Seed);

        if (serverLogGroup == null)
            continue;

        Display("****************************************************************");
        Display($"Analyse of logs from {clientLogGroup.RegionName} Seed={clientLogGroup.Seed}");
        Display("****************************************************************");

        if (clientLogGroup.MarvelHeroesLogs.Count == serverLogGroup.MarvelHeroesLogs.Count)
            Display("Same number of logs", DisplayType.Valid);
        else if (clientLogGroup.MarvelHeroesLogs.Count > serverLogGroup.MarvelHeroesLogs.Count)
            Display("Not enought server logs", DisplayType.Error);
        else Display("Too many server logs", DisplayType.Error);

        for (int i = 0; i < clientLogGroup.MarvelHeroesLogs.Count; i++)
        {
            string clientLog = clientLogGroup.MarvelHeroesLogs[i].ToString();
            string serverLog = i < serverLogGroup.MarvelHeroesLogs.Count ? serverLogGroup.MarvelHeroesLogs[i].ToString() : "No server log";

            Display("Client : " + clientLog);
            Display("Server : " + serverLog);

            if (clientLog.ToLowerInvariant() == serverLog.ToLowerInvariant())
                Display("OK", DisplayType.Valid);
            else
                Display(@"/!\ Not equals /!\", DisplayType.Error);
        }

        for (int i = clientLogGroup.MarvelHeroesLogs.Count; i < serverLogGroup.MarvelHeroesLogs.Count; i++)
        {
            Display("No client log");
            Display(serverLogGroup.MarvelHeroesLogs[i].ToString());
            Display(@"/!\ Not equals /!\", DisplayType.Error);
        }

        Display("");
    }

}

void Display(string log, DisplayType displayType = DisplayType.Information)
{
    switch (displayType)
    {
        case DisplayType.Information:
            Console.ForegroundColor = ConsoleColor.White;
            break;
        case DisplayType.Error:
            Console.ForegroundColor = ConsoleColor.DarkRed;
            break;
        case DisplayType.Valid:
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            break;
    }

    Console.WriteLine(log);
    File.AppendAllLines(resultFile, new List<string> { log });
}

enum DisplayType
{
    Information,
    Error,
    Valid
}