using GenerationRegionLogsAnalyzer.Enums;
using GenerationRegionLogsAnalyzer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenerationRegionLogsAnalyzer.LogModels
{
    public class CellSetRegistry : MarvelHeroesLog
    {
        protected override string _logPattern => @".*CellSetRegistry::LoadCellSet\((.+)\)";

        public string CellName { get; set; }

        public override string ToString()
        {
            return $"CellSetRegistry::LoadCellSet({CellName})";
        }

        protected override void ExtractData(Match match)
        {
            CellName = match.Groups[1].Value;
        }
    }
}
