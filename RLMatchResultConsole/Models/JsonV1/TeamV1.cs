using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models.JsonV1
{
    internal class TeamV1
    {

        public int teamNum { get; set; }
        public string? teamName { get; set; }
        public int score { get; set; }
        public int hasForfeit { get; set; }

    }
}
