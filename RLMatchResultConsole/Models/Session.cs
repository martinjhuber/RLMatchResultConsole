using RLMatchResultConsole.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models
{
    internal class Session
    {

        const int MAX_HOURS_BETWEEN_MATCHES = 3;

        public DateTime FirstMatchDateTime { get; set; } = DateTime.MaxValue;
        public DateTime LastMatchDateTime { get; set; } = DateTime.MinValue;

        public List<MatchResult> MatchResults { get; set; } = new List<MatchResult>();


        public void AddMatchResult(MatchResult matchResult)
        {
            MatchResults.Add(matchResult);

            FirstMatchDateTime = matchResult.Date < FirstMatchDateTime ? matchResult.Date : FirstMatchDateTime;
            LastMatchDateTime = matchResult.Date > LastMatchDateTime ? matchResult.Date : LastMatchDateTime;
        }

        public bool IsMatchResultInSession(MatchResult matchResult)
        {
            var startDate = FirstMatchDateTime.AddHours(MAX_HOURS_BETWEEN_MATCHES * -1);
            var endDate = LastMatchDateTime.AddHours(MAX_HOURS_BETWEEN_MATCHES);
            return (matchResult.Date >= startDate && matchResult.Date <= endDate);
        }

        public override string ToString()
        {
            Dictionary<GameMode, int> counts = new Dictionary<GameMode, int>()
            {
                { GameMode.Duel, 0 },
                { GameMode.Doubles, 0 },
                { GameMode.Standard, 0 },
                { GameMode.Chaos, 0 },
                { GameMode.Tournament, 0 },
            };
            int countRanked = 0;
            int wins = 0;
            int losses = 0;
            int gf = 0;
            int ga = 0;

            foreach (MatchResult matchResult in MatchResults)
            {
                counts[matchResult.Match.GameMode] += 1;
                if (matchResult.Match.IsRanked) { countRanked++; }
                if (matchResult.Match.Result == Result.Win) { wins++; } else { losses++; }
                gf += matchResult.Teams[0].TeamScore;
                ga += matchResult.Teams[1].TeamScore;
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<GameMode, int> kvp in counts)
            {
                if (kvp.Value > 0)
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(string.Format("{0,2} {1}", kvp.Value, kvp.Key.ToViewString()));
                }
            }

            return string.Format("{0} | {1,2} W - {2,2} L | {3,3}:{4,3} | {6}", 
                Formatting.FormatDateTimeFull(FirstMatchDateTime),
                wins, losses, gf, ga,
                MatchResults.Count, 
                sb.ToString());
        }
    }
}
