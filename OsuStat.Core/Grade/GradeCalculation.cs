namespace OsuStat.Core.Grade;

public static class GradeCalculation
{
    public static Grade CalculateGrade(double accuracy, ushort n300, ushort n100, ushort n50, ushort miss)
    {
        var total = n300 + n100 + n50 + miss;
    
        var percent300 = n300 * 100 / total;
        var percent50 = n50 * 100 / total;
    
        if (accuracy.Equals(100)) return Grade.SS;
        if ( percent300 > 90 && percent50 < 1 && miss == 0 ) return Grade.S;
        if ( (percent300 > 80 && miss == 0) || percent300 > 90 ) return Grade.A;
        if ( (percent300 > 70 && miss == 0) || percent300 > 80 ) return Grade.B;
        if ( percent300 > 60 ) return Grade.C;
        return Grade.D;
    }
}