using GenerationRegionLogsAnalyzer.Enums;
using GenerationRegionLogsAnalyzer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenerationRegionLogsAnalyzer.LogModels
{
    public class AddingCell : MarvelHeroesLog
    {
        protected override string _logPattern =>
            @".*Adding cell (.+), cellid=(\d+), cellpos=\((.+)\), game=(.+) in region (.+), .*, DIFF=(.*), SEED=(\d+), GAMEID=(.+)";

        public string CellName { get; private set; }
        public int CellId { get; private set; }
        public Vector3 CellPos { get; private set; }
        public string GameId { get; private set; }
        public string RegionName { get; private set; }
        public Difficulty Difficulty { get; private set; }
        public ulong Seed { get; private set; }

        public override string ToString()
        {
            return $"Adding cell {CellName}, cellpos=({CellPos.X:0.00}, {CellPos.Y:0.00}, {CellPos.Z:0.00}), in region {RegionName}, DIFF={Difficulty}, SEED={Seed}";
            //return $"CellId:{CellId} => Adding cell {CellName}, cellpos=({CellPos.X:0.00}, {CellPos.Y:0.00}, {CellPos.Z:0.00}), in region {RegionName}, DIFF={Difficulty}, SEED={Seed}";
        }

        protected override void ExtractData(Match match)
        {
            CellName = match.Groups[1].Value;
            CellId = int.Parse(match.Groups[2].Value);
            CellPos = ConvertString.StringToVector3(match.Groups[3].Value);
            GameId = match.Groups[4].Value;
            RegionName = match.Groups[5].Value;
            Difficulty = Enum.Parse<Difficulty>(match.Groups[6].Value);
            Seed = ulong.Parse(match.Groups[7].Value);
        }
    }
}
