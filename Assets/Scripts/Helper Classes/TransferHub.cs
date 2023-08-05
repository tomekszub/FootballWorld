public static class TransferHub
{
    public enum TransferResult
    {
        Success = 0,
        NotEnoughMoney = 1,
        TooFewPlayers = 2,
        TooManyPlayers = 3
    }

    public static TransferResult TryTransferToMyClub(Footballer footballer, decimal fee)
    {
        Club to = MyClub.Instance.Club;

        Club from = Database.clubDB[footballer.ClubID];

        if (!to.HasSpaceForPlayer())
            return TransferResult.TooManyPlayers;

        if (!from.HasEnoughPlayers())
            return TransferResult.TooFewPlayers;

        if (!to.HasEnoughMoney(fee))
            return TransferResult.NotEnoughMoney;

        Transfer(footballer, from, to, fee);

        MyClub.Instance.UpdateBudgetUI();

        MyClub.Instance.SetKnowledgeAboutFootballer(footballer.Id, MyClub.MAX_KNOWLEDGE_LEVEL);

        MyClub.Instance.AddNewFootballerToPresets(footballer.Id);

        from.RefreshSquad();

        return TransferResult.Success;
    }

    public static TransferResult TryTransfer(Footballer footballer, Club to, decimal fee)
    {
        return TransferResult.Success;
    }

    static void Transfer(Footballer footballer,Club from, Club to, decimal fee)
    {
        from.RemovePlayer(footballer.Id);

        footballer.ClubID = to.Id;

        to.AddPlayer(footballer.Id, fee);
    }

    public static bool TerminateContract(Footballer footballer)
    {
        if (!Database.clubDB[footballer.ClubID].HasEnoughPlayers())
            return false;

        Database.clubDB[footballer.ClubID].RemovePlayer(footballer.Id);

        footballer.ClubID = -1;

        return true;
    }
}
