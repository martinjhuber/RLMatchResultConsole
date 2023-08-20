namespace RLMatchResultConsole.Common
{
    public interface ISettings
    {
        string MatchResultDirectory { get; set; }
        Filters Filters { get; set; }

        string GetParsedMatchResultDirectory();
    }
}