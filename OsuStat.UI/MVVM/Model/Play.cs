using OsuStat.Core.Enums;

namespace OsuStat.UI.MVVM.Model;

public class Play
{
    public double PpGained { get; set; }
    public ushort MaxCombo  { get; set; }
    public double Accuracy { get; set; }
    public List<string> Mods { get; set; } = [];
    public Grade Grade { get; set; }
}