using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class CommentLine : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _ResultText;
    [SerializeField] TextMeshProUGUI _Commentary;
    string guestName, hostName;
    int guestId, hostId;
    List<Footballer>[] teams = new List<Footballer>[2];
    int[,] defWingPos = new int[2, 2];
    readonly Declination[] _host_guest_string = {new Declination("gospodarze", "gospodarzy", "gospodarzom", "gospodarzy", "gospodarzami", "gospodarzach"),
                                              new Declination("goście", "gości", "gościom", "gości", "gośćmi", "gościach")};
    readonly Declination[] _left_right_wing_string = {new Declination("lewe", "lewego", "lewemu", "lewe", "lewym", "lewym"),
                                              new Declination("prawe", "prawego", "prawemu", "prawe", "prawym", "prawym")};
    readonly string[] _left_right_direction_string = { "lewo","prawo"};

    public static CommentLine Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    public void UpdateTeams()
    {
        teams = Comment.Instance.GetTeams();
        defWingPos = Comment.Instance.GetDefWingPos();
    }
    public void StartingSettings()
    {
        _Commentary.text = "";
        guestName = Comment.Instance.GetGuestName();
        hostName = Comment.Instance.GetHostName();
        _ResultText.text = $"{hostName} 0 - 0 {guestName}";
        guestId = Comment.Instance.GetGuestID();
        hostId = Comment.Instance.GetHostID();
        UpdateTeams();
    }
    public void EndOfTheMatch()
    {
        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. Sędzia gwiżdże po raz ostatni. Koniec meczu";
    }
    public void StartOfTheMatch()
    {
        
        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. Pierwszy gwizdek sędziego i " + guestName + " może rozpocząć grę.";
    }
    public void BoringPartOfTheMatch()
    {
        
        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. Bardzo słaba część spotkania, piłka ciagle znajduje się w środku boiska.";
    }
    public void StartingComment()
    {
        int rnd = Random.Range(1, 11);
        int arb = Random.Range(0, 5);
        string text;
        List<Footballer> hostTeam = teams[0];
        List<Footballer> guestTeam = teams[1];
        switch (rnd)
        {
            case 1: text = "\nWitam, dzisiaj przed nami bardzo ciekawe widowisko. Mecz: " + hostName + " - " + guestName + "."; break;
            case 2: text = "\nWitam z " + Database.clubDB[hostId].Stadium + ", dziś przed nami bardzo ciekawy mecz: " + hostName + " - " + guestName + "."; break;
            case 3: text = "\nDeszczowy wieczór i spotkanie rozgrywane na " + Database.clubDB[hostId].Stadium + ". Jak zwykle liczymy na emocje."; break;
            case 4: text = "\nPiłkarze wychodzą na boisko, skupieni czekają na pierwszy gwizdek arbitra."; break;
            case 5: text = "\nArbitrem dzisiejszego spotkania jest " + Database.arbiterDB[0, arb] + "-letni " + Database.arbiterDB[1, arb] + "."; break;
            default: text = "\nDzisiaj " + hostName + " w składzie: " + hostTeam[0].Surname + ", " + hostTeam[1].Surname + ", " + hostTeam[2].Surname + ", " + hostTeam[3].Surname + ", " + hostTeam[4].Surname + ", " + hostTeam[5].Surname + ", " + hostTeam[6].Surname + ", " + hostTeam[7].Surname + ", " + hostTeam[8].Surname + ", " + hostTeam[9].Surname + ", " + hostTeam[10].Surname + ".\n" + guestName + " w składzie: " + guestTeam[0].Surname + ", " + guestTeam[1].Surname + ", " + guestTeam[2].Surname + ", " + guestTeam[3].Surname + ", " + guestTeam[4].Surname + ", " + guestTeam[5].Surname + ", " + guestTeam[6].Surname + ", " + guestTeam[7].Surname + ", " + guestTeam[8].Surname + ", " + guestTeam[9].Surname + ", " + guestTeam[10].Surname + "."; break;
        }
        _Commentary.text += text;
    }
    public void InfoComment()
    {
        MatchStats[] ms = Comment.Instance.GetMatchStats();
        int hostShots = ms[0].GetShots();
        int guestShots = ms[1].GetShots();
        int hostChances = Comment.Instance.GetHostChances();
        int hostGoals = ms[0].GetGoals();
        int guestGoals = ms[1].GetGoals();
        int rnd = Random.Range(1, 9);
        string text = "";
        int attendence = Random.Range((Database.clubDB[guestId].Rate / 10) * Database.clubDB[hostId].StadiumCapacity, Database.clubDB[hostId].StadiumCapacity);
        switch (rnd)
        {
            case 1: text = "Gra toczy się aktualnie na środku boiska."; break;
            case 2: text = "Żadna z drużyn nie ma pomysłu na przebicie się przez obronę przeciwnika."; break;
            case 3: text = "Na " + Database.clubDB[hostId].Stadium + " zasiada dzisiaj " + attendence + "."; break;
            case 4: text = "Nic się teraz nie dzieje na boisku."; break;
            case 5: text = "Statystyka strzałów prezentuje się następująco: " + hostName + " - " + hostShots + ", " + guestName + " - " + guestShots + "."; break;
        }
        if (rnd == 6)
        {
            int pos = Random.Range(50, 70);
            if ((hostChances >= 50 && hostGoals == guestGoals) || hostGoals > guestGoals)
                text = "Posiadanie piłki: " + hostName + " - " + pos + ", " + guestName + " - " + (100 - pos) + ".";
            else if ((hostChances < 50 && hostGoals == guestGoals) || hostGoals < guestGoals)
                text = "Posiadanie piłki: " + guestName + " - " + pos + ", " + hostName + " - " + (100 - pos) + ".";
        }
        if (rnd == 7)
        {
            if ((hostChances >= 50 && hostGoals == guestGoals) || hostGoals > guestGoals)
                text = hostName + " kontroluje wydarzenia na boisku.";
            else if ((hostChances < 50 && hostGoals == guestGoals) || hostGoals < guestGoals)
                text = guestName + " kontroluje wydarzenia na boisku.";
        }
        if (rnd == 8)
        {
            if ((hostChances >= 50 && hostGoals == guestGoals) || hostGoals < guestGoals)
                text = hostName + " stosuje wysoki pressing na połowie przeciwnika.";
            else if ((hostChances < 50 && hostGoals == guestGoals) || hostGoals > guestGoals)
                text = guestName + " stosuje wysoki pressing na połowie przeciwnika.";
        }
        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. " + text;

    }
    public void AttackFirstPhase()
    {
        int rnd = Random.Range(10, 70);
        rnd /= 10;
        string text = "";
        switch (rnd)
        {
            case 1: text += "Akcje " + _host_guest_string[Comment.Instance.GetGuestBall()].getGenitive() + " długim podaniem rozpoczyna " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ".";break;
            case 2: text += "Przy piłce " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ", przyjmuje piłkę i podaje.";break;
            case 3: text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " dostaje piłkę i odgrywa ją do partnera.";break;
            case 4: text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " podaje piłkę do kolegi.";break;
            case 5: text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " w posiadaniu piłki, szuka kolegi z zespołu i podaje.";break;
            case 6: text += "Teraz " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ", od razu podaje do przodu.";break;
        }

        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. " + text;
    }
    public void InterceptionAndCounter()
    {
        _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " przechwytuje piłkę i " + _host_guest_string[Comment.Instance.GetGuestBall()].getNominative() + " wychodzą z kontratakiem.";
    }
    public void PassMiddle()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nPiłka trafia do " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ". Ten dogrywa do lepiej ustawionego partnera.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " dostał piłkę i podaje ją do przodu.";
                break;
            case 3:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " po otrzymaniu futbolówki niezwłocznie oddaje ją do nieatakowanego partnera.";
                break;
        }
    }
    public void PassToTheWing(int isRightWing)
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nPiłka trafia do " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ". Zagranie na " + _left_right_wing_string[isRightWing].getNominative() + " skrzydło.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " podaje na skrzydło.";
                break;
            case 3:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " po otrzymaniu piłki bez zastanowienia posyła ją na " + _left_right_direction_string[isRightWing] + ".";
                break;
        }
    }
    public void DecidesToShoot()
    {
        _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " decyduje się na uderzenie z daleka...";
    }
    public void PreparingToPenalty()
    {
        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. Do piłki podchodzi " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + "...";
    }
    public void PenaltyGoal()
    {
        int ran = Random.Range(10, 40);
        ran /= 10;
        string text = "";
        switch (ran)
        {
            case 1:
                text = "Gol, pewnie wykorzystany karny.";
                break;
            case 2:
                text = "Fantastyczny strzał, " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " bez szans.";
                break;
            case 3:
                text = "Mocno, w dolny róg bramki i gol dla " + _host_guest_string[Comment.Instance.GetGuestBall()].getGenitive() + ".";
                break;
        }
        _Commentary.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public void PenaltyMissed()
    {
        int ran = Random.Range(10, 40);
        ran /= 10;
        string text = "";
        switch (ran)
        {
            case 1:
                text = teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " wyczuł intencje strzelajacego i łapie piłkę.";
                break;
            case 2:
                text = "Za słaby strzał, " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odbija bez problemu.";
                break;
            case 3:
                text = "Mocno, nad bramką i nadal wynik bez zmian.";
                break;
        }
        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. " + text;
    }
    public void LongShotCorner()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nStrzela " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ", ale piłka po rykoszecie wypada za boisko, Rzut rożny.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ", dobry strzał i dobra obrona bramkarza, wybija piłkę na rzut rożny.";
                break;
            case 3:
                _Commentary.text += "\nStrzał " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + " zablokowany przez jednego z graczy " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public void LongShotGoal()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, cudowny strzał " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ".";
                break;
            case 2:
                text = "Piękny gol " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ". Piłka w oknie bramki " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Przepiękne uderzenie " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + " i gol.";
                break;
        }
        _Commentary.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public void LongShotMiss()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nAle bardzo nieudana próba.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " strzela z daleka ale niecelnie.";
                break;
            case 3:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " strzela z dystansu ale piłka ląduje poza boiskiem.";
                break;
        }
    }
    public void CounterAttackSave()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + ", z problemami ale łapie piłkę.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " wyłapuje to niegroźne uderzenie.";
                break;
            case 3:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " decyduje się złapać piłkę zamiast wybić ją do boku i udaje mu się to.";
                break;
        }
    }
    public void CounterAttackCorner()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nCoż za interwencja " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", rzut rożny.";
                break;
            case 2:
                _Commentary.text += "\nDobry strzał i bardzo dobra obrona bramkarza, mamy korner. To powinna być bramka.";
                break;
            case 3:
                _Commentary.text += "\nŚwietna obrona " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", ratuje " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public void CounterAttackGoal()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, piękne uderzenie " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ".";
                break;
            case 2:
                text = "Piękny gol " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ". Piłka w samym okienku bramki " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Kapitalne uderzenie, po ziemi " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + " i gol.";
                break;
        }
        _Commentary.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public void CounterAttackMiss()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odprowadza piłkę wzrokiem. Niecelny strzał.";
                break;
            case 3:
                _Commentary.text += "\nWysoko nad bramką, bardzo słaby strzał. Dlaczego nie podawał?";
                break;
        }
    }
    public void NormalAttackSave()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + ", z problemami ale łapie piłkę.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " wyłapuje to niegroźne uderzenie.";
                break;
            case 3:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " decyduje się złapać piłkę zamiast wybić ją do boku i udaje mu się to.";
                break;
        }
    }
    public void NormalAttackCorner()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nCoż za interwencja " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", rzut rożny.";
                break;
            case 2:
                _Commentary.text += "\nDobry strzał ale bardzo dobra parada " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", mamy korner. To powinna być bramka.";
                break;
            case 3:
                _Commentary.text += "\nDoskonała obrona " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + " ratuje " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public void NormalAttackGoal()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, piękne uderzenie " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ".";
                break;
            case 2:
                text = "Piękny gol " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ". Piłka w samym okienku bramki " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Kapitalne uderzenie, po ziemi " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + " i gol.";
                break;
        }
        _Commentary.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public void NormalAttackMiss()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odprowadza piłkę wzrokiem. Niecelny strzał.";
                break;
            case 3:
                _Commentary.text += "\nWysoko nad bramką, bardzo słaby strzał. Dlaczego nie podawał?";
                break;
        }
    }
    public void OneOnOneAttackSave()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + ", wyłuskuje piłkę spod nóg przeciwnika.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " próbuje ominąć " +teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + " ale nie udaje mu się to i piłka w rękach bramkarza.";
                break;
            case 3:
                _Commentary.text += "\nStrzał odbity do boku przez " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " i niebezpieczeństwo zażegnane.";
                break;
        }
    }
    public void OneOnOneAttackCorner()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nCoż za interwencja " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", rzut rożny.";
                break;
            case 2:
                _Commentary.text += "\nSłaby strzał i dobrze spisał się " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + ", " + _host_guest_string[Comment.Instance.GetGuestBall()].getNominative() + " wznowią grę z rogu boiska. To powinno zakoćzyć się zmianą wyniku...";
                break;
            case 3:
                _Commentary.text += "\nDoskonała obrona " + teams[Comment.Instance.GetReverseIsGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ", ratuje " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public void OneOnOneAttackGoal()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " mija bramkarza i pakuje piłkę do pustej bramki.";
                break;
            case 2:
                text = "Cudowny gol " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ". Piłka w samym okienku bramki " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Czysto uderza po ziemi " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " i gol.";
                break;
        }
        _Commentary.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public void OneOnOneAttackMiss()
    {
        
        int rnd = Random.Range(10, 50);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odprowadza piłkę wzrokiem. Zmarnowana 100 % okazja.";
                break;
            case 3:
                _Commentary.text += "\nWysoko nad bramką, bardzo słaby strzał. Takie sytuacje trzeba wykorzystywać.";
                break;
            case 4:
                _Commentary.text += "\nStrzał i słupek ratuje " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive();
                break;
        }
    }
    public void HeaderSave()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " bez trudu łapie piłkę.";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " prosto w ręce " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ".";
                break;
            case 3:
                _Commentary.text += "\nStrzał odbity do boku przez " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " i niebezpieczeństwo zażegnane.";
                break;
        }
    }
    public void HeaderCorner()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nCoż za interwencja " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", rzut rożny.";
                break;
            case 2:
                _Commentary.text += "\nNiezbyt dobry strzał " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ", ale " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " z problemami wybija piłkę na rzut rożny.";
                break;
            case 3:
                _Commentary.text += "\nDoskonała obrona " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + " ratuje " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public void HeaderGoal()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " bez szans";
                break;
            case 2:
                text = "Świetny strzał " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ". Piłka w okienku bramki " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Bramka dla " + _host_guest_string[Comment.Instance.GetGuestBall()].getGenitive() + " po strzale głową.";
                break;
        }
        _Commentary.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public void HeaderMiss()
    {
        
        int rnd = Random.Range(10, 50);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nBardzo blisko, ale jednak obok bramki strzeżonej przez " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ".";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odprowadza piłkę wzrokiem. Zmarnowana dobra okazja na zdobycie bramki.";
                break;
            case 3:
                _Commentary.text += "\nWysoko nad bramką, bardzo słaba główka. Takie sytuacje mogą się zemścić.";
                break;
            case 4:
                _Commentary.text += "\nPoprzeczka ratuje " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive();
                break;
        }
    }
    public void CounterAttackShotTry()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " przebiega kilkanaście metrów i strzela...";
                break;
            case 2:
                _Commentary.text += "\nPiłkę przy nodze ma " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " i decyduje się na uderzenie...";
                break;
            case 3:
                _Commentary.text += "\nPrzy piłce " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ", widzi wysuniętego bramkarza...";
                break;
        }
    }
    public void CounterAttackFailedPass()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nFatalne podanie " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + " i koniec kontrataku.";
                break;
            case 2:
                _Commentary.text += "\nPogubił się " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " podaje wprost do przeciwnika.";
                break;
            case 3:
                _Commentary.text += "\nPodaje " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ", ale piłka przechwycona przez obronę.";
                break;
        }
    }
    public void CounterAttackPenaltyFoul()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nAle zostaje podcięty przez obrońcę i mamy jedenastkę.";
                break;
            case 2:
                _Commentary.text += "\nW ostatniej chwili obrońca wchodzi na wślizgu w nogi atakującego i mamy rzut karny.";
                break;
            case 3:
                _Commentary.text += "\nI zostaje nieprzepisowo zatrzymany przez nadbiegającego obrońcę, karny.";
                break;
        }
    }
    public void CounterAttackPreShot()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ", ten ma przed sobą tylko bramkarza.";
                break;
            case 2:
                _Commentary.text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ", ten jest sam na sam z bramkarzem.";
                break;
            case 3:
                _Commentary.text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].AlteredSurname + ", który jest teraz na czystej pozycji.";
                break;
        }
    }
    public void CounterAttackSuccessPass()
    {
        int rnd = Random.Range(10, 30);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " podaje do ";
                break;
            case 2:
                _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " zgrywa do ";
                break;
        }
    }
    public void CornerExecution(Footballer f)
    {
        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. Stały fragment gry wykonywać będzie " + f.Surname + ". Dośrodkowanie...";
    }
    public void FreeHeader()
    {
        _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " urywa się spod opieki obrońcy i strzela...";
    }
    public void ContestedHeader()
    {
        _Commentary.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + " wygrywa pojedynek główkowy...";
    }
    public void DefenderWinsHeader(Footballer defender)
    {
        _Commentary.text += "\nDobre dośrodkowanie, ale " + defender.Surname + " wygrywa pojedynek główkowy.";
    }
    public void FailedCross()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nZłe było to dośrodkowanie i obrońca wybija bez problemu.";
                break;
            case 2:
                _Commentary.text += "\nNieźle wykonane, ale " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " łapie piłkę.";
                break;
            case 3:
                _Commentary.text += "\nZa dużo siły w tym dograniu i aut dla " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
        }
    }
    public void TryingToDodge()
    {
        
        _Commentary.text += "\n" + Comment.Instance.GetMinute() + " min. Piłkę przy nodze ma " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ". próbuje minąć przeciwnika...";
    }
    public void DecidesToCross()
    {
        
        _Commentary.text += "\nMinął rywala i dośrodkowuje...";
    }
    public void DecidesToShootInsteadOfCrossing()
    {
        
        _Commentary.text += "\nOszukał obrońcę i decyduje się na strzał...";
    }
    public void FailedWingDribble(int direction)
    {
        _Commentary.text += "\nSprytna próba, ale " + teams[Comment.Instance.GetReverseIsGuestBall()][defWingPos[Comment.Instance.GetReverseIsGuestBall(), direction]].Surname + " nie dał się nabrać i zabrał piłkę.";
    }
    public void FailedMidDribble(Footballer defender)
    {
        
        _Commentary.text += "\nSprytna próba, ale " + defender.Surname + " nie dał się nabrać i zabrał piłkę.";
    }
    public void DecidesToShootInsteadOfPassing()
    {
        
        _Commentary.text += "\nOminął obrońcę i decyduje się na strzał...";
    }
    public void DecidesToPass()
    {
        
        _Commentary.text += "\nOminął obrońcę i podaje do lepiej ustawionego partnera...";
    }
    public void FailedPass()
    {
        
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                _Commentary.text += "\nZłe było to podanie i obrońca przechwytuje bez problemu.";
                break;
            case 2:
                _Commentary.text += "\nNieźle wykonane, ale trochę za mocno i " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " łapie piłkę.";
                break;
            case 3:
                _Commentary.text += "\nZa dużo siły w tym dograniu i " + _host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " zaczną od bramki.";
                break;
        }
    }
    public void ChanceForOneOnOne()
    {
        _Commentary.text += "\nTam jest " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.PlayerWithBall].Surname + ", odwraca się, a przed nim tylko jeden obrońca...";
    }
    public void FailedChanceOneToOne()
    {
        _Commentary.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][Comment.Instance.PlayerWithBall].Surname + " wyłuskuje piłkę napastnikowi i wybija ją.";
    }
    public void OneToOneSituation()
    {
        _Commentary.text += "\nMija go, jeszcze tylko " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " może zapobiec utracie bramki...";
    }
    public void UpdateResult(MatchStats[] matchStats)
    {
        _ResultText.text = $"{hostName} {matchStats[0].GetGoals()} - {matchStats[1].GetGoals()} {guestName}";
    }
}
