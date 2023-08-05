namespace RLMatchResultConsole.Common
{
    public interface ISettings
    {
        string? MatchResultDirectory { get; set; }
        DefaultFilters DefaultFilters { get; set; }
    }
}