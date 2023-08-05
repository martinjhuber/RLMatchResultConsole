using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models.JsonV1
{
    internal class PlayerV1
    {
        public string? name { get; set; }
        public int playerId { get; set; }
        public int score { get; set; }
        public int goals { get; set; }
        public int assists { get; set; }
        public int saves { get; set; }
        public int shots { get; set; }
        public int teamNum { get; set; }
        public int isMvp { get; set; }

    }
}
