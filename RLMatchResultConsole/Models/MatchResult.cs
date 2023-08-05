using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models
{
    internal class MatchResult
    {
        internal string? FileName { get; set; }

        public DateTime Date { get; set; }
        public Match Match { get; set; } = new Match();

        public List<List<Player>> Players { get; } = new List<List<Player>>();
        public List<Team> Teams { get; } = new List<Team>();

        public override string ToString()
        {
            return String.Format("Date: {0}, ", Date, Match.ToString());
        }

    }
}
