using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models.JsonV1
{

    internal class MatchV1
    {
        public string? gameMode { get; set; }

        public int gameTime { get; set; }
        public int isClubMatch { get; set; }
        public int isForfeit { get; set; }
        public int isOvertime { get; set; }
        public int isRanked { get; set; }
        public bool isTournament { get; set; }

        public float overtimeTimePlayed { get; set; }

        public string? result { get; set; }

        public override string ToString() {
            return String.Format("Mode: {0}, Result: {1}", gameMode, result);
        }

    }
}
