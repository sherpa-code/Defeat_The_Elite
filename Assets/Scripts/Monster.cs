using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Monster : MonoBehaviour
{
    public string monsterName;

    public int attack;
    public int defense;
    public int speed;
    public string specialAbility;
    public string specialAbilityDescription;
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
