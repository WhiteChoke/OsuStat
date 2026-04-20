using OsuParsers.Enums;
using OsuStat.Core.Enums;

namespace OsuStat.Core.Model;

public class ReplayData
{
    public long OnlineScoreId {get; set;}
    public string Name {get; set;}
    public string Artist {get; set;}
    public string Mapper {get; set;}
    public double Bpm {get; set;}
    public string Length {get; set;}
    public double StarRate {get; set;}
    public double Hp {get; set;}
    public double Cs {get; set;}
    public double Ar {get; set;}
    public string BgPath {get; set;}
    public double PpGained {get; set;}
    public ushort Combo {get; set;}
    public double Accuracy {get; set;}
    public List<Mods> Mods {get; set;}
    public Grade Grade {get; set;}
    
    public override string ToString()
    {
        var modsString = (Mods.Count > 0) 
            ? string.Join(", ", Mods) 
            : "None";

        return $"{Artist} - {Name} [{Mapper}]\n" +
               $"SR: {StarRate:F2}* | BPM: {Bpm} | Length: {Length}\n" +
               $"CS: {Cs} | AR: {Ar} | HP: {Hp}\n" +
               $"Result: {Grade} | {Accuracy:F2}% | {Combo}x | {PpGained:F2}pp\n" +
               $"Mods: {modsString} | ID: {OnlineScoreId}" +
               BgPath;
    }
}