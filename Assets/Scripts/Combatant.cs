using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    public string combatantName;

    public int attack;
    public int defense;
    public int speed;
    public int maxHP;
    public int currentHP;

    
    public BattleAbilities meleeAttack;
    public BattleAbilities specialMove;


    public bool TakeDamage(int damage)
    {
        currentHP -= damage;

        return HasDied();
    }

    public bool HasDied()
    {
        if (currentHP <= 0)
        {
            return true;
        }
        return false;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

}
