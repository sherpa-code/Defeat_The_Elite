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

    public string specialAbilityName;
    public string specialAbilityDescription;
    public int specialAbilityPower;

    public int maxHP;
    public int currentHP;
    
    //public BattleAbilities meleeAttack;
    //public BattleAbilities specialMove;

    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public bool TakeDamage(int damage)
    {
        if (currentHP - damage < 0)
        {
            currentHP = 0;
        } else
        {
            currentHP -= damage;
        }
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

    public int getSpeed()
    {
        return this.speed;
    }

    public IEnumerator playHurtAnimation()
    {
        animator.SetBool("Was Hit", true);
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("Was Hit", false);
    }
    public IEnumerator playDeathAnimation()
    {
        animator.SetBool("Was Hit", true);
        animator.SetBool("Dead", true);
        yield return new WaitForSeconds(1.5f);
        
    }
    public IEnumerator playAttackAnimation()
    {
        animator.SetBool("Melee Attacking", true);
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("Melee Attacking", false);
    }
    public IEnumerator playSpecialAnimation()
    {
        animator.SetBool("Magic Attacking", true);
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("Magic Attacking", false);
    }
}
