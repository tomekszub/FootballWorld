using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class Comment : MonoBehaviour
{
    enum Direction
    {
        Left,
        Right,
        Middle
    }

    const int SECOND_PHASE_SAFE_INDEX = 37;

    public static Comment Instance;

    [SerializeField] TextMeshProUGUI _StartStopButton;
    [SerializeField] List<CommentPanelFootballerRow> _HostSquadStatsUI;
    [SerializeField] List<CommentPanelFootballerRow> _GuestSquadStatsUI;

    float _time = 0.1f;
    bool _isPlaying = false;
    int _minute = 0;
    int _hostId, _guestId;
    PitchTeam[] _teams = new PitchTeam[2];
    List<PlayersMatchData>[] _teamsMatchData = new List<PlayersMatchData>[2];
    int _hostChances, _goalChances;
    MatchStats[] _matchStats = new MatchStats[2];
    string[] _teamName = new string[2];
    int _playerWithBall;  // piłkarz przy piłce
    int _prevPlayerWithBall;
    int _not = 0;
    float _curiosity = 1;
    int _weakness = 0;
    int _changedCuriosity = 0;
    bool _end = false;
    string _competitionName = "";

    #region Getters
    public string GetGuestName() => _teamName[1];
    public string GetHostName() => _teamName[0];
    public int GetGuestID() => _guestId;
    public int GetHostID() => _hostId;
    public int GetMinute() => _minute;
    public PitchTeam[] GetTeams() => _teams;
    public MatchStats[] GetMatchStats() => _matchStats;
    public int GetHostChances() => _hostChances;
    public int GuestBall 
    {
        private set;
        get;
    }
    public int PlayerWithBall
    {
        get => _playerWithBall;
        private set
        {
            _prevPlayerWithBall = _playerWithBall;
            _playerWithBall = value;
        }
    }
    #endregion

    public int ReverseGuestBall => (GuestBall == 0) ? 1 : 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Use this for initialization, 
    public void Init(int hostID, int guestID, string compName)
    {
        _competitionName = compName;
        _hostId = hostID;
        _guestId = guestID;
        _time = 0.1f;
        PrepareNextMatch();
    }

    public void OnSimSpeedChanged(float val) => _time = val;

    public void SimulateMatch()
    {
        if(!_end)
        {
            _time = 0;
            if(!_isPlaying)
                StartCoroutine(CommentStart());
        }
    }

    void PrepareNextMatch()
    {
        _end = false;
        _matchStats[0] = new MatchStats(new List<Scorer>());
        _matchStats[1] = new MatchStats(new List<Scorer>());
        _teamName[0] = Database.clubDB[_hostId].Name;
        _teamName[1] = Database.clubDB[_guestId].Name;
       
        _teams[0] = new(Database.GetFootballersFromClub(_hostId), _competitionName, Database.clubDB[_hostId].Formation);
        _teams[1] = new(Database.GetFootballersFromClub(_guestId), _competitionName, Database.clubDB[_guestId].Formation);
        _teamsMatchData[0] = new(11);
        _teamsMatchData[1] = new(11);
        _teams[0].ReportAllPlayersStartedMatch();
        _teams[1].ReportAllPlayersStartedMatch();
        for (int i = 0; i < 11; i++)
        {
            _teamsMatchData[0].Add(new PlayersMatchData());
            _HostSquadStatsUI[i].Init(_teams[0][i], _teamsMatchData[0][i]);
            _teamsMatchData[1].Add(new PlayersMatchData());
            _GuestSquadStatsUI[i].Init(_teams[1][i], _teamsMatchData[1][i]);
        }
        Clear();
        CalculateWeaknessOfTheDay();
        _isPlaying = false;
        CommentLine.Instance.StartingSettings();
        _StartStopButton.text = "Rozpocznij mecz";
    }

    void FinishMatch()
    {
        for (int i = 0; i < 11; i++)
        {
            _teams[0].ReportPlayerMatchRating(i, _teamsMatchData[0][i].MatchRating);
            _teams[1].ReportPlayerMatchRating(i, _teamsMatchData[1][i].MatchRating);
        }

        if (_matchStats[1].Goals == 0)
            _teams[0].ReportAllPlayersCleanSheet();
        if (_matchStats[0].Goals == 0)
            _teams[1].ReportAllPlayersCleanSheet();
    }

    public void StartStopButtonClick()
    {
        if (_end)
        {
            MyClub.Instance.ProcesssMatchStats(_matchStats, _competitionName);
            WindowsManager.Instance.ShowWindow("Club");
            return;
        }

        if (!_isPlaying)
        {
            StartCoroutine(CommentStart());
        }
    }

    IEnumerator CommentStart()
    {
        if (_end)
            yield break;

        UpdateChances();
        CommentLine.Instance.UpdateResult(_matchStats);
        if (_minute >= 90)
        {
            if(_time > 0)
                yield return new WaitForSeconds(_time);

            StopAllCoroutines();
            _end = true;
            CommentLine.Instance.EndOfTheMatch();
            FinishMatch();
            _StartStopButton.text = "Zakończ mecz";
            Debug.Log("KONIEC MECZU!!!---Wynik: " + _matchStats[0].Goals + " - " + _matchStats[1].Goals);
            yield break;
        }

        _isPlaying = true;

        if (_minute == 0)
        {
            CommentLine.Instance.StartingComment();
            MinutePassed();
            if(_time > 0) yield return new WaitForSeconds(_time);
            CommentLine.Instance.StartOfTheMatch();
            MinutePassed();
        }
        if (_minute < 90 && _minute > 1)
        {
            int ch = Random.Range(1, 100);
            if (ch >= 90)
            {
                _not = 0;
                if(_time > 0) yield return new WaitForSeconds(_time);
                CommentLine.Instance.InfoComment();
                MinutePassed();
                StartCoroutine(CommentStart());
                yield break;
            }
            if (ch > 5 + _goalChances && ch < 90)
            {
                _not++;
                if (_not > 8)
                {
                    if(_time > 0) yield return new WaitForSeconds(_time);
                    CommentLine.Instance.BoringPartOfTheMatch();
                    _not = 0;
                }
                MinutePassed();
                StartCoroutine(CommentStart());
                yield break;
            }
            if (ch <= 5 + _goalChances)
            {
                _not = 0;
                if(_time > 0) yield return new WaitForSeconds(_time);
                AttackFirstPhase();
                if(_time > 0) yield return new WaitForSeconds(_time);
                AttackSecondPhase();
            }
        }
    }

    void AttackFirstPhase()
    {
        GuestBall = Random.Range(1, 100) <= _hostChances ? 0 : 1;

        var index = _teams[GuestBall].GetIndexOfRandomDefender();
        PlayerWithBall = index == -1 ? 0 : index;
        CommentLine.Instance.AttackFirstPhase();
    }

    void AttackSecondPhase()
    {
        int counterPos = _teams[ReverseGuestBall].GetIndexOfRandomAttacker(); ;
        float counterChance = _teams[ReverseGuestBall][counterPos].Tackle - _teams[GuestBall][PlayerWithBall].Pass;
        counterChance = (counterChance + 100) / 20;
        int dec = Random.Range(1, SECOND_PHASE_SAFE_INDEX + (int)counterChance);

        UpdateMatchRatingForCurrentPlayer(dec > SECOND_PHASE_SAFE_INDEX ? MatchRatingConstants.COUNTER_BALL_LOSS : MatchRatingConstants.BASIC_PASS_SUCCESS);

        // TODO: Add decision making based on some attributes
        if (dec <= 34)
        {
            List<Direction> directions = new() { Direction.Left, Direction.Right, Direction.Middle };
            directions = directions.OrderBy(x => Random.Range(0, 100)).ToList();
            int midfielderIndex = _teams[GuestBall].GetIndexOfRandomMidfielder();

            if (midfielderIndex != -1)
            {
                do
                {
                    if (directions[0] == Direction.Middle)
                    {
                        // środek podanie TODO: potential duplicated player
                        PlayerWithBall = midfielderIndex;
                        CommentLine.Instance.PassMiddle();
                        UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.BASIC_PASS_SUCCESS);
                        PlayerWithBall = _teams[GuestBall].GetIndexOfRandomMiddlePlayer();
                        StartCoroutine(AttackThirdPhase(Direction.Middle));
                        return;
                    }
                    else
                    {
                        var wingerIndex = _teams[GuestBall].GetIndexOfWinger((int)directions[0]);

                        if (wingerIndex == -1 || wingerIndex == midfielderIndex)
                            wingerIndex = _teams[GuestBall].GetIndexOfDefensiveWinger((int)directions[0]);

                        if (wingerIndex != -1)
                        {
                            PlayerWithBall = midfielderIndex;

                            CommentLine.Instance.PassToTheWing((int)directions[0]);

                            UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.BASIC_PASS_SUCCESS);

                            PlayerWithBall = wingerIndex;

                            StartCoroutine(AttackThirdPhase(directions[0]));
                            return;
                        }
                    }

                    directions.RemoveAt(0);
                }
                while (directions.Count > 0);
            }

            if (Random.Range(0, 100) > 70)
            {
                BallLost();
                return;
            }

            MiddlePassAndShot();
        }
        else if (dec <= 37)
            MiddlePassAndShot();
        else
            BallLost();

        void BallLost()
        {
            // Kontratak
            PlayerWithBall = counterPos;
            GuestBall = ReverseGuestBall;
            CommentLine.Instance.InterceptionAndCounter();

            UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.COUNTER_BALL_INTERCEPTION);

            StartCoroutine(CounterAttack());
        }

        void MiddlePassAndShot()
        {
            var playerIndex = _teams[GuestBall].GetIndexOfRandomMidfielder();
            if (playerIndex == -1)
                BallLost();

            PlayerWithBall = playerIndex;
            // Strzał
            CommentLine.Instance.DecidesToShoot();
            StartCoroutine(Shot(10));
        }
    }

    void UpdateChances()
    {
        _hostChances = 50;
        // difference between host's attack and guest's defense
        float advantage = ((_teams[0].TeamAttackRating - _teams[1].TeamDefenceRating) / 10) / 3;
        advantage += ((_teams[0].TeamDefenceRating - _teams[1].TeamAttackRating) / 10) / 3;
        advantage += ((_teams[0].TeamMidfieldRating - _teams[1].TeamMidfieldRating) / 10) / 3;
        _hostChances += 4 + Mathf.RoundToInt(advantage);
      
        _hostChances -= _weakness;

        if (_hostChances >= 160)
            _hostChances = 95;
        else if (_hostChances >= 90)
            _hostChances = 90;
        else if (_hostChances <= -160)
            _hostChances = 5;
        else if (_hostChances < 10)
            _hostChances = 10;

        float ratingDiff = Mathf.Abs(Database.clubDB[_hostId].Rate - Database.clubDB[_guestId].Rate) / 10;
        float goalChances = (5 + (ratingDiff / 3)) / _curiosity + _changedCuriosity;
        _goalChances = Mathf.Clamp(Mathf.RoundToInt(goalChances), 4, 35);
    }

    void CalculateWeaknessOfTheDay()
    {
        _weakness = 0;
        int rnd = Random.Range(1, 101);

        if (rnd > 10)
            return;

        int ran = Random.Range(0, Database.clubDB[_hostId].Rate + Database.clubDB[_guestId].Rate);

        bool hostIsWeakToday = Database.clubDB[_hostId].Rate > ran;

        _weakness = hostIsWeakToday ? 10 : -10;

        if (Database.clubDB[_hostId].Rate > Database.clubDB[_guestId].Rate)
            _curiosity = hostIsWeakToday ? 2 : 0.5f;
        else if (Database.clubDB[_hostId].Rate < Database.clubDB[_guestId].Rate)
            _curiosity = hostIsWeakToday ? 0.5f : 2;
        else
            _curiosity = 3;
    }

    void Clear()
    {
        _matchStats[0].Reset();
        _matchStats[1].Reset();
        _minute = 0;
    }

    void UpdateCuriosity()
    {
        int multiplier = GuestBall * 2 - 1;

        if (Database.clubDB[_hostId].Rate > Database.clubDB[_guestId].Rate)
            _changedCuriosity += 5 * multiplier;
        else if (Database.clubDB[_hostId].Rate < Database.clubDB[_guestId].Rate)
            _changedCuriosity += -5 * multiplier;
        else
            _changedCuriosity -= 2;
    }

    void GoalScored()
    {
        _matchStats[GuestBall].GoalScored();
        _matchStats[GuestBall].AddScorer(_teams[GuestBall][PlayerWithBall], _teamName[GuestBall], 1);
        UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.GOAL_SCORED);
        _teams[GuestBall][PlayerWithBall].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.Goals);
        _teamsMatchData[GuestBall].ForEach(data => data.UpdateMatchRating(MatchRatingConstants.GOAL_SCORED_TEAM));
        _teamsMatchData[ReverseGuestBall].ForEach(data => data.UpdateMatchRating(MatchRatingConstants.GOAL_LOST_TEAM));
        if (_prevPlayerWithBall != -1)
        {
            _teams[GuestBall][_prevPlayerWithBall].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.Assists);
            UpdateMatchRatingForPlayer(GuestBall, _prevPlayerWithBall, MatchRatingConstants.ASSIST);
            _prevPlayerWithBall = -1;
        }
        UpdateCuriosity();
    }

    void UpdateMatchRatingForCurrentPlayer(float change)
    {
        UpdateMatchRatingForPlayer(GuestBall, PlayerWithBall, change);
    }

    void UpdateMatchRatingForPreviousPlayer(float change)
    {
        UpdateMatchRatingForPlayer(GuestBall, _prevPlayerWithBall, change);
    }

    void UpdateMatchRatingForPlayer(int guestTeam, int id, float change)
    {
        _teamsMatchData[guestTeam][id].UpdateMatchRating(change);
    }

    void MinutePassed()
    {
        _minute++;

        _teams[0].IncreaseMinuteFatigueForAll();
        _teams[1].IncreaseMinuteFatigueForAll();
        _HostSquadStatsUI.ForEach(f => f.UpdateState());
        _GuestSquadStatsUI.ForEach(f => f.UpdateState());
    }

    IEnumerator FreeKick(bool isPenalty)
    {
        // no assist when free kick
        PlayerWithBall = -1;

        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        if (isPenalty)
        {
            MinutePassed();
            PlayerWithBall = _teams[GuestBall].GetPenaltyExecutor();
            CommentLine.Instance.PreparingToPenalty();
            MinutePassed();

            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            float plus = (_teams[GuestBall][PlayerWithBall].Penalty / 5) - (_teams[ReverseGuestBall][0].Rating / 7);
            int rnd = Random.Range(1, 101);
            if (rnd < 80 + plus)
            {
                // gol
                GoalScored();
                CommentLine.Instance.PenaltyGoal();
            }
            else
            {
                CommentLine.Instance.PenaltyMissed();

                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.PENALTY_MISSED);
                UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.PENALTY_SAVED);
            }

            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            StartCoroutine(CommentStart());
        }
        else
        {
            //TODO: free kick
        }
    }

    IEnumerator Shot(int difficulty)
    {
        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        if (difficulty == 10)
        {
            // strzał z bardzo daleka
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int x = Random.Range(1, 101);
            float plus = (_teams[GuestBall][PlayerWithBall].Shoot / 5) - (_teams[ReverseGuestBall][0].Rating / 10);
            if (x <= 20)
            {
                // rożny
                //TODO: differentiate between richochet and saved by goalkeeper
                CommentLine.Instance.LongShotCorner();
                UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.LONG_SAVE_SUCCESS);
                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.LONG_SHOT_SUCCESS);
                StartCoroutine(Corner());
            }
            else
            {
                if (x <= 30 + plus)
                {
                    // TODO: differentiate betweeen fail save and no chance goal
                    CommentLine.Instance.LongShotGoal();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.LONG_SAVE_FAIL);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.LONG_SHOT_SUCCESS);
                    GoalScored();
                }
                else
                {
                    CommentLine.Instance.LongShotMiss();
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.LONG_SHOT_FAIL);
                }

                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 8)
        {
            // strzal z okolo 20 metrow przy kontrataku
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(20 + _teams[ReverseGuestBall][0].Rating / 3);
            int shooterPlus = (int)(20 + _teams[0][PlayerWithBall].Shoot / 5);
            int x = Random.Range(1, keeperPlus + shooterPlus + 10);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 31) <= _teams[ReverseGuestBall][0].Rating / 10)  // maks 10/30 szansy ze zlapie
                {
                    //lapie
                    CommentLine.Instance.CounterAttackSave();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.COUNTER_SAVE_SUCCESS);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.COUNTER_SHOT_FAIL);
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.Instance.CounterAttackCorner();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.COUNTER_SAVE_CORNER);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.COUNTER_SHOT_SUCCESS);
                    StartCoroutine(Corner());
                }
            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    CommentLine.Instance.CounterAttackGoal();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.COUNTER_SAVE_FAIL);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.COUNTER_SHOT_SUCCESS);
                    GoalScored();
                }
                else
                {
                    CommentLine.Instance.CounterAttackMiss();
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.COUNTER_SHOT_FAIL);
                }
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 5)  // okolo 16 metrow normalny atak
        {
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(5 + _teams[ReverseGuestBall][0].Rating / 5);
            int shooterPlus = (int)(15 + _teams[GuestBall][PlayerWithBall].Shoot / 3);
            int x = Random.Range(1, keeperPlus + shooterPlus + 15);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 51) <= _teams[ReverseGuestBall][0].Rating / 10)  // maks 10/50 szansy ze zlapie
                {
                    //lapie
                    CommentLine.Instance.NormalAttackSave();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.NORMAL_SAVE_SUCCESS);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.NORMAL_SHOT_FAIL);
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.Instance.NormalAttackCorner();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.NORMAL_SAVE_CORNER);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.NORMAL_SHOT_SUCCESS);
                    StartCoroutine(Corner());
                }
            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    CommentLine.Instance.NormalAttackGoal();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.NORMAL_SAVE_FAIL);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.NORMAL_SHOT_SUCCESS);
                    GoalScored();
                }
                else
                {
                    CommentLine.Instance.NormalAttackMiss();
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.NORMAL_SHOT_FAIL);
                }

                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 2)    //   Sam na sam
        {
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(10 + _teams[ReverseGuestBall][0].Rating / 5);
            int shooterPlus = (int)(25 + _teams[GuestBall][PlayerWithBall].Shoot / 2);
            int x = Random.Range(1, keeperPlus + shooterPlus + 5);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 51) <= _teams[ReverseGuestBall][0].Rating / 10)  // maks 10/50 szansy ze zlapie albo nie bedzie roznego
                {
                    //lapie
                    CommentLine.Instance.OneOnOneAttackSave();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.ONE_ON_ONE_SAVE_SUCCESS);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.ONE_ON_ONE_SHOT_FAIL);
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.Instance.OneOnOneAttackCorner();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.ONE_ON_ONE_SAVE_CORNER);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.ONE_ON_ONE_SHOT_FAIL);
                    StartCoroutine(Corner());
                }
            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    CommentLine.Instance.OneOnOneAttackGoal();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.ONE_ON_ONE_SAVE_FAIL);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.ONE_ON_ONE_SHOT_SUCCESS);
                    GoalScored();
                }
                else
                {
                    CommentLine.Instance.OneOnOneAttackMiss();
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.ONE_ON_ONE_SHOT_FAIL);
                }

                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        // TODO: detach headers from difficulty 3
        else if (difficulty == 3)    //   główka
        {
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(5 + _teams[ReverseGuestBall][0].Rating / 5);
            int shooterPlus = (int)(10 + _teams[GuestBall][PlayerWithBall].Heading / 2);
            int x = Random.Range(1, keeperPlus + shooterPlus + 10);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 31) <= _teams[ReverseGuestBall][0].Rating / 10)  // maks 10/30 szansy ze zlapie albo nie bedzie roznego
                {
                    //lapie
                    CommentLine.Instance.HeaderSave();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.GOOD_SAVE_SUCCESS);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.GOOD_SHOT_FAIL);
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.Instance.HeaderCorner();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.GOOD_SAVE_CORNER);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.GOOD_SHOT_SUCCESS);
                    StartCoroutine(Corner());
                }

            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    CommentLine.Instance.HeaderGoal();
                    UpdateMatchRatingForPlayer(ReverseGuestBall, 0, MatchRatingConstants.GOOD_SAVE_FAIL);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.GOOD_SHOT_SUCCESS);
                    GoalScored();
                }
                else
                {
                    CommentLine.Instance.HeaderMiss();
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.GOOD_SHOT_FAIL);
                }
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
    }

    IEnumerator CounterAttack()
    {
        // getting ball from opponent
        _prevPlayerWithBall = -1;

        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        int rnd = Random.Range(1, 100);
        float x = 20 - _teams[GuestBall][PlayerWithBall].Pass / 10;
        int potentialAttackerIndex = _teams[GuestBall].GetIndexOfRandomAttacker(PlayerWithBall);

        if (potentialAttackerIndex == -1 || rnd <= 30)
        {
            CommentLine.Instance.CounterAttackShotTry();

            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            StartCoroutine(Shot(8));
        }
        else if (rnd <= 30 + x)
        {
            // TODO: differentiate between bad pass and good tackle/interception
            CommentLine.Instance.CounterAttackFailedPass();
            UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.COUNTER_PASS_FAIL);
            MinutePassed();
            StartCoroutine(CommentStart());
        }
        else
        {
            CommentLine.Instance.CounterAttackSuccessPass();
            UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.COUNTER_PASS_SUCCESS);

            PlayerWithBall = potentialAttackerIndex;

            CommentLine.Instance.CounterAttackPreShot();

            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            // TODO: add chance to clean tackle based on defender's tackle and attacker's ... ball control?
            int chan = Random.Range(1, 101);
            if (chan > 70)
            {
                CommentLine.Instance.CounterAttackPenaltyFoul();
                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.PENALTY_WON);
                StartCoroutine(FreeKick(true));
            }
            else
            {
                StartCoroutine(Shot(2));
            }
        }
    }

    IEnumerator Corner()
    {
        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        int cornerExecutorIndex = _teams[GuestBall].GetCornerExecutorIndex();
        CommentLine.Instance.CornerExecution(_teams[GuestBall][cornerExecutorIndex]);
        // TODO: stat Corner is used only to check for executor, but not to estimate player's ability to execute corners...
        float border = 35 + ((_teams[GuestBall][cornerExecutorIndex].FreeKicks + _teams[GuestBall][cornerExecutorIndex].Pass) * .32f);

        if (Random.Range(1, 101) <= border)
        {
            int pairIndex = Random.Range(0, 89) / 30;

            var attackerIndex = _teams[GuestBall].GetCornerHeaderExecutorIndex(pairIndex, cornerExecutorIndex);
            var defenderIndex = _teams[ReverseGuestBall].GetCornerHeaderExecutorIndex(pairIndex);

            PlayerWithBall = attackerIndex;
            // TODO: instead of comparing rating, could compare positioning or smth
            // główka
            if (Random.Range(1, 101) <= 25 + ((_teams[GuestBall][attackerIndex].Rating - _teams[ReverseGuestBall][defenderIndex].Rating) / 5))
            {
                // zgubienie obrońcy i strzał głową
                if(_time > 0) 
                    yield return new WaitForSeconds(_time);

                CommentLine.Instance.FreeHeader();
                UpdateMatchRatingForPlayer(GuestBall, cornerExecutorIndex, MatchRatingConstants.CORNER_SUCCESS);
                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.CORNER_WON_POSITIONING);
                UpdateMatchRatingForPlayer(ReverseGuestBall, defenderIndex, MatchRatingConstants.CORNER_LOST_POSITIONING);

                if(_time > 0) 
                    yield return new WaitForSeconds(_time);

                // TODO: defferentiate between free header and contested header
                StartCoroutine(Shot(3));
            }
            else
            {
                if(_time > 0) 
                    yield return new WaitForSeconds(_time);

                if (Random.Range(1, 101) <= 50 + ((_teams[GuestBall][attackerIndex].Heading - _teams[ReverseGuestBall][defenderIndex].Heading) / 2))
                {
                    CommentLine.Instance.ContestedHeader();
                    UpdateMatchRatingForPlayer(GuestBall, cornerExecutorIndex, MatchRatingConstants.CORNER_SUCCESS);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.CORNER_WON_HEADER);
                    UpdateMatchRatingForPlayer(ReverseGuestBall, defenderIndex, MatchRatingConstants.CORNER_LOST_HEADER);

                    if (_time > 0) 
                        yield return new WaitForSeconds(_time);

                    StartCoroutine(Shot(3));
                }
                else
                {
                    CommentLine.Instance.DefenderWinsHeader(_teams[ReverseGuestBall][defenderIndex]);
                    UpdateMatchRatingForPlayer(GuestBall, cornerExecutorIndex, MatchRatingConstants.CORNER_SUCCESS);
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.CORNER_LOST_HEADER);
                    UpdateMatchRatingForPlayer(ReverseGuestBall, defenderIndex, MatchRatingConstants.CORNER_WON_HEADER);
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
            }
        }
        else
        {
            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            CommentLine.Instance.FailedCross();
            UpdateMatchRatingForPlayer(GuestBall, cornerExecutorIndex, MatchRatingConstants.CORNER_FAIL);

            MinutePassed();
            StartCoroutine(CommentStart());
        }
    }

    IEnumerator AttackThirdPhase(Direction direction)
    {
        MinutePassed();

        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        if (direction == Direction.Left || direction == Direction.Right)
        {
            CommentLine.Instance.TryingToGetPastDefender();

            if (_time > 0)
                yield return new WaitForSeconds(_time);

            int defenderIndex = _teams[ReverseGuestBall].GetIndexOfDefensiveWinger((int)direction);

            float plus = defenderIndex == -1 ? 100 
                : ((_teams[GuestBall][PlayerWithBall].Dribling + _teams[GuestBall][PlayerWithBall].Speed) 
                - (_teams[ReverseGuestBall][defenderIndex].Tackle + _teams[ReverseGuestBall][defenderIndex].Speed)) 
                / 3;

            // add variant with empty space without need for dribbling
            if (Random.Range(1, 101) < 55 + plus)
            {
                if (Random.Range(1, 101) <= 70)
                {
                    CommentLine.Instance.DecidesToCross();

                    if (defenderIndex != -1)
                    {
                        UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.DRIBBLE_DUEL_WON);
                        UpdateMatchRatingForPlayer(ReverseGuestBall, defenderIndex, MatchRatingConstants.DRIBBLE_DUEL_LOST);
                    }

                    float border = 40 + (_teams[0][PlayerWithBall].Pass / 3);           //----------------- ewentualnie zmniejszyc mnożnik gdyby za dużo goli z główki
                    int acc = Random.Range(1, 100);
                    if (acc <= border)
                    {
                        int attackerHeader = _teams[GuestBall].GetIndexOfRandomAttacker(PlayerWithBall);
                        
                        int defenderHeader = _teams[ReverseGuestBall].GetIndexOfRandomDefender(defenderIndex);

                        // główka
                        if (Random.Range(1, 101) <= (30 + (_teams[GuestBall][attackerHeader].Rating - _teams[ReverseGuestBall][defenderHeader].Rating) / 3))
                        {
                            // zgubienie obrońcy i strzał głową
                            if(_time > 0) 
                                yield return new WaitForSeconds(_time);

                            PlayerWithBall = attackerHeader;
                            CommentLine.Instance.FreeHeader();
                            UpdateMatchRatingForPreviousPlayer(MatchRatingConstants.CROSS_SUCCESS);
                            UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.CROSS_WON_POSITIONING);
                            UpdateMatchRatingForPlayer(ReverseGuestBall, defenderHeader, MatchRatingConstants.CROSS_LOST_POSITIONING);

                            if (_time > 0) 
                                yield return new WaitForSeconds(_time);

                            StartCoroutine(Shot(3));
                        }
                        else
                        {
                            if(_time > 0) 
                                yield return new WaitForSeconds(_time);

                            if (Random.Range(1, 101) <= 50 + ((_teams[GuestBall][attackerHeader].Heading - _teams[ReverseGuestBall][defenderHeader].Heading) / 3))
                            {
                                PlayerWithBall = attackerHeader;
                                CommentLine.Instance.ContestedHeader();
                                UpdateMatchRatingForPreviousPlayer(MatchRatingConstants.CROSS_SUCCESS);
                                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.CROSS_WON_HEADER);
                                UpdateMatchRatingForPlayer(ReverseGuestBall, defenderHeader, MatchRatingConstants.CROSS_LOST_HEADER);

                                if (_time > 0) 
                                    yield return new WaitForSeconds(_time);

                                StartCoroutine(Shot(3));
                            }
                            else
                            {
                                CommentLine.Instance.DefenderWinsHeader(_teams[ReverseGuestBall][defenderHeader]);
                                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.CROSS_SUCCESS);
                                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.CROSS_LOST_HEADER);
                                UpdateMatchRatingForPlayer(ReverseGuestBall, defenderHeader, MatchRatingConstants.CROSS_WON_HEADER);

                                MinutePassed();
                                StartCoroutine(CommentStart());
                            }
                        }
                    }
                    else
                    {
                        if(_time > 0) 
                            yield return new WaitForSeconds(_time);

                        CommentLine.Instance.FailedCross();
                        UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.CROSS_FAIL);
                        MinutePassed();
                        StartCoroutine(CommentStart());
                    }
                }
                else
                {
                    CommentLine.Instance.DecidesToShootInsteadOfCrossing();
                    if (defenderIndex != -1)
                    {
                        UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.DRIBBLE_DUEL_WON);
                        UpdateMatchRatingForPlayer(ReverseGuestBall, defenderIndex, MatchRatingConstants.DRIBBLE_DUEL_LOST);
                    }
                    StartCoroutine(Shot(5));
                }
            }
            else
            {
                CommentLine.Instance.FailedWingDribble((int)direction);
                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.DRIBBLE_DUEL_LOST);
                UpdateMatchRatingForPlayer(ReverseGuestBall, _teams[ReverseGuestBall].GetIndexOfDefensiveWinger((int)direction), MatchRatingConstants.DRIBBLE_DUEL_WON);
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else
        {
            int firstDef = _teams[ReverseGuestBall].GetIndexOfRandomMiddleDefender();
            if (firstDef == -1) 
            {
                CommentLine.Instance.MiddleEmptySpace();
                StartCoroutine(Shot(2));
                yield break;
            }

            CommentLine.Instance.TryingToGetPastDefender();

            if (_time > 0)
                yield return new WaitForSeconds(_time);

            float plus = ((_teams[GuestBall][PlayerWithBall].Dribling + _teams[GuestBall][PlayerWithBall].Speed) - (_teams[ReverseGuestBall][firstDef].Tackle + _teams[ReverseGuestBall][firstDef].Speed)) / 3;
            if (Random.Range(1, 101) < 45 + plus)
            {
                //podanie bądź strzał
                if (Random.Range(1, 101) <= 65)
                {
                    CommentLine.Instance.DecidesToPass();
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.DRIBBLE_DUEL_WON);
                    UpdateMatchRatingForPlayer(ReverseGuestBall, firstDef, MatchRatingConstants.DRIBBLE_DUEL_LOST);

                    float border = 30 + _teams[GuestBall][PlayerWithBall].Pass / 3;
                    if (Random.Range(1, 101) <= border)
                    {
                        PlayerWithBall = _teams[GuestBall].GetIndexOfRandomAttacker(PlayerWithBall);

                        int defenderIndex = _teams[GuestBall].GetIndexOfRandomMiddleDefender(firstDef);

                        if(_time > 0) 
                            yield return new WaitForSeconds(_time);

                        CommentLine.Instance.ChanceForOneOnOne();
                        UpdateMatchRatingForPreviousPlayer(MatchRatingConstants.ADVANCED_PASS_FAIL);

                        if (_time > 0)
                            yield return new WaitForSeconds(_time);

                        if (Random.Range(1, 101) <= (40 + (_teams[GuestBall][PlayerWithBall].Dribling - _teams[ReverseGuestBall][defenderIndex].Tackle) / 3))
                        {
                            CommentLine.Instance.OneToOneSituation();
                            UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.KEY_DRIBBLE_DUEL_WON);
                            UpdateMatchRatingForPlayer(ReverseGuestBall, defenderIndex, MatchRatingConstants.KEY_DRIBBLE_DUEL_LOST);

                            if (_time > 0) 
                                yield return new WaitForSeconds(_time);

                            StartCoroutine(Shot(2));
                        }
                        else
                        {
                            PlayerWithBall = defenderIndex;
                            GuestBall = ReverseGuestBall;
                            CommentLine.Instance.FailedChanceOneToOne();
                            UpdateMatchRatingForPlayer(ReverseGuestBall, _prevPlayerWithBall, MatchRatingConstants.KEY_DRIBBLE_DUEL_LOST);
                            UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.KEY_DRIBBLE_DUEL_WON);
                            MinutePassed();
                            StartCoroutine(CommentStart());
                        }
                    }
                    else
                    {
                        if(_time > 0) 
                            yield return new WaitForSeconds(_time);

                        CommentLine.Instance.FailedPass();
                        UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.ADVANCED_PASS_FAIL);
                        MinutePassed();
                        StartCoroutine(CommentStart());
                    }
                }
                else
                {
                    CommentLine.Instance.DecidesToShootInsteadOfPassing();
                    UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.DRIBBLE_DUEL_WON);
                    UpdateMatchRatingForPlayer(ReverseGuestBall, firstDef, MatchRatingConstants.DRIBBLE_DUEL_LOST);
                    StartCoroutine(Shot(5));
                }
            }
            else
            {
                CommentLine.Instance.FailedMidDribble(_teams[ReverseGuestBall][firstDef]);
                UpdateMatchRatingForCurrentPlayer(MatchRatingConstants.DRIBBLE_DUEL_LOST);
                UpdateMatchRatingForPlayer(ReverseGuestBall, firstDef, MatchRatingConstants.DRIBBLE_DUEL_WON);
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
	}
}
