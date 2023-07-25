public class PlayersMatchData
{
    public float MatchRating { get; private set; } = 6;

    public float UpdateMatchRating(float change) => MatchRating += change;
}
