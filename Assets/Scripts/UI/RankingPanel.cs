using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RankingPanel : MonoBehaviour
{
    [SerializeField] List<RankingElement> _RankingElements;

    private void OnEnable()
    {
        //var countries = Database.clubDB.GroupBy(c => c.CountryName).OrderBy(c => c.Key);
        var query = from c in Database.clubDB
                    group c by c.CountryName into countries
                    select new
                    {
                        Country = countries.Key,
                        TotalPoints = countries.Sum(x => x.RankingPoints),
                        Season1Points = countries.Sum(x => x.GetRankingPoints(0)),
                        Season2Points = countries.Sum(x => x.GetRankingPoints(1)),
                        Season3Points = countries.Sum(x => x.GetRankingPoints(2)),
                        Season4Points = countries.Sum(x => x.GetRankingPoints(3)),
                        Season5Points = countries.Sum(x => x.GetRankingPoints(4))
                    };

        query = query.OrderByDescending(query => query.TotalPoints);

        int i = 0;

        foreach (var q in query)
        {
            if (i >= _RankingElements.Count)
                break;
            _RankingElements[i].SetData(q.Country, q.TotalPoints, q.Season1Points, q.Season2Points, q.Season3Points, q.Season4Points, q.Season5Points);
            i++;
        }
    }
}
