using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Monster : MonoBehaviour {
    public string monsterName;

    public int attack;
    public int defense;
    public int speed;
    public int maxHP;
    public int currentHP;
    public bool needsHeals;
    public bool isPoisoned;
    public int poisonTurnsLeft;
    public int poisonDamageTaken;
    public bool isDeathBreathed;
    public int deathBreathTurnsLeft;
    public bool isDebuffed;
    public bool isDefending;

    public string specialAbilityName;
    public string specialAbilityDescription;
    public int specialDamage;
    public bool isSpecialPoison;
    public int specialPoisonDamage;
    public int specialPoisonDuration;
    public bool isSpecialDebuff;
    public int specialDebuffDuration;
    public int specialDebuffAttackAmount;
    public int specialDebuffDefenseAmount;
    public int specialDebuffSpeedAmount;
    public bool isSpecialDeathBreath;
    public int specialDeathBreathDuration;
    public bool isSpecialHeals;
    public int specialHealsAmount;
    //public int specialChargesLeft;
    //public int specialChargesMax;

    public int damageTaken;

    public Animator animator;
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip specialSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    public bool isPlayerMonster = false;
    public double playerDamageModifier = 1.1; // Used to balance combat
    public bool isEnemyMonster = false;
    //public double enemyDamageModifier = 0.8; // Used to balance combat
    public double enemyDamageModifier = 5; // Used to balance combat

    void Start() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public int calculateDamage(int damageInput) {
        double damageOutput = damageInput * (100.0 / (10.0 + defense));
        int damageApplied;
        if (isPlayerMonster) {
            damageApplied = (int)Math.Round(damageOutput * enemyDamageModifier);
            Debug.Log("isPlayerMonster and damageOutput*enemyMod = " + damageApplied);
        } else {
            damageApplied = (int)Math.Round(damageOutput * playerDamageModifier);
            Debug.Log("isEnemyMonster and damageOutput*playerMod = " + damageApplied);
        }
        return damageApplied;
    }

    public bool TakeDamage(int damageInput) {
        int damageTaken = calculateDamage(damageInput);

        if (currentHP - damageTaken < 0) {
            currentHP = 0;
        } else {
            currentHP -= damageTaken;
        }
        
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