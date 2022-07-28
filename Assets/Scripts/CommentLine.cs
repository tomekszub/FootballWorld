using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentLine : MonoBehaviour
{
    static Text resultText;
    static Text t;
    //static bool test = false;
    //static bool simulation = false;
    static Database database;
    static string guestName, hostName;
    static int guestId, hostId;
    static List<Footballer>[] teams = new List<Footballer>[2];
    static List<int>[] teamsMidPos = new List<int>[2];
    static int[] defLastPlayerNumber = new int[2];
    static int[] midLastPlayerNumber = new int[2];
    //static int[] leftPos = new int[2];
    //static int[] rightPos = new int[2];
    static int[,] wingPos = new int[2, 2];
    static int[,] defWingPos = new int[2, 2];
    //static string[] writeHostGuest = { "goście","gospodarze","gości","gospodarze"};
    static Declination[] host_guest_string = {new Declination("gospodarze", "gospodarzy", "gospodarzom", "gospodarzy", "gospodarzami", "gospodarzach"),
                                              new Declination("goście", "gości", "gościom", "gości", "gośćmi", "gościach")};
    static Declination[] left_right_wing_string = {new Declination("lewe", "lewego", "lewemu", "lewe", "lewym", "lewym"),
                                              new Declination("prawe", "prawego", "prawemu", "prawe", "prawym", "prawym")};
    static string[] left_right_direction_string = { "lewo","prawo"};

    static CommentLine instance;
    void Start()
    {
        t = transform.Find("Text").GetComponent<Text>();
        resultText = transform.parent.parent.Find("StatsPanel").Find("Result").GetComponent<Text>();  // glupie to ale dziala
        instance = this;
    }
    public static void UpdateTeams()
    {
        teams = Comment.Instance.GetTeams();
        teamsMidPos = Comment.Instance.GetMidPos();
        defLastPlayerNumber = Comment.Instance.GetDefLastPlayerNumber();
        midLastPlayerNumber = Comment.Instance.GetMidLastPlayerNumber();
        wingPos = Comment.Instance.GetWingPos();
        defWingPos = Comment.Instance.GetDefWingPos();
    }
    public static void StartingSettings()
    {
        //test = Comment.coomment_or_teams_Test;
        t.text = "";
        guestName = Comment.Instance.GetGuestName();
        hostName = Comment.Instance.GetHostName();
        guestId = Comment.Instance.GetGuestID();
        hostId = Comment.Instance.GetHostID();
        UpdateTeams();
    }
    public static void EndOfTheMatch()
    {
        t.text += "\n" + Comment.Instance.GetMinute() + " min. Sędzia gwiżdże po raz ostatni. Koniec meczu";
    }
    public static void StartOfTheMatch()
    {
        
        t.text += "\n" + Comment.Instance.GetMinute() + " min. Pierwszy gwizdek sędziego i " + guestName + " może rozpocząć grę.";
    }
    public static void BoringPartOfTheMatch()
    {
        
        t.text += "\n" + Comment.Instance.GetMinute() + " min. Bardzo słaba część spotkania, piłka ciagle znajduje się w środku boiska.";
    }
    public static void StartingComment()
    {
        int rnd = Random.Range(1, 11);
        int arb = Random.Range(0, 5);
        string text = "";
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
        t.text += text;
    }
    public static void InfoComment()
    {
        
        MatchStats[] ms = Comment.Instance.GetMatchStats();
        int hostShots = ms[0].GetShots();
        int guestShots = ms[1].GetShots();
        int hostChances = Comment.Instance.GetHostChances();
        //int goalChances = Comment.GetGoalChances();
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
        t.text += "\n" + Comment.Instance.GetMinute() + " min. " + text;

    }
    public static void AttackFirstPhase()
    {
        int rnd = Random.Range(10, 70);
        rnd /= 10;
        string text = "";
        switch (rnd)
        {
            case 1: text += "Akcje " + host_guest_string[Comment.Instance.GetGuestBall()].getGenitive() + " długim podaniem rozpoczyna " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ".";break;
            case 2: text += "Przy piłce " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ", przyjmuje piłkę i podaje.";break;
            case 3: text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " dostaje piłkę i odgrywa ją do partnera.";break;
            case 4: text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " podaje piłkę do kolegi.";break;
            case 5: text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " w posiadaniu piłki, szuka kolegi z zespołu i podaje.";break;
            case 6: text += "Teraz " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ", od razu podaje do przodu.";break;
        }

        t.text += "\n" + Comment.Instance.GetMinute() + " min. " + text;
    }
    public static void InterceptionAndCounter()
    {
        t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " przechwytuje piłkę i " + host_guest_string[Comment.Instance.GetGuestBall()].getNominative() + " wychodzą z kontratakiem.";
    }
    public static void PassMiddle()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nPiłka trafia do " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ". Ten dogrywa do lepiej ustawionego partnera.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " dostał piłkę i podaje ją do przodu.";
                break;
            case 3:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " po otrzymaniu futbolówki niezwłocznie oddaje ją do nieatakowanego partnera.";
                break;
        }
    }
    public static void PassToTheWing(int isRightWing)
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nPiłka trafia do " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ". Zagranie na " + left_right_wing_string[isRightWing].getNominative() + " skrzydło.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " podaje na skrzydło.";
                break;
            case 3:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " po otrzymaniu piłki bez zastanowienia posyła ją na " + left_right_direction_string[isRightWing] + ".";
                break;
        }
    }
    public static void DecidesToShoot()
    {
        t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " decyduje się na uderzenie z daleka...";
    }
    public static void PreparingToPenalty()
    {
        t.text += "\n" + Comment.Instance.GetMinute() + " min. Do piłki podchodzi " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + "...";
    }
    public static void PenaltyGoal()
    {
        
        int ran = Random.Range(10, 40);
        ran = ran / 10;
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
                text = "Mocno, w dolny róg bramki i gol dla " + host_guest_string[Comment.Instance.GetGuestBall()].getGenitive() + ".";
                break;
        }
        t.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public static void PenaltyMissed()
    {
        
        int ran = Random.Range(10, 40);
        ran = ran / 10;
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
        t.text += "\n" + Comment.Instance.GetMinute() + " min. " + text;
    }
    public static void LongShotCorner()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nStrzela " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ", ale piłka po rykoszecie wypada za boisko, Rzut rożny.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ", dobry strzał i dobra obrona bramkarza, wybija piłkę na rzut rożny.";
                break;
            case 3:
                t.text += "\nStrzał " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + " zablokowany przez jednego z graczy " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public static void LongShotGoal()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, cudowny strzał " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ".";
                break;
            case 2:
                text = "Piękny gol " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ". Piłka w oknie bramki " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Przepiękne uderzenie " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + " i gol.";
                break;
        }
        t.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public static void LongShotMiss()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nAle bardzo nieudana próba.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " strzela z daleka ale niecelnie.";
                break;
            case 3:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " strzela z dystansu ale piłka ląduje poza boiskiem.";
                break;
        }
    }
    public static void CounterAttackSave()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + ", z problemami ale łapie piłkę.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " wyłapuje to niegroźne uderzenie.";
                break;
            case 3:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " decyduje się złapać piłkę zamiast wybić ją do boku i udaje mu się to.";
                break;
        }
    }
    public static void CounterAttackCorner()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nCoż za interwencja " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", rzut rożny.";
                break;
            case 2:
                t.text += "\nDobry strzał i bardzo dobra obrona bramkarza, mamy korner. To powinna być bramka.";
                break;
            case 3:
                t.text += "\nŚwietna obrona " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", ratuje " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public static void CounterAttackGoal()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, piękne uderzenie " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ".";
                break;
            case 2:
                text = "Piękny gol " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ". Piłka w samym okienku bramki " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Kapitalne uderzenie, po ziemi " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + " i gol.";
                break;
        }
        t.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public static void CounterAttackMiss()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odprowadza piłkę wzrokiem. Niecelny strzał.";
                break;
            case 3:
                t.text += "\nWysoko nad bramką, bardzo słaby strzał. Dlaczego nie podawał?";
                break;
        }
    }
    public static void NormalAttackSave()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + ", z problemami ale łapie piłkę.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " wyłapuje to niegroźne uderzenie.";
                break;
            case 3:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " decyduje się złapać piłkę zamiast wybić ją do boku i udaje mu się to.";
                break;
        }
    }
    public static void NormalAttackCorner()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nCoż za interwencja " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", rzut rożny.";
                break;
            case 2:
                t.text += "\nDobry strzał ale bardzo dobra parada " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", mamy korner. To powinna być bramka.";
                break;
            case 3:
                t.text += "\nDoskonała obrona " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + " ratuje " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public static void NormalAttackGoal()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, piękne uderzenie " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ".";
                break;
            case 2:
                text = "Piękny gol " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ". Piłka w samym okienku bramki " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Kapitalne uderzenie, po ziemi " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + " i gol.";
                break;
        }
        t.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public static void NormalAttackMiss()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odprowadza piłkę wzrokiem. Niecelny strzał.";
                break;
            case 3:
                t.text += "\nWysoko nad bramką, bardzo słaby strzał. Dlaczego nie podawał?";
                break;
        }
    }
    public static void OneOnOneAttackSave()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + ", wyłuskuje piłkę spod nóg przeciwnika.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " próbuje ominąć " +teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + " ale nie udaje mu się to i piłka w rękach bramkarza.";
                break;
            case 3:
                t.text += "\nStrzał odbity do boku przez " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " i niebezpieczeństwo zażegnane.";
                break;
        }
    }
    public static void OneOnOneAttackCorner()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nCoż za interwencja " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", rzut rożny.";
                break;
            case 2:
                t.text += "\nSłaby strzał i dobrze spisał się " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + ", " + host_guest_string[Comment.Instance.GetGuestBall()].getNominative() + " wznowią grę z rogu boiska. To powinno zakoćzyć się zmianą wyniku...";
                break;
            case 3:
                t.text += "\nDoskonała obrona " + teams[Comment.Instance.GetReverseIsGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ", ratuje " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public static void OneOnOneAttackGoal()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " mija bramkarza i pakuje piłkę do pustej bramki.";
                break;
            case 2:
                text = "Cudowny gol " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ". Piłka w samym okienku bramki " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Czysto uderza po ziemi " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " i gol.";
                break;
        }
        t.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public static void OneOnOneAttackMiss()
    {
        
        int rnd = Random.Range(10, 50);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odprowadza piłkę wzrokiem. Zmarnowana 100 % okazja.";
                break;
            case 3:
                t.text += "\nWysoko nad bramką, bardzo słaby strzał. Takie sytuacje trzeba wykorzystywać.";
                break;
            case 4:
                t.text += "\nStrzał i słupek ratuje " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive();
                break;
        }
    }
    public static void HeaderSave()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " bez trudu łapie piłkę.";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " prosto w ręce " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ".";
                break;
            case 3:
                t.text += "\nStrzał odbity do boku przez " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " i niebezpieczeństwo zażegnane.";
                break;
        }
    }
    public static void HeaderCorner()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nCoż za interwencja " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ", rzut rożny.";
                break;
            case 2:
                t.text += "\nNiezbyt dobry strzał " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ", ale " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " z problemami wybija piłkę na rzut rożny.";
                break;
            case 3:
                t.text += "\nDoskonała obrona " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + " ratuje " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " i mamy korner.";
                break;
        }
    }
    public static void HeaderGoal()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        string text = "";
        switch (rnd)
        {
            case 1:
                text = "Gooooool, " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " bez szans";
                break;
            case 2:
                text = "Świetny strzał " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ". Piłka w okienku bramki " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
            case 3:
                text = "Bramka dla " + host_guest_string[Comment.Instance.GetGuestBall()].getGenitive() + " po strzale głową.";
                break;
        }
        t.text += "\n<color=#ffa500ff>" + Comment.Instance.GetMinute() + " min. " + text + "</color>";
    }
    public static void HeaderMiss()
    {
        
        int rnd = Random.Range(10, 50);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nBardzo blisko, ale jednak obok bramki strzeżonej przez " + teams[Comment.Instance.GetReverseIsGuestBall()][0].AlteredSurname + ".";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " odprowadza piłkę wzrokiem. Zmarnowana dobra okazja na zdobycie bramki.";
                break;
            case 3:
                t.text += "\nWysoko nad bramką, bardzo słaba główka. Takie sytuacje mogą się zemścić.";
                break;
            case 4:
                t.text += "\nPoprzeczka ratuje " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive();
                break;
        }
    }
    public static void CounterAttackShotTry()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " przebiega kilkanaście metrów i strzela...";
                break;
            case 2:
                t.text += "\nPiłkę przy nodze ma " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " i decyduje się na uderzenie...";
                break;
            case 3:
                t.text += "\nPrzy piłce " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ", widzi wysuniętego bramkarza...";
                break;
        }
    }
    public static void CounterAttackFailedPass()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nFatalne podanie " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + " i koniec kontrataku.";
                break;
            case 2:
                t.text += "\nPogubił się " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " podaje wprost do przeciwnika.";
                break;
            case 3:
                t.text += "\nPodaje " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ", ale piłka przechwycona przez obronę.";
                break;
        }
    }
    public static void CounterAttackPenaltyFoul()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nAle zostaje podcięty przez obrońcę i mamy jedenastkę.";
                break;
            case 2:
                t.text += "\nW ostatniej chwili obrońca wchodzi na wślizgu w nogi atakującego i mamy rzut karny.";
                break;
            case 3:
                t.text += "\nI zostaje nieprzepisowo zatrzymany przez nadbiegającego obrońcę, karny.";
                break;
        }
    }
    public static void CounterAttackPreShot()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                t.text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ", ten ma przed sobą tylko bramkarza.";
                break;
            case 2:
                t.text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ", ten jest sam na sam z bramkarzem.";
                break;
            case 3:
                t.text += teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].AlteredSurname + ", który jest teraz na czystej pozycji.";
                break;
        }
    }
    public static void CounterAttackSuccessPass()
    {
        int rnd = Random.Range(10, 30);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " podaje do ";
                break;
            case 2:
                t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " zgrywa do ";
                break;
        }
    }
    public static void CornerExecution(Footballer f)
    {
        t.text += "\n" + Comment.Instance.GetMinute() + " min. Stały fragment gry wykonywać będzie " + f.Surname + ". Dośrodkowanie...";
    }
    public static void FreeHeader()
    {
        t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " urywa się spod opieki obrońcy i strzela...";
    }
    public static void ContestedHeader()
    {
        t.text += "\n" + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " wygrywa pojedynek główkowy...";
    }
    public static void DefenderWinsHeader(Footballer defender)
    {
        t.text += "\nDobre dośrodkowanie, ale " + defender.Surname + " wygrywa pojedynek główkowy.";
    }
    public static void FailedCross()
    {
        int rnd = Random.Range(10, 40);
        rnd /= 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nZłe było to dośrodkowanie i obrońca wybija bez problemu.";
                break;
            case 2:
                t.text += "\nNieźle wykonane, ale " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " łapie piłkę.";
                break;
            case 3:
                t.text += "\nZa dużo siły w tym dograniu i aut dla " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + ".";
                break;
        }
    }
    public static void TryingToDodge()
    {
        
        t.text += "\n" + Comment.Instance.GetMinute() + " min. Piłkę przy nodze ma " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ". próbuje minąć przeciwnika...";
    }
    public static void DecidesToCross()
    {
        
        t.text += "\nMinął rywala i dośrodkowuje...";
    }
    public static void DecidesToShootInsteadOfCrossing()
    {
        
        t.text += "\nOszukał obrońcę i decyduje się na strzał...";
    }
    public static void FailedWingDribble(int direction)
    {
        
        t.text += "\nSprytna próba, ale " + teams[Comment.Instance.GetReverseIsGuestBall()][defWingPos[Comment.Instance.GetReverseIsGuestBall(), direction]].Surname + " nie dał się nabrać i zabrał piłkę.";
    }
    public static void FailedMidDribble(Footballer defender)
    {
        
        t.text += "\nSprytna próba, ale " + defender.Surname + " nie dał się nabrać i zabrał piłkę.";
    }
    public static void DecidesToShootInsteadOfPassing()
    {
        
        t.text += "\nOminął obrońcę i decyduje się na strzał...";
    }
    public static void DecidesToPass()
    {
        
        t.text += "\nOminął obrońcę i podaje do lepiej ustawionego partnera...";
    }
    public static void FailedPass()
    {
        
        int rnd = Random.Range(10, 40);
        rnd = rnd / 10;
        switch (rnd)
        {
            case 1:
                t.text += "\nZłe było to podanie i obrońca przechwytuje bez problemu.";
                break;
            case 2:
                t.text += "\nNieźle wykonane, ale trochę za mocno i " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " łapie piłkę.";
                break;
            case 3:
                t.text += "\nZa dużo siły w tym dograniu i " + host_guest_string[Comment.Instance.GetReverseIsGuestBall()].getGenitive() + " zaczną od bramki.";
                break;
        }
    }
    public static void ChanceForOneOnOne()
    {
        t.text += "\nTam jest " + teams[Comment.Instance.GetGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + ", odwraca się, a przed nim tylko jeden obrońca...";
    }
    public static void FailedChanceOneToOne()
    {
        t.text += "\n" + teams[Comment.Instance.GetReverseIsGuestBall()][Comment.Instance.GetPlayerWithBall()].Surname + " wyłuskuje piłkę napastnikowi i wybija ją.";
    }
    public static void OneToOneSituation()
    {
        t.text += "\nMija go, jeszcze tylko " + teams[Comment.Instance.GetReverseIsGuestBall()][0].Surname + " może zapobiec utracie bramki...";
    }
    public static void UpdateResult(MatchStats[] matchStats)
    {
        resultText.text = hostName + "  " + matchStats[0].GetGoals() + " - " + matchStats[1].GetGoals() + "  " + guestName;
    }
}
