using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public string monsterName;

    public int meleeDamage;
    public int magicDamage;

    public int maxHP;
    public int currentHP;

    /**
     * Returns True if monster dies to damage
     */
    public bool TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
