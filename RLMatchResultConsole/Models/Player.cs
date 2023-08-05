using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models
{
    internal class Player
    {
        public string? Name { get; set; }
        public int PlayerId { get; set; }
        public int Score { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Saves { get; set; }
        public int Shots { get; set; }
        public int TeamNumber { get; set; }
        public bool IsMvp { get; set; }

    }
}
