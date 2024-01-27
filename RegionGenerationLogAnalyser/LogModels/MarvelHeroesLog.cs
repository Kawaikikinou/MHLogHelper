using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenerationRegionLogsAnalyzer.LogModels
{
    public abstract class MarvelHeroesLog
    {
        protected abstract string _logPattern { get; }
        public bool TryMatch(string log)
        {
            Regex regex = new(_logPattern);
            Match match = regex.Match(log);
            if (!match.Success)
                return false;

            ExtractData(match);
            return true;
        }

        protected abstract void ExtractData(Match match);
    }
}
