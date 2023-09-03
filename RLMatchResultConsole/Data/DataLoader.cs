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

        private readonly ISettings _settings;
        private readonly IDataCache _dataCache;

        public DataLoader(ISettings settings, IDataCache dataCache) {
            _settings = settings;
            _dataCache = dataCache;
        }

        public enum ProgressType
        {
            MatchesFound,
            MatchesLoaded,
            MatchesAnalysed,
            SessionsGenerated,
            FinishedLoading
        }

        public delegate void ProgressCallback(ProgressType type, int count);

        internal void LoadFullData(ProgressCallback progressCallback)
        {

            _dataCache.Clear();

            string path = _settings.GetParsedMatchResultDirectory();

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("*.json").ToArray();

            progressCallback(ProgressType.MatchesFound, files.Length);

            foreach (FileInfo fileInfo in files)
            {
                LoadFile(fileInfo, progressCallback);
            }

            foreach (MatchResult matchResult in _dataCache.MatchResults.OrderByDescending(mr => mr.Date))
            {
                GenerateOrAddToSession(matchResult, progressCallback);
            }

            progressCallback(ProgressType.FinishedLoading, 0);

        }

        public List<MatchResult> LoadFile(FileInfo fileInfo, ProgressCallback progressCallback)
        {

            string fileContent = File.ReadAllText(fileInfo.FullName);
            string[] jsonStrings;

            List<MatchResult> matchResults = new List<MatchResult>();

            if (fileInfo.Name.EndsWith(".multi.json"))
            {
                jsonStrings = fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                progressCallback(ProgressType.MatchesFound, jsonStrings.Length);
            }
            else
            {
                jsonStrings = new string[] { fileContent };
            }

            foreach (string line in jsonStrings)
            {
                var matchResult = ParseMatchResult(line);
                if (matchResult != null)
                {
                    matchResult.FileName = fileInfo.Name;
                    matchResults.Add(matchResult);
                    _dataCache.AddMatchResult(matchResult);
                }
                progressCallback(ProgressType.MatchesLoaded, 1);

                //Thread.Sleep(1);
            }

            return matchResults;
        }

        public Session GenerateOrAddToSession(MatchResult matchResult, ProgressCallback progressCallback)
        {
            Session? session = _dataCache.Sessions.FirstOrDefault(s => s.IsMatchResultInSession(matchResult));
            if (session is null)
            {
                session = new Session();
                _dataCache.AddSession(session);
                progressCallback(ProgressType.SessionsGenerated, 1);
            }
            session.AddMatchResult(matchResult);
            progressCallback(ProgressType.MatchesAnalysed, 1);

            //Thread.Sleep(1);
            return session;
        }

        internal MatchResult? ParseMatchResult (string content)
        {

            if (Regex.IsMatch(content, "\"version\".*:.*1")) {
                return ParseMatchResultV1(content);
            }

            return null;

        }

        private MatchResult? ParseMatchResultV1 (string content)
        {

            MatchResultV1? matchResultV1 = JsonConvert.DeserializeObject<MatchResultV1>(content);

            MatchResult mr = new MatchResult();
            if (matchResultV1 != null)
            {
                if (matchResultV1.error is not null)
                {
                    return null;
                }

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
                        m.SetGameModeByString(matchResultV1.match.gameMode);
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

                bool mvpIsSet = false;

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
                            if (!mvpIsSet && player.isMvp == 1)
                            {
                                mvpIsSet = true;
                            } 
                        }
                        mr.Players.Add(playerList);
                    }
                }

                // The Rocket League in-game stats that we get via the API do not return that the player
                // of the winning team with the most points is an MVP if this player has less points than
                // any player of the opposing team.
                // In such a case, we get no MVP at all. However, as the game awards the MVP even if 
                // the player does not have the most points overall, we have to fix this discrepancy when
                // we load the match results.
                if (!mvpIsSet && mr.Players.Count == 2)
                {
                    int winnerTeam = mr.Match.Result == Result.Win ? 0 : 1;
                    var mvp = mr.Players[winnerTeam].OrderByDescending(p => p.Score).FirstOrDefault();
                    if (mvp is not null)
                    {
                        mvp.IsMvp = true;
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
