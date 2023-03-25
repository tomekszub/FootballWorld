using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconMaster")]
public class IconMaster : SerializedScriptableObject
{
    [SerializeField] Dictionary<Perk, Sprite> _PerkIcons;

    public Sprite GetPerkIcon(Perk perk) => _PerkIcons[perk];
}
