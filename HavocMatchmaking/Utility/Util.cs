namespace HavocMatchmaking.Utility;

public static class Util
{
    public static byte ConvertRank(int rank)
    {
        return rank switch
        {
            >= 0 and < 150 => 0,
            >= 150 and < 300 => 0, //should be one doing 0 for playtest
            >= 300 and < 450 => 2,
            >= 450 and < 600 => 3,
            >= 600 and < 750 => 4,
            >= 750 and < 900 => 5,
            _ => 0
        };
    }
}