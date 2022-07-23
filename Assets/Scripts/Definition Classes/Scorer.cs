
[System.Serializable]
public class Scorer
{
    public int id, goals;
    public string name, surname, club;
    public Scorer(int id, string name, string surname, string club, int goals)
    {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.club = club;
        this.goals = goals;
    }
}
