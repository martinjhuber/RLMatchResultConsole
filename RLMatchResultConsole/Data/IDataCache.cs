using RLMatchResultConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Data
{
    internal interface IDataCache
    {

        IList<MatchResult> MatchResults { get; }

        IList<Session> Sessions { get; }

        void AddMatchResult(MatchResult matchResult);
        void AddSession(Session session);

        IEnumerable<Session> GetFilteredSessions();
    }
}
