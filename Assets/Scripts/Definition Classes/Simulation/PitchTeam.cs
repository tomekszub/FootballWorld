using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Footballer.PlayerStatistics;

public class PitchTeam
{
    List<Footballer> _players;
    string _competitionName;
    int[] _playersMidPos;
    int[] _playersMidDefPos;
    int[] _wingPos = new int[2];
    int[] _defWingPos = new int[2];
    int _defLastPlayerNumber;
    int _midLastPlayerNumber;

    public float TeamDefenceRating { get; private set; }

    public float TeamMidfieldRating { get; private set; }

    public float TeamAttackRating { get; private set; }

    public PitchTeam(List<Footballer> players, string competitionName, string formation)
    {
        _players = players;
        _competitionName = competitionName;
        RecognizeFormation(formation);
    }

    public Footballer this[int i] => _players[i];

    #region Specific Players

    public int GetIndexOfWinger(int isRightWing) => _wingPos[isRightWing];

    public int GetIndexOfDefensiveWinger(int isRightWing) => _defWingPos[isRightWing];

    public int GetIndexOfLastDefender() => _defLastPlayerNumber;

    #endregion

    #region Random Players

    public int GetIndexOfRandomDefender()
    {
        return Random.Range(1, _defLastPlayerNumber + 1);
    }
    
    public int GetIndexOfRandomAttacker()
    {
        return Random.Range(_midLastPlayerNumber + 1, 11);
    }

    public int GetIndexOfRandomAttacker(int excludeID)
    {
        List<int> indices = Enumerable.Range(_midLastPlayerNumber + 1, 10 - _midLastPlayerNumber).ToList();
        indices.Remove(excludeID);
        return indices[Random.Range(0, indices.Count)];
    }

    public int GetIndexOfRandomDefender(int excludeID)
    {
        List<int> indices = Enumerable.Range(1, _defLastPlayerNumber).ToList();
        indices.Remove(excludeID);
        return indices[Random.Range(0, indices.Count)];
    }

    public int GetIndexOfRandomMidfielder()
    {
        return Random.Range(_defLastPlayerNumber + 1, _midLastPlayerNumber + 1);
    }

    public int GetIndexOfRandomMiddlePlayer()
    {
        return _playersMidPos[Random.Range(0, _playersMidPos.Length)];
    }

    public int GetIndexOfRandomMiddleDefender()
    {
        return _playersMidDefPos[Random.Range(0, _playersMidDefPos.Length)];
    }

    public int GetIndexOfRandomMiddleDefender(int excludeID)
    {
        List<int> indices = new(_playersMidDefPos);
        indices.Remove(excludeID);
        return indices[Random.Range(0, indices.Count)];
    }

    #endregion

    #region Stats

    public void IncreaseMinuteFatigueForAll()
    {
        for (int i = 0; i < _players.Count; i++)
            _players[i].GainFatigue(i == 0);
    }

    public void ReportPlayerStartedMatch(int index) => AddStatisticToPlayer(index, StatName.MatchesPlayed);

    public void ReportAllPlayersStartedMatch()
    {
        for (int i = 0; i < 11; i++)
            ReportPlayerStartedMatch(i);
    }

    public void ReportPlayerMatchRating(int index, float rating) => AddStatisticToPlayer(index, StatName.MatchRating, rating);

    public void ReportAllPlayersCleanSheet()
    {
        for (int i = 0; i < 11; i++)
            AddStatisticToPlayer(i, StatName.CleanSheet);
    }

    void AddStatisticToPlayer(int index, StatName statName, double val = 1)
    {
        if (index >= _players.Count || _players[index] == null)
            return;

        _players[index].AddStatistic(_competitionName, statName, val);
    }

    #endregion

    void RecognizeFormation(string formation)
    {
        if (formation == "4-3-3")
        {
            TeamDefenceRating = (_players[1].Rating + _players[2].Rating + _players[3].Rating + _players[4].Rating);
            TeamDefenceRating = (TeamDefenceRating / 4) + (TeamDefenceRating / 4);
            _defLastPlayerNumber = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 3) + (TeamMidfieldRating / 3);
            _midLastPlayerNumber = 7;
            TeamAttackRating = (_players[8].Rating + _players[9].Rating + _players[10].Rating);
            TeamAttackRating = (TeamAttackRating / 3) + (TeamAttackRating / 3);
            _playersMidPos = new[] { 10 };
            _wingPos[0] = 8;
            _wingPos[1] = 9;
        }
        else if (formation == "4-2-3-1" || formation == "4-4-1-1")
        {
            TeamDefenceRating = (_players[1].Rating + _players[2].Rating + _players[3].Rating + _players[4].Rating);
            TeamDefenceRating = (TeamDefenceRating / 4) + (TeamDefenceRating / 4);
            _defLastPlayerNumber = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 2) + (TeamMidfieldRating / 3);
            _midLastPlayerNumber = 6;
            TeamAttackRating = (_players[7].Rating + _players[8].Rating + _players[9].Rating + _players[10].Rating);
            TeamAttackRating = (TeamAttackRating / 4) + (TeamAttackRating / 3);
            _playersMidPos = new[] { 7, 10 };
            _wingPos[0] = 8;
            _wingPos[1] = 9;
        }
        else if (formation == "4-1-4-1")
        {
            TeamDefenceRating = (_players[1].Rating + _players[2].Rating + _players[3].Rating + _players[4].Rating);
            TeamDefenceRating = (TeamDefenceRating / 4) + (TeamDefenceRating / 4);
            _defLastPlayerNumber = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 3) + (TeamMidfieldRating / 3);
            _midLastPlayerNumber = 6;
            TeamAttackRating = (_players[8].Rating + _players[9].Rating + _players[10].Rating);
            TeamAttackRating = (TeamAttackRating / 3) + (TeamAttackRating / 3);
            _playersMidPos = new[] { 6, 7, 10 };
            _wingPos[0] = 8;
            _wingPos[1] = 9;
        }
        else if (formation == "4-3-1-2")
        {
            TeamDefenceRating = (_players[1].Rating + _players[2].Rating + _players[3].Rating + _players[4].Rating);
            TeamDefenceRating = (TeamDefenceRating / 4) + (TeamDefenceRating / 4);
            _defLastPlayerNumber = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 3) + (TeamMidfieldRating / 3);
            _midLastPlayerNumber = 7;
            TeamAttackRating = (_players[8].Rating + _players[9].Rating + _players[10].Rating);
            TeamAttackRating = (TeamAttackRating / 3) + (TeamAttackRating / 3);
            _playersMidPos = new[] { 8, 9, 10 };
            _wingPos[0] = 6;
            _wingPos[1] = 7;
        }
        else if (formation == "4-4-2")
        {
            TeamDefenceRating = (_players[1].Rating + _players[2].Rating + _players[3].Rating + _players[4].Rating);
            TeamDefenceRating = (TeamDefenceRating / 4) + (TeamDefenceRating / 4);
            _defLastPlayerNumber = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 2) + (TeamMidfieldRating / 3);
            _midLastPlayerNumber = 6;
            TeamAttackRating = (_players[7].Rating + _players[8].Rating + _players[9].Rating + _players[10].Rating);
            TeamAttackRating = (TeamAttackRating / 4) + (TeamAttackRating / 3);
            _playersMidPos = new[] { 9, 10 };
            _wingPos[0] = 7;
            _wingPos[1] = 8;
        }
        else if (formation == "5-3-2")
        {
            TeamDefenceRating = (_players[1].Rating + _players[2].Rating + _players[3].Rating + _players[4].Rating + _players[5].Rating);
            TeamDefenceRating = (TeamDefenceRating / 5) + (TeamDefenceRating / 4);
            _defLastPlayerNumber = 5;
            TeamMidfieldRating = (_players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 2) + (TeamMidfieldRating / 3);
            _midLastPlayerNumber = 7;
            TeamAttackRating = (_players[8].Rating + _players[9].Rating + _players[10].Rating);
            TeamAttackRating = (TeamAttackRating / 3) + (TeamAttackRating / 3);
            _playersMidPos = new[] { 8, 9, 10 };
            _wingPos[0] = 6;
            _wingPos[1] = 7;
        }
        else if (formation == "3-4-1-2" || formation == "3-4-2-1")
        {
            TeamDefenceRating = (_players[1].Rating + _players[2].Rating + _players[3].Rating);
            TeamDefenceRating = (TeamDefenceRating / 3) + (TeamDefenceRating / 4);
            _defLastPlayerNumber = 3;
            TeamMidfieldRating = (_players[4].Rating + _players[5].Rating + _players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 4) + (TeamMidfieldRating / 3);
            _midLastPlayerNumber = 7;
            TeamAttackRating = (_players[8].Rating + _players[9].Rating + _players[10].Rating);
            TeamAttackRating = (TeamAttackRating / 3) + (TeamAttackRating / 3);
            _playersMidPos = new[] { 8, 9, 10 };
            _wingPos[0] = 6;
            _wingPos[1] = 7;
        }
        if (_defLastPlayerNumber == 5)
        {
            _playersMidDefPos = new[] { 2, 3, 4 };
            _defWingPos[0] = 1;
            _defWingPos[1] = 5;
        }
        else if (_defLastPlayerNumber == 4)
        {
            _playersMidDefPos = new[] { 2, 3 };
            _defWingPos[0] = 1;
            _defWingPos[1] = 4;
        }
        else if (_defLastPlayerNumber == 3)
        {
            _playersMidDefPos = new[] { 1, 2, 3 };
            _defWingPos[0] = 1;
            _defWingPos[1] = 3;
        }
    }
}
