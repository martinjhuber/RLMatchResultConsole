using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models.JsonV1
{
    internal class MatchResultV1
    {

        public DateTime? date { get; set; }
        public MatchV1? match { get; set; } = null;

        public List<List<PlayerV1>>? players { get; set; } = null;
        public List<TeamV1>? teams { get; set; } = null;

    }
}
