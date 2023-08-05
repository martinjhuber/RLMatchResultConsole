using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models
{
    internal class Team
    {

        public int TeamNumber { get; set; }
        public string? TeamName { get; set; }
        public int TeamScore { get; set; }
        public bool HasForfeit { get; set; }

    }
}
