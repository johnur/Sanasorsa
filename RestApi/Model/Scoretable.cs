using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Model
{
    public class Scoretable
    {
        CultureInfo provider = CultureInfo.InvariantCulture;
        public Scoretable(string[] inputScores)
        {
            ScoreTable = Array.ConvertAll(inputScores, x => Int32.Parse(x, provider));
        }
        public int[] ScoreTable { get; set; }
        
    }
}
