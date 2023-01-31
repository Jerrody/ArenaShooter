namespace Game.Global
{
    internal enum MyEnum: uint
    {
        NoScope,
    }
    
    public static class PlayerDataController
    {
        // Stats
        private const string Kills = "kills";
        private const string Wins = "wins";
        private const string Loses = "loses";
        private const string ScopeAttachments = "attachments";
    }
}