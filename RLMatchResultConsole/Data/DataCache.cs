using RLMatchResultConsole.Common;
using RLMatchResultConsole.Models;
using RLMatchResultConsole.Models.JsonV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Data
{
    internal class DataCache : IDataCache
    {

        private readonly DataFilter _filter;

        public IList<MatchResult> MatchResults { get; } = new List<MatchResult>();

        public IList<Session> Sessions { get; } = new List<Session>();

        public DataCache(DataFilter filter)
        {
            _filter = filter;
        }

        public void AddMatchResult(MatchResult matchResult)
        {
            MatchResults.Add(matchResult);
        }

        public void AddSession(Session session)
        {
            Sessions.Add(session);
        }

        public IEnumerable<Session> GetFilteredSessions()
        {
            return Sessions.Where(s => s.MatchResults.Any(mr => _filter.GameModeFilter(mr.Match)));
        }

        public void Clear() {
            MatchResults.Clear();
            Sessions.Clear(); 
        }

    }
}
