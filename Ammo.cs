using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private AmmoSlot[] ammoSlots;

    [System.Serializable]
    private class AmmoSlot
    {
        public AmmoType ammoType;
        public int ammoCount = 0;
    }

    public int GetCurrentAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        return slot != null ? slot.ammoCount : 0;
    }

    public void ReduceCurrentAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        if (slot != null && slot.ammoCount > 0)
        {
            slot.ammoCount--;
        }
    }

    public void IncreaseCurrentAmmo(AmmoType ammoType, int ammoAmount)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        if (slot != null)
        {
            slot.ammoCount += ammoAmount;
        }
    }

    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach (AmmoSlot slot in ammoSlots)
        {
            if (slot.ammoType == ammoType)
                return slot;
        }

        Debug.LogError($"Ammo type {ammoType} not found!");
        return null;
    }
}
