public static class AdvanceDecider
{
    public enum WhoIsWinning
    {
        Host,
        Guest,
        Draw
    }
    // true if host of the second match won, false - if guest, null draw
    public static WhoIsWinning NormalTwoLeg(MatchResult m1, MatchResult m2)
    {
        if (m1.HostGoals + m2.GuestGoals > m1.GuestGoals + m2.HostGoals) 
            return WhoIsWinning.Guest;
        else if(m1.HostGoals + m2.GuestGoals < m1.GuestGoals + m2.HostGoals) 
            return WhoIsWinning.Host;

        int aTotal = m1.HostGoals + 2 * m2.GuestGoals;
        int bTotal = m1.GuestGoals * 2 + m2.HostGoals;

        if (aTotal > bTotal) 
            return WhoIsWinning.Guest;
        else if (aTotal < bTotal) 
            return WhoIsWinning.Host;
        else 
            return WhoIsWinning.Draw;
    }
    public static WhoIsWinning NormalMatch(MatchResult matchResult)
    {
        if (matchResult.HostGoals > matchResult.GuestGoals) 
            return WhoIsWinning.Host;
        else if (matchResult.HostGoals < matchResult.GuestGoals) 
            return WhoIsWinning.Guest;
        else 
            return WhoIsWinning.Draw;
    }
}
