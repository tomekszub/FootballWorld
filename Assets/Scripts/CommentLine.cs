using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class CommentLine : MonoBehaviour
{
    readonly Declination[] _host_guest_string = {new Declination("gospodarze", "gospodarzy", "gospodarzom", "gospodarzy", "gospodarzami", "gospodarzach"),
                                              new Declination("goście", "gości", "gościom", "gości", "gośćmi", "gościach")};
    readonly Declination[] _left_right_wing_string = {new Declination("lewe", "lewego", "lewemu", "lewe", "lewym", "lewym"),
                                              new Declination("prawe", "prawego", "prawemu", "prawe", "prawym", "prawym")};
    readonly string[] _left_right_direction_string = { "lewo","prawo"};

    public static CommentLine Instance;

    [SerializeField] TextMeshProUGUI _ResultText;
    [SerializeField] TextMeshProUGUI _Commentary;

    string _guestName, _hostName;
    int _guestId, _hostId;
    PitchTeam[] _teams = new PitchTeam[2];

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void UpdateTeams()
    {
        _teams = Comment.Instance.GetTeams();
    }

    public void StartingSettings()
    {
        _Commentary.text = $"";
        _guestName = Comment.Instance.GetGuestName();
        _hostName = Comment.Instance.GetHostName();
        _ResultText.text = $"{_hostName} 0 - 0 {_guestName}";
        _guestId = Comment.Instance.GetGuestID();
        _hostId = Comment.Instance.GetHostID();
        UpdateTeams();
    }

    public void EndOfTheMatch()
    {
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. Sędzia gwiżdże po raz ostatni. Koniec meczu";
    }

    public void StartOfTheMatch()
    {
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. Pierwszy gwizdek sędziego i { _guestName} może rozpocząć grę.";
    }

    public void BoringPartOfTheMatch()
    {
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. Bardzo słaba część spotkania, piłka ciagle znajduje się w środku boiska.";
    }

    public void StartingComment()
    {
        int arb = Random.Range(0, 5);
        string text;
        switch (GetRandomIndex(10))
        {
            case 1: text = $"\nWitam, dzisiaj przed nami bardzo ciekawe widowisko. Mecz: {_hostName} - {_guestName}."; break;
            case 2: text = $"\nWitam z {Database.clubDB[_hostId].Stadium}, dziś przed nami bardzo ciekawy mecz: {_hostName} - {_guestName}."; break;
            case 3: text = $"\nDeszczowy wieczór i spotkanie rozgrywane na {Database.clubDB[_hostId].Stadium}. Jak zwykle liczymy na emocje."; break;
            case 4: text = $"\nPiłkarze wychodzą na boisko, skupieni czekają na pierwszy gwizdek arbitra."; break;
            case 5: text = $"\nArbitrem dzisiejszego spotkania jest {Database.arbiterDB[0, arb]}-letni {Database.arbiterDB[1, arb]}."; break;
            default: text = $"\nDzisiaj {_hostName} w składzie: {_teams[0][0].Surname}, {_teams[0][1].Surname}, {_teams[0][2].Surname}, {_teams[0][3].Surname}, {_teams[0][4].Surname}, " +
                    $"{_teams[0][5].Surname}, {_teams[0][6].Surname}, {_teams[0][7].Surname}, {_teams[0][8].Surname}, {_teams[0][9].Surname}, {_teams[0][10].Surname}." +
                    $"\n{_guestName} w składzie: {_teams[1][0].Surname}, {_teams[1][1].Surname}, {_teams[1][2].Surname}, {_teams[1][3].Surname}, {_teams[1][4].Surname}, " +
                    $"{_teams[1][5].Surname}, {_teams[1][6].Surname}, {_teams[1][7].Surname}, {_teams[1][8].Surname}, {_teams[1][9].Surname}, {_teams[1][10].Surname}."; break;
        }
        _Commentary.text += text;
    }

    public void InfoComment()
    {
        MatchStats[] ms = Comment.Instance.GetMatchStats();
        int hostShots = ms[0].Shots;
        int guestShots = ms[1].Shots;
        int hostChances = Comment.Instance.GetHostChances();
        int hostGoals = ms[0].Goals;
        int guestGoals = ms[1].Goals;
        string text = "";
        int attendence = Random.Range((Database.clubDB[_guestId].Rate / 10) * Database.clubDB[_hostId].StadiumCapacity, Database.clubDB[_hostId].StadiumCapacity);
        switch (GetRandomIndex(8))
        {
            case 1: text = $"Gra toczy się aktualnie na środku boiska."; break;
            case 2: text = $"Żadna z drużyn nie ma pomysłu na przebicie się przez obronę przeciwnika."; break;
            case 3: text = $"Na {Database.clubDB[_hostId].Stadium} zasiada dzisiaj {attendence}."; break;
            case 4: text = $"Nic się teraz nie dzieje na boisku."; break;
            case 5: text = $"Statystyka strzałów prezentuje się następująco: {_hostName} - {hostShots}, {_guestName} - {guestShots}."; break;
            case 6:
                int pos = Random.Range(50, 70);
                text = (hostChances >= 50 && hostGoals == guestGoals) || hostGoals > guestGoals
                    ? $"Posiadanie piłki: {_hostName} - {pos}, {_guestName} - {100 - pos}."
                    : $"Posiadanie piłki: {_guestName} - {pos}, {_hostName} - {100 - pos}.";
                break;
            case 7:
                text = (hostChances >= 50 && hostGoals == guestGoals) || hostGoals > guestGoals
                    ? $"{_hostName} kontroluje wydarzenia na boisku."
                    : $"{_guestName} kontroluje wydarzenia na boisku.";
                break;
            case 8:
                text = (hostChances >= 50 && hostGoals == guestGoals) || hostGoals < guestGoals
                    ? $"{_hostName} stosuje wysoki pressing na połowie przeciwnika."
                    : $"{_guestName} stosuje wysoki pressing na połowie przeciwnika.";
                break;
        }
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. {text}";
    }

    public void AttackFirstPhase()
    {
        string text = "";
        switch (GetRandomIndex(6))
        {
            case 1: text = $"Akcje {_host_guest_string[Comment.Instance.GuestBall].getGenitive()} długim podaniem rozpoczyna {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}.";break;
            case 2: text = $"Przy piłce {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}, przyjmuje piłkę i podaje.";break;
            case 3: text = $"{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} dostaje piłkę i odgrywa ją do partnera.";break;
            case 4: text = $"{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} podaje piłkę do kolegi.";break;
            case 5: text = $"{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} w posiadaniu piłki, szuka kolegi z zespołu i podaje.";break;
            case 6: text = $"Teraz {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}, od razu podaje do przodu.";break;
        }

        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. {text}";
    }

    public void InterceptionAndCounter()
    {
        _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} przechwytuje piłkę i {_host_guest_string[Comment.Instance.GuestBall].getNominative()} wychodzą z kontratakiem.";
    }

    public void PassMiddle()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nPiłka trafia do {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}. Ten dogrywa do lepiej ustawionego partnera.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} dostał piłkę i podaje ją do przodu.";
                break;
            case 3:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} po otrzymaniu futbolówki niezwłocznie oddaje ją do nieatakowanego partnera.";
                break;
        }
    }

    public void PassToTheWing(int isRightWing)
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nPiłka trafia do {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}. Zagranie na {_left_right_wing_string[isRightWing].getNominative()} skrzydło.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} podaje na skrzydło.";
                break;
            case 3:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} po otrzymaniu piłki bez zastanowienia posyła ją na {_left_right_direction_string[isRightWing]}.";
                break;
        }
    }

    public void DecidesToShoot()
    {
        _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} decyduje się na uderzenie z daleka...";
    }

    public void PreparingToPenalty()
    {
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. Do piłki podchodzi {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}...";
    }

    public void PenaltyGoal()
    {
        string text = "";
        switch (GetRandomIndex(3))
        {
            case 1:
                text = $"Gol, pewnie wykorzystany karny.";
                break;
            case 2:
                text = $"Fantastyczny strzał, {_teams[Comment.Instance.ReverseGuestBall][0].Surname} bez szans.";
                break;
            case 3:
                text = $"Mocno, w dolny róg bramki i gol dla {_host_guest_string[Comment.Instance.GuestBall].getGenitive()}.";
                break;
        }
        _Commentary.text += $"\n<color=#ffa500ff>{Comment.Instance.GetMinute()} min. {text}</color>";
    }

    public void PenaltyMissed()
    {
        string text = $"";
        switch (GetRandomIndex(3))
        {
            case 1:
                text = $"{_teams[Comment.Instance.ReverseGuestBall][0].Surname} wyczuł intencje strzelajacego i łapie piłkę.";
                break;
            case 2:
                text = $"Za słaby strzał, {_teams[Comment.Instance.ReverseGuestBall][0].Surname} odbija bez problemu.";
                break;
            case 3:
                text = $"Mocno, nad bramką i nadal wynik bez zmian.";
                break;
        }
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. {text}";
    }

    public void LongShotCorner()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nStrzela {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}, ale piłka po rykoszecie wypada za boisko, Rzut rożny.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}, dobry strzał i dobra obrona bramkarza, wybija piłkę na rzut rożny.";
                break;
            case 3:
                _Commentary.text += $"\nStrzał {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname} zablokowany przez jednego z graczy {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()} i mamy korner.";
                break;
        }
    }

    public void LongShotGoal()
    {
        string text = $"";
        switch (GetRandomIndex(3))
        {
            case 1:
                text = $"Gooooool, cudowny strzał {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}.";
                break;
            case 2:
                text = $"Piękny gol {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}. Piłka w oknie bramki {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()}.";
                break;
            case 3:
                text = $"Przepiękne uderzenie {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname} i gol.";
                break;
        }
        _Commentary.text += $"\n<color=#ffa500ff>{Comment.Instance.GetMinute()} min. {text}</color>";
    }

    public void LongShotMiss()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nAle bardzo nieudana próba.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} strzela z daleka ale niecelnie.";
                break;
            case 3:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} strzela z dystansu ale piłka ląduje poza boiskiem.";
                break;
        }
    }

    public void CounterAttackSave()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname}, z problemami ale łapie piłkę.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} wyłapuje to niegroźne uderzenie.";
                break;
            case 3:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} decyduje się złapać piłkę zamiast wybić ją do boku i udaje mu się to.";
                break;
        }
    }

    public void CounterAttackCorner()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nCoż za interwencja {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname}, rzut rożny.";
                break;
            case 2:
                _Commentary.text += $"\nDobry strzał i bardzo dobra obrona bramkarza, mamy korner. To powinna być bramka.";
                break;
            case 3:
                _Commentary.text += $"\nŚwietna obrona {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname}, ratuje {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()} i mamy korner.";
                break;
        }
    }

    public void CounterAttackGoal()
    {
        string text = $"";
        switch (GetRandomIndex(3))
        {
            case 1:
                text = $"Gooooool, piękne uderzenie {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}.";
                break;
            case 2:
                text = $"Piękny gol {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}. Piłka w samym okienku bramki {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()}.";
                break;
            case 3:
                text = $"Kapitalne uderzenie, po ziemi {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname} i gol.";
                break;
        }
        _Commentary.text += $"\n<color=#ffa500ff>{Comment.Instance.GetMinute()} min. {text}</color>";
    }

    public void CounterAttackMiss()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} odprowadza piłkę wzrokiem. Niecelny strzał.";
                break;
            case 3:
                _Commentary.text += $"\nWysoko nad bramką, bardzo słaby strzał. Dlaczego nie podawał?";
                break;
        }
    }

    public void NormalAttackSave()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname}, z problemami ale łapie piłkę.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} wyłapuje to niegroźne uderzenie.";
                break;
            case 3:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} decyduje się złapać piłkę zamiast wybić ją do boku i udaje mu się to.";
                break;
        }
    }

    public void NormalAttackCorner()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nCoż za interwencja {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname}, rzut rożny.";
                break;
            case 2:
                _Commentary.text += $"\nDobry strzał ale bardzo dobra parada {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname}, mamy korner. To powinna być bramka.";
                break;
            case 3:
                _Commentary.text += $"\nDoskonała obrona {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname} ratuje {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()} i mamy korner.";
                break;
        }
    }

    public void NormalAttackGoal()
    {
        string text = $"";
        switch (GetRandomIndex(3))
        {
            case 1:
                text = $"Gooooool, piękne uderzenie {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}.";
                break;
            case 2:
                text = $"Piękny gol {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}. Piłka w samym okienku bramki {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()}.";
                break;
            case 3:
                text = $"Kapitalne uderzenie, po ziemi {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname} i gol.";
                break;
        }
        _Commentary.text += $"\n<color=#ffa500ff>{Comment.Instance.GetMinute()} min. {text}</color>";
    }

    public void NormalAttackMiss()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} odprowadza piłkę wzrokiem. Niecelny strzał.";
                break;
            case 3:
                _Commentary.text += $"\nWysoko nad bramką, bardzo słaby strzał. Dlaczego nie podawał?";
                break;
        }
    }

    public void OneOnOneAttackSave()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname}, wyłuskuje piłkę spod nóg przeciwnika.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} próbuje ominąć {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname} ale nie udaje mu się to i piłka w rękach bramkarza.";
                break;
            case 3:
                _Commentary.text += $"\nStrzał odbity do boku przez {_teams[Comment.Instance.ReverseGuestBall][0].Surname} i niebezpieczeństwo zażegnane.";
                break;
        }
    }

    public void OneOnOneAttackCorner()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nCoż za interwencja {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname}, rzut rożny.";
                break;
            case 2:
                _Commentary.text += $"\nSłaby strzał i dobrze spisał się {_teams[Comment.Instance.ReverseGuestBall][0].Surname}, {_host_guest_string[Comment.Instance.GuestBall].getNominative()} wznowią grę z rogu boiska. To powinno zakoćzyć się zmianą wyniku...";
                break;
            case 3:
                _Commentary.text += $"\nDoskonała obrona {_teams[Comment.Instance.ReverseGuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}, ratuje {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()} i mamy korner.";
                break;
        }
    }

    public void OneOnOneAttackGoal()
    {
        string text = $"";
        switch (GetRandomIndex(3))
        {
            case 1:
                text = $"Gooooool, {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} mija bramkarza i pakuje piłkę do pustej bramki.";
                break;
            case 2:
                text = $"Cudowny gol {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}. Piłka w samym okienku bramki {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()}.";
                break;
            case 3:
                text = $"Czysto uderza po ziemi {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} i gol.";
                break;
        }
        _Commentary.text += $"\n<color=#ffa500ff>{Comment.Instance.GetMinute()} min. {text}</color>";
    }

    public void OneOnOneAttackMiss()
    {
        switch (GetRandomIndex(4))
        {
            case 1:
                _Commentary.text += $"\nAleż pudło, dobrych kilka metrów obok bramki.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} odprowadza piłkę wzrokiem. Zmarnowana 100 % okazja.";
                break;
            case 3:
                _Commentary.text += $"\nWysoko nad bramką, bardzo słaby strzał. Takie sytuacje trzeba wykorzystywać.";
                break;
            case 4:
                _Commentary.text += $"\nStrzał i słupek ratuje {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()}";
                break;
        }
    }

    public void HeaderSave()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} bez trudu łapie piłkę.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} prosto w ręce {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname}.";
                break;
            case 3:
                _Commentary.text += $"\nStrzał odbity do boku przez {_teams[Comment.Instance.ReverseGuestBall][0].Surname} i niebezpieczeństwo zażegnane.";
                break;
        }
    }

    public void HeaderCorner()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nCoż za interwencja {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname}, rzut rożny.";
                break;
            case 2:
                _Commentary.text += $"\nNiezbyt dobry strzał {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}, ale {_teams[Comment.Instance.ReverseGuestBall][0].Surname} z problemami wybija piłkę na rzut rożny.";
                break;
            case 3:
                _Commentary.text += $"\nDoskonała obrona {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname} ratuje {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()} i mamy korner.";
                break;
        }
    }

    public void HeaderGoal()
    {
        string text = $"";
        switch (GetRandomIndex(3))
        {
            case 1:
                text = $"Gooooool, {_teams[Comment.Instance.ReverseGuestBall][0].Surname} bez szans";
                break;
            case 2:
                text = $"Świetny strzał {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}. Piłka w okienku bramki {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()}.";
                break;
            case 3:
                text = $"Bramka dla {_host_guest_string[Comment.Instance.GuestBall].getGenitive()} po strzale głową.";
                break;
        }
        _Commentary.text += $"\n<color=#ffa500ff>{Comment.Instance.GetMinute()} min. {text}</color>";
    }

    public void HeaderMiss()
    {
        switch (GetRandomIndex(4))
        {
            case 1:
                _Commentary.text += $"\nBardzo blisko, ale jednak obok bramki strzeżonej przez {_teams[Comment.Instance.ReverseGuestBall][0].AlteredSurname}.";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.ReverseGuestBall][0].Surname} odprowadza piłkę wzrokiem. Zmarnowana dobra okazja na zdobycie bramki.";
                break;
            case 3:
                _Commentary.text += $"\nWysoko nad bramką, bardzo słaba główka. Takie sytuacje mogą się zemścić.";
                break;
            case 4:
                _Commentary.text += $"\nPoprzeczka ratuje {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()}";
                break;
        }
    }

    public void CounterAttackShotTry()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} przebiega kilkanaście metrów i strzela...";
                break;
            case 2:
                _Commentary.text += $"\nPiłkę przy nodze ma {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} i decyduje się na uderzenie...";
                break;
            case 3:
                _Commentary.text += $"\nPrzy piłce {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}, widzi wysuniętego bramkarza...";
                break;
        }
    }

    public void CounterAttackFailedPass()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nFatalne podanie {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname} i koniec kontrataku.";
                break;
            case 2:
                _Commentary.text += $"\nPogubił się {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} podaje wprost do przeciwnika.";
                break;
            case 3:
                _Commentary.text += $"\nPodaje {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}, ale piłka przechwycona przez obronę.";
                break;
        }
    }

    public void CounterAttackPenaltyFoul()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nAle zostaje podcięty przez obrońcę i mamy jedenastkę.";
                break;
            case 2:
                _Commentary.text += $"\nW ostatniej chwili obrońca wchodzi na wślizgu w nogi atakującego i mamy rzut karny.";
                break;
            case 3:
                _Commentary.text += $"\nI zostaje nieprzepisowo zatrzymany przez nadbiegającego obrońcę, karny.";
                break;
        }
    }

    public void CounterAttackPreShot()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}, ten ma przed sobą tylko bramkarza.";
                break;
            case 2:
                _Commentary.text += $"{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}, ten jest sam na sam z bramkarzem.";
                break;
            case 3:
                _Commentary.text += $"{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].AlteredSurname}, który jest teraz na czystej pozycji.";
                break;
        }
    }

    public void CounterAttackSuccessPass()
    {
        switch (GetRandomIndex(2))
        {
            case 1:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} podaje do ";
                break;
            case 2:
                _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} zgrywa do ";
                break;
        }
    }

    public void CornerExecution(Footballer f)
    {
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. Stały fragment gry wykonywać będzie {f.Surname}. Dośrodkowanie...";
    }

    public void FreeHeader()
    {
        _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} urywa się spod opieki obrońcy i strzela...";
    }

    public void ContestedHeader()
    {
        _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} wygrywa pojedynek główkowy...";
    }

    public void DefenderWinsHeader(Footballer defender)
    {
        _Commentary.text += $"\nDobre dośrodkowanie, ale {defender.Surname} wygrywa pojedynek główkowy.";
    }

    public void FailedCross()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nZłe było to dośrodkowanie i obrońca wybija bez problemu.";
                break;
            case 2:
                _Commentary.text += $"\nNieźle wykonane, ale {_teams[Comment.Instance.ReverseGuestBall][0].Surname} łapie piłkę.";
                break;
            case 3:
                _Commentary.text += $"\nZa dużo siły w tym dograniu i aut dla {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()}.";
                break;
        }
    }

    public void TryingToGetPastDefender()
    {
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. Piłkę przy nodze ma {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}. próbuje minąć przeciwnika...";
    }

    public void MiddleEmptySpace()
    {
        _Commentary.text += $"\n{Comment.Instance.GetMinute()} min. {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} przy piłce, zastaje nieprawdopodobnie pustą przestrzeń przed sobą. Przed nim tylko bramkarz...";
    }

    public void DecidesToCross()
    {
        _Commentary.text += $"\nMinął rywala i dośrodkowuje...";
    }

    public void DecidesToShootInsteadOfCrossing()
    {
        _Commentary.text += $"\nOszukał obrońcę i decyduje się na strzał...";
    }

    public void FailedWingDribble(int direction)
    {
        _Commentary.text += $"\nSprytna próba, ale {_teams[Comment.Instance.ReverseGuestBall][_teams[Comment.Instance.ReverseGuestBall].GetIndexOfDefensiveWinger(direction)].Surname} nie dał się nabrać i zabrał piłkę.";
    }

    public void FailedMidDribble(Footballer defender)
    {
        _Commentary.text += $"\nSprytna próba, ale {defender.Surname} nie dał się nabrać i zabrał piłkę.";
    }

    public void DecidesToShootInsteadOfPassing()
    {
        _Commentary.text += $"\nOminął obrońcę i decyduje się na strzał...";
    }

    public void DecidesToPass()
    {
        _Commentary.text += $"\nOminął obrońcę i podaje do lepiej ustawionego partnera...";
    }

    public void FailedPass()
    {
        switch (GetRandomIndex(3))
        {
            case 1:
                _Commentary.text += $"\nZłe było to podanie i obrońca przechwytuje bez problemu.";
                break;
            case 2:
                _Commentary.text += $"\nNieźle wykonane, ale trochę za mocno i {_teams[Comment.Instance.ReverseGuestBall][0].Surname} łapie piłkę.";
                break;
            case 3:
                _Commentary.text += $"\nZa dużo siły w tym dograniu i {_host_guest_string[Comment.Instance.ReverseGuestBall].getGenitive()} zaczną od bramki.";
                break;
        }
    }

    public void ChanceForOneOnOne()
    {
        _Commentary.text += $"\nTam jest {_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname}, odwraca się, a przed nim tylko jeden obrońca...";
    }

    public void FailedChanceOneToOne()
    {
        _Commentary.text += $"\n{_teams[Comment.Instance.GuestBall][Comment.Instance.PlayerWithBall].Surname} wyłuskuje piłkę napastnikowi i wybija ją.";
    }

    public void OneToOneSituation()
    {
        _Commentary.text += $"\nMija go, jeszcze tylko {_teams[Comment.Instance.ReverseGuestBall][0].Surname} może zapobiec utracie bramki...";
    }

    public void UpdateResult(MatchStats[] matchStats)
    {
        _ResultText.text = $"{_hostName} {matchStats[0].Goals} - {matchStats[1].Goals} {_guestName}";
    }

    int GetRandomIndex(int countOfIndices)
    {
        int rnd = Random.Range(10, (countOfIndices + 1) * 10);
        return rnd / 10;
    }
}
