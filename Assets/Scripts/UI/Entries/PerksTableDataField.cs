using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerksTableDataField : TableDataField
{
    [SerializeField] List<Image> _PerkSlots;

    public void SetPerks(IEnumerable<Perk> perks)
    {
        _PerkSlots.ForEach(p => p.gameObject.SetActive(false));

        int index = 0;

        var iconMaster = Database.Instance.IconMaster;

        foreach (var perk in perks) 
        {
            if (index == _PerkSlots.Count)
                break;

            _PerkSlots[index].gameObject.SetActive(true);
            _PerkSlots[index].sprite = iconMaster.GetPerkIcon(perk);
        }
    }
}
