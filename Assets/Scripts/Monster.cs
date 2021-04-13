using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Monster : MonoBehaviour {
    public string monsterName;

    public int attack;
    public int defense;
    public int speed;

    public string specialAbilityName;
    public string specialAbilityDescription;
    public int specialAbilityPower;
    public int specialDamage;
    public int specialPoisonDamage;
    public int specialChargesLeft;
    public int specialChargesMax;

    public int maxHP;
    public int currentHP;
    
    //public BattleAbilities meleeAttack;
    //public BattleAbilities specialMove;

    public Animator animator;
    public AudioSource audioSource; //private because example I seen was private
    public AudioClip attackSound;
    public AudioClip specialSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public int damageTaken;

    public bool isPoisoned;
    public int poisonDamageTaken;
    public bool isDeathBreathed;

    void Start() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void calculateDamage(int damageInput) {
        damageTaken = damageInput * (100 / (10 + defense));
    }

    public bool TakeDamage(int damageTaken) {
        if (currentHP - damageTaken < 0) {
            currentHP = 0;
        } else {
            currentHP -= damageTaken;
        }
        Debug.Log("damageTaken by " + monsterName + " = " + damageTaken);
        return HasDied();
    }

    public bool HasDied() {
        if (currentHP <= 0) {
            return true;
        }
        return false;
    }

    public void Heal(int amount) {
        currentHP += amount;
        if (currentHP > maxHP) {
            currentHP = maxHP;
        }
    }

    public IEnumerator playHurtAnimation() {
        audioSource.PlayOneShot(hurtSound);
        animator.SetBool("Was Hit", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Was Hit", false);
        
    }

    public IEnumerator playDeathAnimation() {
        audioSource.PlayOneShot(deathSound);
        animator.SetBool("Was Hit", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Dead", true);

    }

    public IEnumerator playAttackAnimation() {
        audioSource.PlayOneShot(attackSound);
        animator.SetBool("Melee Attacking", true);
        yield return new WaitForEndOfFrame(); ;
        animator.SetBool("Melee Attacking", false);
    }

    public IEnumerator playSpecialAnimation() {
        audioSource.PlayOneShot(specialSound);
        animator.SetBool("Magic Attacking", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Magic Attacking", false);
    }

    public int getSpeed() {
        return this.speed;
    }
}