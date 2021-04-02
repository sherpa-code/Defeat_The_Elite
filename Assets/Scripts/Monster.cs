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
    private AudioSource audioSource; //private because example I seen was private
    public AudioClip attackSound;
    public AudioClip specialSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        playHurtSound();
        animator.SetBool("Was Hit", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Was Hit", false);
        
    }
    public IEnumerator playDeathAnimation()
    {
        animator.SetBool("Was Hit", true);
        playDeathSound();
        animator.SetBool("Dead", true);
        yield return new WaitForEndOfFrame();

    }
    public IEnumerator playAttackAnimation()
    {
        playAttackSound();
        animator.SetBool("Melee Attacking", true);
        yield return new WaitForEndOfFrame(); ;
        animator.SetBool("Melee Attacking", false);
    }
    public IEnumerator playSpecialAnimation()
    {
        playSpecialSound();
        animator.SetBool("Magic Attacking", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Magic Attacking", false);
    }

    public void playHurtSound()
    {
        audioSource.PlayOneShot(hurtSound);
    }
    public void playDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }
    public void playAttackSound()
    {
        audioSource.PlayOneShot(attackSound);
    }
    public void playSpecialSound()
    {
        audioSource.PlayOneShot(specialSound);
    }

}
