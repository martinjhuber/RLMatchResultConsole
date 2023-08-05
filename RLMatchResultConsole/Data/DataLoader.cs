using Newtonsoft.Json;
using RLMatchResultConsole.Common;
using RLMatchResultConsole.Models;
using RLMatchResultConsole.Models.JsonV1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RLMatchResultConsole.Data
{
    internal class DataLoader 
    {

        const string DEFAULT_MATCHRESULTS_DIR = ".\\MatchResults";

        private readonly ISettings _settings;
        private readonly IDataCache _dataCache;


        public DataLoader(ISettings settings, IDataCache dataCache) {
            _settings = settings;
            _dataCache = dataCache;
        }

        public delegate bool ProgressCallback(float fraction, int loaded, int total, int sessions);
        public delegate void FinishedCallback();

        internal void LoadData(ProgressCallback progressCallback, FinishedCallback finishedCallback)
        {

            string path = _settings.MatchResultDirectory ?? DEFAULT_MATCHRESULTS_DIR;

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("*.json").ToArray();
            int total = files.Length;
            int loaded = 0;

            foreach (FileInfo fileInfo in files)
            {
                string fileContent = File.ReadAllText(fileInfo.FullName);
                string[] jsonStrings;

                MatchResult ? matchResult = null;

                if (fileInfo.Name.EndsWith(".multi.json"))
                {
                    jsonStrings = fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    total += jsonStrings.Length - 1;
                }
                else
                {
                    jsonStrings = new string[] { fileContent };
                }

                foreach (string line in jsonStrings)
                {
                    matchResult = ParseMatchResult(line);
                    if (matchResult != null)
                    {
                        matchResult.FileName = fileInfo.Name;
                        _dataCache.AddMatchResult(matchResult);
                    }
                    loaded++;
                    progressCallback((float)loaded / (float)total * 0.8F, loaded, total, 0);

                    //Thread.Sleep(1);
                }

            }

            loaded = 0;
            foreach (MatchResult matchResult in _dataCache.MatchResults.OrderByDescending(mr => mr.Date))
            {
                Session? session = _dataCache.Sessions.FirstOrDefault(s => s.IsMatchResultInSession(matchResult));
                if (session is null)
                {
                    session = new Session();
                    _dataCache.AddSession(session);
                }
                session.AddMatchResult(matchResult);

                ++loaded;
                progressCallback(0.8F + (float)loaded / (float)total * 0.2F, total, total, _dataCache.Sessions.Count);   // creating sessions is 20% of work

            }

            finishedCallback();

        }

        internal MatchResult? ParseMatchResult (string content)
        {

            if (Regex.IsMatch(content, "\"version\".*:.*1")) {
                return ParseMatchResultV1(content);
            }

            return null;

        }

        private MatchResult ParseMatchResultV1 (string content)
        {

            MatchResultV1? matchResultV1 = JsonConvert.DeserializeObject<MatchResultV1>(content);

            MatchResult mr = new MatchResult();
            if (matchResultV1 != null)
            {
                mr.Date = matchResultV1.date ?? DateTime.UtcNow;

                if (matchResultV1.match != null)
                {
                    Models.Match m = new Models.Match()
                    {
                        GameTime = matchResultV1.match.gameTime,
                        IsClubMatch = matchResultV1.match.isClubMatch == 1,
                        IsForfeit = matchResultV1.match.isForfeit == 1,
                        IsOvertime = matchResultV1.match.isOvertime == 1,
                        IsRanked = matchResultV1.match.isRanked == 1,
                        IsTournament = matchResultV1.match.isTournament,
                        OvertimeTimePlayed = matchResultV1.match.overtimeTimePlayed,
                    };

                    if (matchResultV1.match.gameMode != null)
                    {
                        switch (matchResultV1.match.gameMode)
                        {
                            case "Duel":
                                m.GameMode = GameMode.Duel; break;
                            case "Doubles":
                                m.GameMode = GameMode.Doubles; break;
                            case "Standard":
                                m.GameMode = GameMode.Standard; break;
                            case "Chaos":
                                m.GameMode = GameMode.Chaos; break;
                            case "Tournament Match":
                                m.GameMode = GameMode.Tournament; break;
                            default:
                                m.GameMode = GameMode.Undef; break;
                        }
                    }

                    if (matchResultV1.match.result != null)
                    {
                        switch (matchResultV1.match.result)
                        {
                            case "WIN":
                                m.Result = Result.Win; break;
                            default:
                                m.Result = Result.Loss; break;
                        }
                    }

                    mr.Match = m;
                }

                if (matchResultV1.players != null && matchResultV1.players.Count == 2)
                {
                    for (int i = 0; i <= 1; i++)
                    {
                        var players = matchResultV1.players[i];
                        List<Player> playerList = new List<Player>();
                        foreach (var player in players)
                        {
                            Player p = new Player()
                            {
                                Name = player.name ?? "n/a",
                                PlayerId = player.playerId,
                                Score = player.score,
                                Goals = player.goals,
                                Assists = player.assists,
                                Saves = player.saves,
                                Shots = player.shots,
                                TeamNumber = player.teamNum,
                                IsMvp = player.isMvp == 1,
                            };
                            playerList.Add(p);
                        }
                        mr.Players.Add(playerList);
                    }
                }

                if (matchResultV1.teams != null && matchResultV1.teams.Count == 2)
                {
                    for (int i = 0; i <= 1; i++)
                    {
                        var team = matchResultV1.teams[i];

                        Team t = new Team()
                        {
                            TeamName = team.teamName,
                            TeamNumber = team.teamNum,
                            TeamScore = team.score,
                            HasForfeit = team.hasForfeit == 1,
                        };
                        mr.Teams.Add(t);
                    }
                }

            }

            return mr;

        }

    }
}
