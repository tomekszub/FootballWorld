using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class League_Old 
{
    public enum EuropaTournamentType
    {
        ChampionsCup,
        EuropaCup,
        EuropaTrophy
    }

	public string Name, Country;
	public List<Club> Teams = new List<Club>();
    public float RankingPoints;
    public int CupWInnerID;

    public League_Old(string name, string country, List<Club> teams, float rankingPoints, int cupWInnerID = -1)
	{
		Name = name;
		Country = country;
		Teams = teams;
		RankingPoints = rankingPoints;
        if (cupWInnerID == -1) 
            CupWInnerID = teams[0].Id;
        else 
            CupWInnerID = cupWInnerID;
    }

    public Club GetCupWinnerIndex(int rankingPos)
    {
        if (rankingPos > 15)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Teams[i].Id == CupWInnerID)
                {
                    return Teams[3];
                }
            }
            return Database.clubDB[CupWInnerID];
        }
        else if (rankingPos >= 7 && rankingPos <= 15)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Teams[i].Id == CupWInnerID)
                {
                    return Teams[4];
                }
            }
            return Database.clubDB[CupWInnerID];
        }
        else if (rankingPos == 5 || rankingPos == 6)
        {
            for (int i = 0; i < 5; i++)
            {
                if (Teams[i].Id == CupWInnerID)
                {
                    return Teams[5];
                }
            }
            return Database.clubDB[CupWInnerID];
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                if(Teams[i].Id == CupWInnerID)
                {
                    return Teams[6];
                }
            }
            return Database.clubDB[CupWInnerID];
        }
    }

    public List<Club> GetClubsForEuropaTournament(int rankingPosition, EuropaTournamentType tournamentType, int roundIndex)
    {
        rankingPosition++;
        if(tournamentType == EuropaTournamentType.ChampionsCup)
        {
            switch (roundIndex)
            { 
                case 0:
                    return null;
                case 1:
                    switch(rankingPosition)
                    {
                        case >= 35:
                            return new List<Club>() { Teams[0] };
                        default: 
                            return null;
                    }
                case 2:
                    switch (rankingPosition)
                    {
                        case >= 17 and <= 34:
                            return new List<Club>() { Teams[0] };
                        default:
                            return null;
                    }
                case 3:
                    switch (rankingPosition)
                    {
                        case >= 11 and <= 16:
                            return new List<Club>() { Teams[0] };
                        default:
                            return null;
                    }
                case 4:
                    switch (rankingPosition)
                    {
                        case >= 5 and <= 6:
                            return new List<Club>() { Teams[2] };
                        case >= 7 and <= 10:
                            return new List<Club>() { Teams[1] };
                        default:
                            return null;
                    }
                case 5:  // group phase
                    switch (rankingPosition)
                    {
                        case >= 1 and <= 4:
                            return new List<Club>() { Teams[0], Teams[1], Teams[2], Teams[3] };
                        case >= 5 and <= 6:
                            return new List<Club>() { Teams[0], Teams[1] };
                        case >= 7 and <= 10:
                            return new List<Club>() { Teams[0] };
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }
        else if (tournamentType == EuropaTournamentType.EuropaCup)
        {
            switch (roundIndex)
            {
                case 0:
                    switch (rankingPosition)
                    {
                        case >= 53:
                            return new List<Club>() { Teams[1] };
                        default:
                            return null;
                    }
                case 1:
                    switch (rankingPosition)
                    {
                        case >= 24 and <= 52:
                            return new List<Club>() { Teams[1] };
                        default:
                            return null;
                    }
                case 2:
                    switch (rankingPosition)
                    {
                        case >= 17 and <= 23:
                            return new List<Club>() { Teams[1] };
                        case 16:
                            return new List<Club>() { Teams[1], Teams[2] };
                        case >= 11 and <= 15:
                            return new List<Club>() { Teams[2] };
                        case >= 8 and <= 10:
                            return new List<Club>() { Teams[3] };
                        default:
                            return null;
                    }
                case 3:
                    switch (rankingPosition)
                    {
                        case >= 11 and <= 15:
                            return new List<Club>() { Teams[1] };
                        case >= 8 and <= 10:
                            return new List<Club>() { Teams[2] };
                        case 7:
                            return new List<Club>() { Teams[3] };
                        case >= 5 and <= 6:
                            return new List<Club>() { Teams[4] };
                        default:
                            return null;
                    }
                case 4:
                    switch (rankingPosition)
                    {
                        case 7:
                            return new List<Club>() { Teams[2] };
                        case >= 5 and <= 6:
                            return new List<Club>() { Teams[3] };
                        case >= 1 and <= 4:
                            return new List<Club>() { Teams[5] };
                        default:
                            return null;
                    }
                case 5:  // group phase
                    switch (rankingPosition)
                    {
                        case >= 1 and <= 4:
                            return new List<Club>() { Teams[4] };
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }
        else
        {
            switch (roundIndex)
            {
                case 0:
                    switch (rankingPosition)
                    {
                        case >= 37:
                            return new List<Club>() { Teams[3] };
                        default:
                            return null;
                    }
                case 1:
                    switch (rankingPosition)
                    {
                        case >= 37:
                            return new List<Club>() { Teams[2] };
                        case >= 26 and <= 36:
                            return new List<Club>() { Teams[2], Teams[3] };
                        case >= 17 and <= 25:
                            return new List<Club>() { Teams[3]};
                        case 16:
                            return new List<Club>() { Teams[4]};
                        default:
                            return null;
                    }
                case 2:
                    switch (rankingPosition)
                    {
                        case >= 17 and <= 25:
                            return new List<Club>() { Teams[2] };
                        case 16:
                            return new List<Club>() { Teams[3] };
                        case >= 11 and <= 15:
                            return new List<Club>() { Teams[4] };
                        default:
                            return null;
                    }
                case 3:
                    switch (rankingPosition)
                    {
                        case >= 11 and <= 15:
                            return new List<Club>() { Teams[3] };
                        case >= 9 and <= 10:
                            return new List<Club>() { Teams[4] };
                        default:
                            return null;
                    }
                case 4:
                    switch (rankingPosition)
                    {
                        case >= 7 and <= 8:
                            return new List<Club>() { Teams[4] };
                        default:
                            return null;
                    }
                case 5:  // group phase
                    switch (rankingPosition)
                    {
                        case >= 5 and <= 6:
                            return new List<Club>() { Teams[5] };
                        case >= 1 and <= 4:
                            return new List<Club>() { Teams[6] };
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }
    }

    public static int[] GetPositionRanges(int rankingPos)
    {
        rankingPos++;
        int[] ranges = new int[4];

        ranges[3] = 3;  // last 3 teams are relegated TODO: set it inthe league scriptable object

        switch (rankingPos)
        {
            case >= 1 and <= 4:
                ranges[0] = 4;
                ranges[1] = 2;
                ranges[2] = 1;
                break;
            case >= 5 and <= 6:
                ranges[0] = 3;
                ranges[1] = 2;
                ranges[2] = 1;
                break;
            case >= 7 and <= 10:
                ranges[0] = 2;
                ranges[1] = 2;
                ranges[2] = 1;
                break;
            case >= 11 and <= 16:
                ranges[0] = 1;
                ranges[1] = 2;
                ranges[2] = 2;
                break;
            default:
                ranges[0] = 1;
                ranges[1] = 1;
                ranges[2] = 2;
                break;
        }

        return ranges;
    }

    public static int GetNumberOfClubsInEuropaCups(int rankingPos)
    {
        var ranges = GetPositionRanges(rankingPos);
        return ranges[0] + ranges[1] + ranges[2];
    }
}
