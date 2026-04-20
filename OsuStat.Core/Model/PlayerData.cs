namespace OsuStat.Core.Model;

public class PlayerData
{
    public string Nickname {get; set; }
    public int Id {get; set; }
    public int Ranking {get; set; }
    public DateTimeOffset LastSeenAt {get; set; }
    
    public override string ToString()
    {
        var lastSeen = LastSeenAt.LocalDateTime.ToString("dd.MM.yyyy HH:mm");
    
        return $"#{Ranking} {Nickname} (ID: {Id}) | Last seen: {lastSeen}";
    }
}