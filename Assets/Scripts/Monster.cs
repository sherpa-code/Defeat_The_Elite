using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Monster : MonoBehaviour {
    public string monsterName;

    public int originalAttack;
    public int attack;
    public int originalDefense;
    public int defense;
    public int originalSpeed;
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
    public int debuffedTurnsLeft;
    public int debuffedAttackAmount;
    public int debuffedDefenseAmount;
    public int debuffedSpeedAmount;
    public bool isBuffed;
    public int buffedTurnsLeft;
    public int buffedAttackAmount;
    public int buffedDefenseAmount;
    public int buffedSpeedAmount;
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
    //public double playerDamageModifier = 1.1; // Used to balance combat
    public double playerDamageModifier = 3.0; // DEBUG
    //public bool isEnemyMonster = false;
    //public double enemyDamageModifier = 0.8; // Used to balance combat
    //public double enemyDamageModifier = 50000;
    public float enemyDamageModifier = 50000f;

    System.Single animLength;
    System.Single animBlendFactor = 0.35f;

    public int lastDamageTaken;



    void Start() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originalAttack = attack;
        originalDefense = defense;
        originalSpeed = speed;
    }

    public void updateMyStats() {
        //attack = originalAttack;
        //defense = originalDefense;
        //speed = originalSpeed;i

        attack = attack - debuffedAttackAmount + buffedAttackAmount;
        defense = defense - debuffedDefenseAmount + buffedDefenseAmount;
        speed = speed - debuffedSpeedAmount + buffedSpeedAmount;

        if (!isBuffed) {
            buffedAttackAmount = 0;
            buffedDefenseAmount = 0;
            buffedSpeedAmount = 0;
        }

        if (!isDebuffed) {
            debuffedAttackAmount = 0;
            debuffedDefenseAmount = 0;
            debuffedSpeedAmount = 0;
        }

        
    }

    //public void updateMyStats() {
    //    if (isDebuffed) {
    //        attack = attack - debuffedAttackAmount;
    //        defense = defense - debuffedDefenseAmount;
    //        speed = speed - debuffedSpeedAmount;
    //    } else {
    //        attack = originalAttack;
    //        defense = originalDefense;
    //        speed = originalSpeed;
    //    }
    //}

    public int calculateDamage(int damageInput) {
        //double damageOutput = damageInput * (100.0 / (10.0 + defense));
        float damageOutput = damageInput * (100.0f / (10.0f + defense));
        Debug.Log("damageApplied = " + damageOutput);
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
        lastDamageTaken = damageTaken;

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

    public int Heal(int amount) { // TODO: confirm that this logic works
        int healedAmount = 0;
        if (currentHP + amount > maxHP) {
            healedAmount = maxHP - currentHP;
            Debug.Log("Current HP + amount > maxHP so return = " + healedAmount);
            currentHP = maxHP;
            return healedAmount;
        } else {
            Debug.Log("Current HP + amount <= maxHP so return = " + amount);
            currentHP += amount;
            return amount;
        }
    }

    public IEnumerator playHurtAnimation() {
        audioSource.PlayOneShot(hurtSound);
        animator.SetBool("Was Hit", true);
        animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength * animBlendFactor);
        animator.SetBool("Was Hit", false);
        Debug.Log("playHurtAnimation() ended");
    }

    public IEnumerator playDeathAnimation() {
        audioSource.PlayOneShot(deathSound);
        animator.SetBool("Was Hit", true);
        animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength * animBlendFactor);
        animator.SetBool("Dead", true);
    }

    public IEnumerator playAttackAnimation() {
        audioSource.PlayOneShot(attackSound);
        animator.SetBool("Melee Attacking", true);
        //while (animation.IsPlaying("yourAnimation")) {
        //    yield;
        //}
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        //yield return new WaitForSeconds(animLength - animLength * animBlendFactor);
        yield return new WaitForSeconds(animLength * animBlendFactor);
        animator.SetBool("Melee Attacking", false);
    }

    public IEnumerator playSpecialAnimation() {
        audioSource.PlayOneShot(specialSound);
        animator.SetBool("Magic Attacking", true);
        animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength * animBlendFactor);
        animator.SetBool("Magic Attacking", false);
    }

    public int getSpeed() {
        return this.speed;
    }
}