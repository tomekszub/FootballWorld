using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class League_Old 
{
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
    public Club GetPreeliminary_CL_Clubs(int rankingPos)
    {
        if (rankingPos >= 53)
        {
            return Teams[0];
        }
        else return null;
    }
    public Club GetFirstQ_CL_Clubs(int rankingPos)
    {
        if (rankingPos >= 20 && rankingPos <= 52)
        {
            return Teams[0];
        }
        else return null;
    }
    public Club GetSecondChampionsPathQ_CL_Clubs(int rankingPos)
    {
        if (rankingPos >= 17 && rankingPos <= 19)
        {
            return Teams[0];
        }
        else return null;
    }
    public Club GetSecondLeaguePathQ_CL_Clubs(int rankingPos)
    {
        if (rankingPos >= 10 && rankingPos <= 15)
        {
            return Teams[1];
        }
        else return null;
    }
    public Club GetThirdChampionsPathQ_CL_Clubs(int rankingPos)
    {
        if (rankingPos >= 15 && rankingPos <= 16)
        {
            return Teams[0];
        }
        else return null;
    }
    public Club GetThirdLeaguePathQ_CL_Clubs(int rankingPos)
    {
        if (rankingPos >= 7 && rankingPos <= 9)
        {
            return Teams[1];
        }
        else if(rankingPos >= 5 && rankingPos <= 6)
        {
            return Teams[2];
        }
        else return null;
    }
    public Club GetPlayOffsChampionsPathQ_CL_Clubs(int rankingPos)
    {
        if (rankingPos >= 13 && rankingPos <= 14)
        {
            return Teams[0];
        }
        else return null;
    }
    public List<Club> GetGroupStage_CL_Clubs(int rankingPos)
    {
        List<Club> ret = new List<Club>();
        if (rankingPos >= 7 && rankingPos <= 12)
        {
            ret.Add(Teams[0]);
        }
        else if (rankingPos >= 5 && rankingPos <= 6)
        {
            ret.Add(Teams[0]);
            ret.Add(Teams[1]);
        }
        else if (rankingPos >= 1 && rankingPos <= 4)
        {
            ret.Add(Teams[0]);
            ret.Add(Teams[1]);
            ret.Add(Teams[2]);
            ret.Add(Teams[3]);
        }
        else return null;
        return ret;
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

    public int[] GetPositionRanges(int rankingPos)
    {
        int[] ranges = new int[4];

        ranges[3] = 3;  // last 3 teams are relegated TODO: set it inthe league scriptable object

        switch (rankingPos)
        {
            case >= 1 and <= 4:
                ranges[0] = 4;
                ranges[1] = 2;
                ranges[2] = 1;
                break;
            case 5:
                ranges[0] = 3;
                ranges[1] = 2;
                ranges[2] = 1;
                break;
            case 6:
                ranges[0] = 3;
                ranges[1] = 1;
                ranges[2] = 2;
                break;
            case >= 7 and <= 15:
                ranges[0] = 2;
                ranges[1] = 1;
                ranges[2] = 2;
                break;
            case >= 16 and <= 49:
                ranges[0] = 1;
                ranges[1] = 0;
                ranges[2] = 3;
                break;
            default:
                ranges[0] = 1;
                ranges[1] = 0;
                ranges[2] = 2;
                break;
        }

        return ranges;
    }
}
