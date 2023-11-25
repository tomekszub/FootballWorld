using System.Collections;
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
    int _defLastPlayer;
    int _midLastPlayer;

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

    #region PositionChecks

    public bool HasWinger(int rigthSide) => _players[_wingPos[rigthSide]] != null;

    public bool HasMiddfielder() => AnyPlayerOnThePitch(_defLastPlayer + 1, _midLastPlayer);

    #endregion

    #region Specific Players

    public int GetIndexOfWinger(int rightSide) => _players[_wingPos[rightSide]] == null ? -1 : _wingPos[rightSide];

    public int GetIndexOfDefensiveWinger(int rightSide) => _players[_defWingPos[rightSide]] == null ? -1 : _defWingPos[rightSide];

    public int GetIndexOfLastDefender() => _defLastPlayer;

    public int GetPenaltyExecutor()
    {
        List<int> penaltyPlayers = new(10);
        for (int i = 1; i < 11; i++)
        {
            if (_players[i] != null)
                penaltyPlayers.Add(i);
        }
        return penaltyPlayers.OrderByDescending(x => _players[x].Penalty).First();
    }

    public int GetCornerExecutorIndex()
    {
        List<int> cornerPlayers = new(10);
        for (int i = 1; i < 11; i++)
        {
            if (_players[i] != null)
                cornerPlayers.Add(i);
        }
        return cornerPlayers.OrderByDescending(x => _players[x].Corner).First();
    }

    public int GetCornerHeaderExecutorIndex(int rankPosition, int excludeID = -1)
    {
        List<int> headerPlayers = new(10);
        for (int i = 1; i < 11; i++)
        {
            if (_players[i] != null && excludeID != i)
                headerPlayers.Add(i);
        }

        if (rankPosition > headerPlayers.Count - 1)
            rankPosition = 0;

        return headerPlayers.OrderByDescending(x => _players[x].Heading).ElementAt(rankPosition);
    }

    #endregion

    #region Random Players

    public int GetIndexOfRandomDefender(int excludeID = -1) => GetRandomPlayerIndex(1, _defLastPlayer, excludeID);

    public int GetIndexOfRandomMidfielder(int excludeID = -1) => GetRandomPlayerIndex(_defLastPlayer + 1, _midLastPlayer, excludeID);

    public int GetIndexOfRandomAttacker(int excludeID = -1) => GetRandomPlayerIndex(_midLastPlayer + 1, 10, excludeID);

    public int GetIndexOfRandomMiddlePlayer(int excludeID = -1) => GetRandomPlayerIndex(_playersMidPos, excludeID);

    public int GetIndexOfRandomMiddleDefender(int excludeID = -1) => GetRandomPlayerIndex(_playersMidDefPos, excludeID);

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
            _defLastPlayer = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 3) + (TeamMidfieldRating / 3);
            _midLastPlayer = 7;
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
            _defLastPlayer = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 2) + (TeamMidfieldRating / 3);
            _midLastPlayer = 6;
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
            _defLastPlayer = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 3) + (TeamMidfieldRating / 3);
            _midLastPlayer = 6;
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
            _defLastPlayer = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 3) + (TeamMidfieldRating / 3);
            _midLastPlayer = 7;
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
            _defLastPlayer = 4;
            TeamMidfieldRating = (_players[5].Rating + _players[6].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 2) + (TeamMidfieldRating / 3);
            _midLastPlayer = 6;
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
            _defLastPlayer = 5;
            TeamMidfieldRating = (_players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 2) + (TeamMidfieldRating / 3);
            _midLastPlayer = 7;
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
            _defLastPlayer = 3;
            TeamMidfieldRating = (_players[4].Rating + _players[5].Rating + _players[6].Rating + _players[7].Rating);
            TeamMidfieldRating = (TeamMidfieldRating / 4) + (TeamMidfieldRating / 3);
            _midLastPlayer = 7;
            TeamAttackRating = (_players[8].Rating + _players[9].Rating + _players[10].Rating);
            TeamAttackRating = (TeamAttackRating / 3) + (TeamAttackRating / 3);
            _playersMidPos = new[] { 8, 9, 10 };
            _wingPos[0] = 6;
            _wingPos[1] = 7;
        }
        if (_defLastPlayer == 5)
        {
            _playersMidDefPos = new[] { 2, 3, 4 };
            _defWingPos[0] = 1;
            _defWingPos[1] = 5;
        }
        else if (_defLastPlayer == 4)
        {
            _playersMidDefPos = new[] { 2, 3 };
            _defWingPos[0] = 1;
            _defWingPos[1] = 4;
        }
        else if (_defLastPlayer == 3)
        {
            _playersMidDefPos = new[] { 1, 2, 3 };
            _defWingPos[0] = 1;
            _defWingPos[1] = 3;
        }
    }

    int GetRandomPlayerIndex(IEnumerable<int> sourceIndices, int excludeIndex = -1)
    {
        List<int> indices = new(sourceIndices);
        for (int i = indices.Count - 1; i >= 0; i--)
        {
            if (indices[i] == excludeIndex || _players[indices[i]] == null)
                indices.RemoveAt(i);
        }

        if(indices.Count == 0)
            return -1;

        return indices[Random.Range(0, indices.Count)];
    }

    int GetRandomPlayerIndex(int minIndex, int maxIndex, int excludeIndex = -1)
    {
        return GetRandomPlayerIndex(Enumerable.Range(minIndex, maxIndex - minIndex + 1), excludeIndex);
    }

    bool AnyPlayerOnThePitch(int minIndex, int maxIndex)
    {
        for (int i = minIndex; i < maxIndex; i++)
        {
            if (_players[i] == null)
                return false;
        }

        return true;
    }
}
