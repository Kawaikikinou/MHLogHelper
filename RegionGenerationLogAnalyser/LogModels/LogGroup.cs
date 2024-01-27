using GenerationRegionLogsAnalyzer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerationRegionLogsAnalyzer.LogModels
{
    public class LogGroup
    {
        public string RegionName { get; private set; }
        public ulong Seed { get; private set; }
        public LogType LogType { get; private set; }
        public List<MarvelHeroesLog> MarvelHeroesLogs { get; private set; }

        public LogGroup(string regionName, ulong seed, LogType logType)
        {
            RegionName = regionName;
            Seed = seed;
            LogType = logType;
            MarvelHeroesLogs = new List<MarvelHeroesLog>();
        }

        public void AddLogs(List<MarvelHeroesLog> marvelHeroesLogs)
        {
            if (marvelHeroesLogs == null)
                return;

            MarvelHeroesLogs.AddRange(marvelHeroesLogs);
        }
    }
}
