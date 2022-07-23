using System.Collections.Generic;
[System.Serializable]
public class MatchStats
{
    int shots,goals;
    public List<Scorer> scorers = new List<Scorer>();

    public MatchStats(List<Scorer> s)
    {
        Reset();
        scorers = s;
    }
    public void ShotTaken()
    {
        shots++;
    }
    public int GetShots()
    {
        return shots;
    }
    public void GoalScored()
    {
        goals++;
    }
    public int GetGoals()
    {
        return goals;
    }
    public void Reset()
    {
        shots = 0;
        goals = 0;
    }
    public void AddScorer(Footballer s, string club, int goalsScored)
    {
        for (int i = 0; i < scorers.Count; i++)
        {
            if(scorers[i].id == s.Id)
            {
                scorers[i].goals += goalsScored;
                return;
            }
        }
        scorers.Add(new Scorer(s.Id,s.Name,s.Surname,club, goalsScored));
    }
    public void AddScorer(Scorer s)
    {
        for (int i = 0; i < scorers.Count; i++)
        {
            if (scorers[i].id == s.id)
            {
                scorers[i].goals += s.goals;
                return;
            }
        }
        scorers.Add(new Scorer(s.id, s.name, s.surname, s.club, s.goals));
    }
}
