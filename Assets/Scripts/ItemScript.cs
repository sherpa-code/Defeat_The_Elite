using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public enum ItemType
    {
        HEAL,
        REVIVE,
        BUFF,
        ANTIDOTE
    }

    public string itemName;
    public int itemEffectiveness;
    
}
