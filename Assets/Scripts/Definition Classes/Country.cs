using UnityEngine;

public class Country
{
    [SerializeField] string _Name;
    [SerializeField] Sprite _Flag;
    [SerializeField] TextAsset _ClubNameFirstParts;
    [SerializeField] TextAsset _ClubNameSecondParts;
    [SerializeField] TextAsset _Names;
    [SerializeField] TextAsset _Surnames;

    public TextAsset GetClubNameFirstPart() => _ClubNameFirstParts;
    public TextAsset GetClubNameSecondPart() => _ClubNameSecondParts;
    public TextAsset GetNames() => _Names;
    public TextAsset GetSurnames() => _Surnames;
    public Sprite GetFlag() => _Flag;
}
