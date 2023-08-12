using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Models
{

    internal enum GameMode
    {
        Undef,
        Duel,
        Doubles,
        Standard,
        Chaos,
        Tournament,
        Rumble,
        Dropshot,
        Hoops,
        SnowDay,
    }

    internal static class MatchEnumExtensions
    {
        public static string ToViewString(this GameMode gm)
        {
            switch (gm)
            {
                case GameMode.Undef:
                    return "Other";
                case GameMode.SnowDay:
                    return "Snow Day";
                default:
                    return gm.ToString();
            }
        }

        public static string ToViewString(this Result gm)
        {
            switch (gm)
            {
                case Result.Win:
                    return "WIN";
                default:
                    return "LOSS";
            }
        }
    }

    internal enum Result
    {
        Win,
        Loss
    }

    internal class Match
    {
        public GameMode GameMode { get; set; }

        public void SetGameModeByString(string? modeName)
        {
            switch (modeName)
            {
                case "Duel":
                    GameMode = GameMode.Duel;
                    break;
                case "Doubles":
                    GameMode = GameMode.Doubles;
                    break;
                case "Standard":
                    GameMode = GameMode.Standard;
                    break;
                case "Chaos":
                    GameMode = GameMode.Chaos;
                    break;
                case "Tournament Match":
                    GameMode = GameMode.Tournament;
                    break;
                case "Rumble":
                    GameMode = GameMode.Rumble;
                    break;
                case "Dropshot":
                    GameMode = GameMode.Dropshot;
                    break;
                case "Hoops":
                    GameMode = GameMode.Hoops;
                    break;
                case "Snow Day":
                    GameMode = GameMode.SnowDay;
                    break;
                default:
                    GameMode = GameMode.Undef;
                    break;
            }
        }

        public int GameTime { get; set; }
        public bool IsClubMatch { get; set; }
        public bool IsForfeit { get; set; }
        public bool IsOvertime { get; set; }
        public bool IsRanked { get; set; }
        public bool IsTournament { get; set; }

        public float OvertimeTimePlayed { get; set; }

        public Result Result { get; set; }

        public void SetResultByString(string? resultName)
        {
            switch (resultName)
            {
                case "WIN":
                    Result = Result.Win;
                    break;
                case "LOSS":
                    Result = Result.Loss;
                    break;
                default:
                    Result = Result.Loss;
                    break;
            }
        }

        public override string ToString() {
            return String.Format("Mode: {0}, Result: {1}", GameMode, Result);
        }

    }
}
