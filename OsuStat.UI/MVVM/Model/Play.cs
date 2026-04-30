using OsuStat.Core.Enums;

namespace OsuStat.UI.MVVM.Model;

public class Play
{
    public double PpGained { get; set; }
    public ushort MaxCombo { get; set; }
    public double Accuracy { get; set; }
    public List<string> Mods { get; set; } = [];
    public Grade Grade { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Play other) return false;
        if (ReferenceEquals(this, other)) return true;

        return PpGained.Equals(other.PpGained) &&
               MaxCombo == other.MaxCombo &&
               Accuracy.Equals(other.Accuracy) &&
               Mods.SequenceEqual(other.Mods) &&
               Grade.Equals(other.Grade);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PpGained, MaxCombo, Accuracy, Mods, (int)Grade);
    }
}