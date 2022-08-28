using System;
using System.Collections.Generic;

public static class MatchCalendar
{
    // firstDaymatches określa ile meczow ma byc rozegranych w pierwszym dniu kazdego tygodnia, reszta rozegrana zostanie nastepnego dnia
    // domyslnie zero co onzacza ze wszystkie emcze danej rundy beda rozgrywane w tym samym dniu
    // liga mistrzow jest dobrym przykladem wykorzystania tego argumentu, jesli firstDayMatches = 8, to 8 spotkan we wtorek ,a pozostale (8) w srode
    /// <summary>
    /// 
    /// </summary>
    /// <param name="matches"></param>
    /// <param name="competitionName">Name of the competition</param>
    /// <param name="clubs">Clubs envolved in a competition</param>
    /// <param name="start">Competition start date</param>
    /// <param name="secondDayMatches">Matches played on the second day</param>
    /// <param name="thirdDayMatches"></param>
    /// <param name="fourthDayMatches"></param>
    public static List<Match> CreateGroupCalendar(/*List<Match> matches,*/ string competitionName,int roundIndex, List<Club> clubs, DateTime start,int periodBetweenRounds = 7, int secondDayMatches = 0, int thirdDayMatches = 0, int fourthDayMatches = 0)
    {
        DateTime start2 = start;
        //int firstDayMatches = 0;
        if (clubs.Count < 2)
        {
            return null;
        }
        List<Match> ms = new List<Match>();
        int clubsCount = clubs.Count;
        int roundsAmount = clubsCount * 2 - 2;
        int[] help = new int[clubs.Count + 1];
        for (int i = 0; i < clubsCount; i++)
        {
            help[i] = clubs[i].Id;
        }
        help[clubs.Count] = -1;
        for (int i = 0; i < roundsAmount; i++)
        {
            if (i > 0)
            {
                for (int x = clubsCount; x > 1; x--)
                {
                    help[x] = help[x - 1];
                }
                help[1] = help[clubsCount];
            }
            for (int x = 0; x < clubsCount / 2; x++)
            {
                if(i < roundsAmount/2)
                    ms.Add(new Match(competitionName, start2, i, help[x], help[clubsCount - x - 1]));
                else 
                    ms.Add(new Match(competitionName, start2, i, help[clubsCount - x - 1], help[x]));
            }
            start2 = start2.AddDays(periodBetweenRounds);
        }

        if (MyClub.Instance.Matches.Count == 0)
        {
            foreach (var item in ms)
            {
                MyClub.Instance.Matches.Add(item);
            }
            return ms;
        }
        int matchesPerRound = clubs.Count / 2;
        int addToTheEnd = -matchesPerRound;
        for(int i = 0; i < ms.Count; i += matchesPerRound)
        {
            // round
            bool toTheEnd = true;
            for (int m = 0; m < MyClub.Instance.Matches.Count; m++)
            {
                if (MyClub.Instance.Matches[m].Date > start)
                {
                    // jesli to nie poczatek listy i poprzedni mecz ma ta sama date co runda ,ktora chcemy dodac
                    if (m != 0 && MyClub.Instance.Matches[m - 1].Date == start && MyClub.Instance.Matches[m-1].CompetitionName != competitionName)
                    {
                        start = start.AddDays(periodBetweenRounds);
                    }
                    else
                    {
                        toTheEnd = false;
                        for (int r = i+matchesPerRound-1; r >= i; r--)
                        {
                            if (roundIndex >= 0) ms[r].RoundIndex = roundIndex;
                            ms[r].Date = start;
                            MyClub.Instance.Matches.Insert(m, ms[r]);
                        }
                        start = start.AddDays(periodBetweenRounds);
                        addToTheEnd = i+matchesPerRound;
                        break;
                    }
                }
            }
            if (toTheEnd) 
                break;
        }
        if (addToTheEnd < ms.Count - 1)
        {
            for (int i = addToTheEnd; i < ms.Count; i += matchesPerRound)
            {
                for (int r = i; r < i+matchesPerRound; r++)
                {
                    if (roundIndex >= 0) 
                        ms[r].RoundIndex = roundIndex;
                    ms[r].Date = start;
                    MyClub.Instance.Matches.Add(ms[r]);
                }
                start = start.AddDays(periodBetweenRounds);
            }
        }
        return ms;
    }
}
