using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace GenerationRegionLogsAnalyzer.Helpers
{
    internal class ConvertString
    {
        /// <summary>
        /// Parse x,y,z from a string to Vector3
        /// </summary>
        public static Vector3 StringToVector3(string str)
        {

            float[] components = str.Split(',').Select(s => float.Parse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture)).ToArray();


            if (components.Length == 3)
                return new Vector3(components[0], components[1], components[2]);

            throw new ArgumentException("Invalid string format");
        }

        /// <summary>
        /// Convert string with hex value to int
        /// </summary>
        public static int HexStringToInt(string str) => Convert.ToInt32(str, 16);
    }
}
