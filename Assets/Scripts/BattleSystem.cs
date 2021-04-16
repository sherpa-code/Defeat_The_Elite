using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSystem : MonoBehaviour {
    
    public Transform allySpawnTransform;
    public Transform enemySpawnTransform;

    public GameObject enemyGameObject;
    public GameObject allyGameObject;

    public Monster allyMonster;
    public Monster enemyMonster;

    public Image playerActions;
    public Image gameOverHUD;
    public Image combatReadout;
    public Text dialogueText;

    public BattleHUDScript allyHUD;
    public BattleHUDScript enemyHUD;

    public Canvas battleCanvas;

    public string playerName;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI enemyNameText;

    public List<Monster> allyTeamList = new List<Monster>() { null, null, null };

    public List<Trainer> enemyTrainers = new List<Trainer>() { null, null, null, null };

    public Trainer currentEnemyTrainer;
    public List<Monster> currentEnemyTeamList = new List<Monster>() { null, null, null };

    public System.Random r = new System.Random();

    public TextMeshProUGUI specialMove;
    public TextMeshProUGUI specialMoveDescriptionText;
    public TextMeshProUGUI specialMoveChargesText;

    //public ItemMenuScript itemMenu;
    public Image itemMenu;

    public AudioManager audioManager;
    public BGMManager BGMManager;

    public static System.Single messageDisplayTime = 2f;
    //public System.Single attackAnimationTime = 1.5f;
    public static System.Single attackAnimationTime = messageDisplayTime*0.75f;

    public Monster lastMonster;
    public Transform lastMonsterTransform;

    public int smallPotionQty = 3; //none of these numbers matter and must be set in the editor instead
    public int largePotionQty = 1;
    public int reviveLeafQty = 1;
    public int antidoteQty = 1;
    public int boostQty = 1;

    public Text smallPotionQtyText;
    public Text largePotionQtyText;
    public Text reviveLeafQtyText;
    public Text antidoteQtyText;
    public Text boostQtyText;

    public void beginGame() {
        BGMManager.playBattleBGM();
        gameObject.SetActive(true);
        battleCanvas.gameObject.SetActive(true);
        enemyHUD.gameObject.SetActive(false);
        allyHUD.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        currentEnemyTrainer = enemyTrainers[r.Next(0, enemyTrainers.Count)];
        enemyNameText.text = currentEnemyTrainer.firstName + " " + currentEnemyTrainer.lastName;
        playerNameText.text = playerName;

        currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);

        //flagMonstersByTeam();
        //updateSpecialMoveChargesText();

        StartCoroutine(beginBattle());
    }

    public IEnumerator beginBattle() {
        dialogueText.text = "The battle begins!";
        //dialogueText.text = "Long multiple line string for testing text wrapping and spacing in the section."; // DEBUG
        yield return new WaitForSeconds(messageDisplayTime);

        spawnAllyMonster(0);
        yield return new WaitForSeconds(messageDisplayTime);

        spawnNextEnemyMonster();
        yield return new WaitForSeconds(messageDisplayTime);

        StartCoroutine(checkSpeedAndContinue());
    }

    public IEnumerator checkSpeedAndContinue() {
        if (isPlayerFaster() == "yes") {
            dialogueText.text = allyMonster.monsterName + " is faster and acts first!";
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(PlayerTurn());
        } else if (isPlayerFaster() == "tie") {
            dialogueText.text = "Both monsters are equally fast!";
            yield return new WaitForSeconds(messageDisplayTime);
            if (r.Next(0, 1) == 0) {
                dialogueText.text = "Your " + allyMonster.monsterName + " acts first!";
                yield return new WaitForSeconds(messageDisplayTime);
                StartCoroutine(PlayerTurn());
            } else {
                dialogueText.text = currentEnemyTrainer.firstName + "'s " + enemyMonster.monsterName + " acts first!";
                yield return new WaitForSeconds(messageDisplayTime);
                StartCoroutine(EnemyTurn());
            }
        } else {
            dialogueText.text = enemyMonster.monsterName + " is faster and acts first!";
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(EnemyTurn());
        }
    }

    public IEnumerator PlayerTurn() {
        bool isDead = false;

        if (allyMonster.isDefending) {
            allyMonster.defense = allyMonster.defense / 2;
            allyMonster.isDefending = false;
            dialogueText.text = allyMonster.monsterName + " stopped defending.\nDefense returned to " + allyMonster.defense + ".";
            combatReadout.gameObject.SetActive(true);
            yield return new WaitForSeconds(messageDisplayTime);
        }

        // Start checking conditions
        if (allyMonster.isPoisoned) {
            dialogueText.text = allyMonster.monsterName + " is still poisoned!";
            combatReadout.gameObject.SetActive(true);
            isDead = allyMonster.TakeDamage(allyMonster.poisonDamageTaken);
            allyHUD.SetHP(allyMonster.currentHP);
            allyMonster.playHurtAnimation();
            yield return new WaitForSeconds(messageDisplayTime);

            if (isDead) {
                StartCoroutine(allyMonsterDied(allyMonster));
                yield break;
            }
            allyMonster.poisonTurnsLeft--;
            if (allyMonster.poisonTurnsLeft <= 0) {
                allyMonster.isPoisoned = false;
                dialogueText.text = "...and the poison wore off!";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        }

        if (allyMonster.isDeathBreathed) {
            dialogueText.text = allyMonster.monsterName + " still smells the Death Breath...";
            yield return new WaitForSeconds(messageDisplayTime);
            combatReadout.gameObject.SetActive(true);
            if (r.Next(0, 10) == 9) {
                isDead = allyMonster.TakeDamage(allyMonster.currentHP); // receive all remaining HP in damage for insta-kill
                dialogueText.text = "...and it was critical!";
                allyHUD.SetHP(allyMonster.currentHP);
                allyMonster.playHurtAnimation();
            } else {
                dialogueText.text = "but " + allyMonster.name + " survived!";
            }
            yield return new WaitForSeconds(messageDisplayTime);

            if (isDead) {
                StartCoroutine(allyMonsterDied(allyMonster));
                yield break;
            }
            allyMonster.deathBreathTurnsLeft--;
            if (allyMonster.deathBreathTurnsLeft <= 0) {
                allyMonster.isDeathBreathed = false;
                dialogueText.text = "...and the Death Breath wore off!";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        }

        //if (allyMonster.isDebuffed) {
        //    dialogueText.text = allyMonster.monsterName + " is still weakened...";
        //    combatReadout.gameObject.SetActive(true);
        //    yield return new WaitForSeconds(messageDisplayTime);
        //    allyMonster.debuffedTurnsLeft--;
        //    if (allyMonster.debuffedTurnsLeft <= 0) {
        //        allyMonster.isDebuffed = false;
        //        dialogueText.text = "...but the weakness wore off!";
        //        yield return new WaitForSeconds(messageDisplayTime);
        //    }
        //}

        //if (allyMonster.isBuffed) {
        //    dialogueText.text = allyMonster.monsterName + " is still buffed by the Power Gem!";
        //    combatReadout.gameObject.SetActive(true);
        //    yield return new WaitForSeconds(messageDisplayTime);
        //    allyMonster.buffedTurnsLeft--;
        //    if (allyMonster.buffedTurnsLeft <= 0) {
        //        allyMonster.isBuffed = false;
        //        dialogueText.text = "...but it finally wore off.";
        //        yield return new WaitForSeconds(messageDisplayTime);
        //    }
        //}

        if (allyMonster.isDebuffed) {
            dialogueText.text = allyMonster.monsterName + " is still weakened...";
            combatReadout.gameObject.SetActive(true);
            yield return new WaitForSeconds(messageDisplayTime);
            allyMonster.debuffedTurnsLeft--;
            if (allyMonster.debuffedTurnsLeft <= 0) {
                allyMonster.isDebuffed = false;
                allyMonster.debuffedAttackAmount = 0;
                allyMonster.debuffedDefenseAmount = 0;
                allyMonster.debuffedSpeedAmount = 0;
                dialogueText.text = "...but the weakness wore off!";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        }

        if (allyMonster.isBuffed) {
            dialogueText.text = allyMonster.monsterName + " is still buffed by the Power Gem!";
            combatReadout.gameObject.SetActive(true);
            yield return new WaitForSeconds(messageDisplayTime);
            allyMonster.buffedTurnsLeft--;
            if (allyMonster.buffedTurnsLeft <= 0) {
                allyMonster.isBuffed = false;
                allyMonster.buffedAttackAmount = 0;
                dialogueText.text = "...but it finally wore off.";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        }

        combatReadout.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(true);
    }

    public IEnumerator PlayerAttack() {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        dialogueText.text = allyMonster.monsterName + " tried to melee attack...";

        StartCoroutine(allyMonster.playAttackAnimation());
        yield return new WaitForSeconds(attackAnimationTime);

        dialogueText.text = "The attack was successful!";
        bool isDead = enemyMonster.TakeDamage(allyMonster.attack);

        enemyHUD.SetHP(enemyMonster.currentHP);

        if (isDead) {
            StartCoroutine(enemyMonster.playDeathAnimation());
            dialogueText.text = enemyMonster.monsterName + " has died!";
            yield return new WaitForSeconds(5f);
            Destroy(enemyGameObject);
            currentEnemyTeamList.RemoveAt(0);
            if (currentEnemyTeamList.Count > 0) { //if enemy trainer has monsters left

                spawnNextEnemyMonster();
                yield return new WaitForSeconds(messageDisplayTime);
                StartCoroutine(checkSpeedAndContinue());
            } else {  // if enemy trainer has no monsters left
                currentEnemyTrainer.isDefeated = true;
                if (allTrainersDefeated()) {
                    GameOverVictory();
                } else { //if there are still trainers undefeated
                    while (currentEnemyTrainer.isDefeated) {
                        currentEnemyTrainer = enemyTrainers[Random.Range(0, enemyTrainers.Count)];
                    }
                    currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);
                    dialogueText.text = currentEnemyTrainer.getFullName() + " wants to battle!";
                    enemyNameText.text = currentEnemyTrainer.getFullName();
                    yield return new WaitForSeconds(messageDisplayTime);
                    spawnNextEnemyMonster();
                    yield return new WaitForSeconds(messageDisplayTime);
                    StartCoroutine(checkSpeedAndContinue());
                }
            }
        } else { //if monster lives
            StartCoroutine(enemyMonster.playHurtAnimation());
            yield return new WaitForSeconds(3f);
            StartCoroutine(EnemyTurn());
        }
    }

    public IEnumerator EnemyTurn() {
        bool isDead = false;
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        if (enemyMonster.isPoisoned) {
            dialogueText.text = enemyMonster.monsterName + " is still poisoned!";
            combatReadout.gameObject.SetActive(true);
            isDead = enemyMonster.TakeDamage(enemyMonster.poisonDamageTaken);
            enemyHUD.SetHP(enemyMonster.currentHP);
            enemyMonster.playHurtAnimation();
            yield return new WaitForSeconds(messageDisplayTime);

            if (isDead) {
                StartCoroutine(enemyMonsterDied(enemyMonster));
                yield break;
            }
            enemyMonster.poisonTurnsLeft--;
            if (enemyMonster.poisonTurnsLeft <= 0) {
                enemyMonster.isPoisoned = false;
                dialogueText.text = "...and the poison wore off!";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        }

        if (enemyMonster.isDeathBreathed) {
            dialogueText.text = enemyMonster.monsterName + " still smells the Death Breath...";
            yield return new WaitForSeconds(messageDisplayTime);
            combatReadout.gameObject.SetActive(true);
            if (r.Next(0, 9) == 9) {
                isDead = enemyMonster.TakeDamage(enemyMonster.currentHP); // receive all remaining HP in damage for insta-kill
                dialogueText.text = "...and it was critical!";
                enemyHUD.SetHP(enemyMonster.currentHP);
                enemyMonster.playHurtAnimation();
            } else {
                dialogueText.text = "but " + enemyMonster.name + " survived!";
            }
            yield return new WaitForSeconds(messageDisplayTime);

            if (isDead) {
                StartCoroutine(enemyMonsterDied(enemyMonster));
                yield break;
            }
            enemyMonster.deathBreathTurnsLeft--;
            if (enemyMonster.deathBreathTurnsLeft <= 0) {
                enemyMonster.isDeathBreathed = false;
                dialogueText.text = "...and the Death Breath wore off!";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        }

        if (enemyMonster.isDebuffed) {
            dialogueText.text = enemyMonster.monsterName + " is still weakened...";
            combatReadout.gameObject.SetActive(true);
            yield return new WaitForSeconds(messageDisplayTime);
            enemyMonster.debuffedTurnsLeft--;
            if (enemyMonster.debuffedTurnsLeft <= 0) {
                enemyMonster.isDebuffed = false;
                dialogueText.text = "...but the weakness wore off!";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        }


        // ACTION BEGINS
        string decision = EnemyDecision();

        if (decision == "melee") {
            dialogueText.text = enemyMonster.monsterName + " attacks...";
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(enemyMonster.playAttackAnimation());
            if (enemyMonster.monsterName.EndsWith("s")) {
                dialogueText.text = enemyMonster.monsterName + "' attack hit!";
            } else {
                dialogueText.text = enemyMonster.monsterName + "'s attack hit!";
            }
            yield return new WaitForSeconds(messageDisplayTime);
            isDead = allyMonster.TakeDamage(enemyMonster.attack);
        } else {
            dialogueText.text = enemyMonster.monsterName + " tries " + enemyMonster.specialAbilityName + "...";
            yield return new WaitForSeconds(messageDisplayTime);
            dialogueText.text = enemyMonster.specialAbilityName + " was successful.";
            yield return new WaitForSeconds(messageDisplayTime);
            if (enemyMonster.specialAbilityName == "Death Breath") {
                dialogueText.text = allyMonster.monsterName + " is at risk of perishing from the smell...";
            } else if (enemyMonster.specialAbilityName == "Seed" || enemyMonster.specialAbilityName == "Sting") {
                dialogueText.text = allyMonster.monsterName + " became poisoned!";
            } else if (enemyMonster.specialAbilityName == "Glare" || enemyMonster.specialAbilityName == "Shriek" || enemyMonster.specialAbilityName == "Intimidate") {
                dialogueText.text = allyMonster.monsterName + "'s attack, defense, and speed have been lowered.";
            }
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(enemyMonster.playSpecialAnimation());
            isDead = allyMonster.TakeDamage(allyMonster.specialDamage);
        }

        enemyHUD.SetHP(enemyMonster.currentHP);
        //yield return new WaitForSeconds(attackAnimationTime);

        if (isDead) {
            lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
            StartCoroutine(allyMonster.playDeathAnimation());
            dialogueText.text = allyMonster.monsterName + " has died!";
            yield return new WaitForSeconds(4f);
            Destroy(allyGameObject);
            allyTeamList.RemoveAt(0);
            if (allyTeamList.Count > 0) {
                yield return new WaitForSeconds(messageDisplayTime);
                spawnAllyMonster(0);        // TODO: Add a way to select 0 or 1, AKA select which of 2 remaining monsters to send out
                yield return new WaitForSeconds(messageDisplayTime);
                StartCoroutine(checkSpeedAndContinue());
            } else {
                yield return new WaitForSeconds(messageDisplayTime);
                GameOverLost();
            }
        } else {
            StartCoroutine(allyMonster.playHurtAnimation());
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(PlayerTurn());
        }
    }

    //public IEnumerator EnemyTurn() {
    //    bool isDead = false;
    //    playerActions.gameObject.SetActive(false);
    //    combatReadout.gameObject.SetActive(true);
        
    //    string decision = EnemyDecision();

    //    if (decision == "melee") {
    //        dialogueText.text = enemyMonster.monsterName + " attacks...";
    //        yield return new WaitForSeconds(messageDisplayTime);
    //        StartCoroutine(enemyMonster.playAttackAnimation());
    //        if (enemyMonster.monsterName.EndsWith("s")) {
    //            dialogueText.text = enemyMonster.monsterName + "' attack hit!";
    //        } else {
    //            dialogueText.text = enemyMonster.monsterName + "'s attack hit!";
    //        }
    //        yield return new WaitForSeconds(messageDisplayTime);
    //        isDead = allyMonster.TakeDamage(enemyMonster.attack);
    //    } else {
    //        dialogueText.text = enemyMonster.monsterName + " tries " + enemyMonster.specialAbilityName + "...";
    //        yield return new WaitForSeconds(messageDisplayTime);
    //        dialogueText.text = enemyMonster.specialAbilityName + " was successful.";
    //        StartCoroutine(enemyMonster.playSpecialAnimation());
    //        isDead = allyMonster.TakeDamage(allyMonster.specialDamage);
    //    }

    //    allyHUD.SetHP(allyMonster.currentHP);
    //    //yield return new WaitForSeconds(attackAnimationTime);

    //    if (isDead) {
    //        lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
    //        StartCoroutine(allyMonster.playDeathAnimation());
    //        dialogueText.text = allyMonster.monsterName + " has died!";
    //        yield return new WaitForSeconds(4f);
    //        Destroy(allyGameObject);
    //        allyTeamList.RemoveAt(0);
    //        if (allyTeamList.Count > 0) {
    //            yield return new WaitForSeconds(messageDisplayTime);
    //            spawnAllyMonster(0);        // TODO: Add a way to select 0 or 1, AKA select which of 2 remaining monsters to send out
    //            yield return new WaitForSeconds(messageDisplayTime);
    //            StartCoroutine(checkSpeedAndContinue());
    //        } else {
    //            yield return new WaitForSeconds(messageDisplayTime);
    //            GameOverLost();
    //        }
    //    } else {
    //        StartCoroutine(allyMonster.playHurtAnimation());
    //        yield return new WaitForSeconds(messageDisplayTime);
    //        StartCoroutine(PlayerTurn());
    //    }
    //}



    public bool needsHeals(Monster monster) {
        //double currentHP = monster.currentHP
        if (monster.currentHP <= 450) { // DEBUG
            //if (monster.currentHP <= (monster.maxHP * 0.6)) { // TODO: test and confirm multiplying by double works here
            return true;
        }
        return false;
    }

    public string EnemyDecision() { // Note: return logic separated out for readabilityuhj
        enemyMonster.needsHeals = needsHeals(enemyMonster);

        if (enemyMonster.needsHeals && enemyMonster.isSpecialHeals) {
            if (healRoll() == 1) {
                Debug.Log("enemy needs heals and is going to use heal special (steelupine)");
                return "special";
            }
        }
        
        if (enemyMonster.isSpecialPoison && !allyMonster.isPoisoned) {
            if (specialRoll() == 1) {
                Debug.Log("enemy has poison special, ally not poisoned, is going to use it (gorimite and spinpion)");
                return "special";
            }
        }
        
        if (enemyMonster.isSpecialDebuff && !allyMonster.isDebuffed) {
            if (specialRoll() == 1) {
                Debug.Log("enemy has debuff, ally not debuffed, is going to use it (dragonewt, quivark, ramodo)");
                return "special";
            }
        }

        if (enemyMonster.isSpecialDeathBreath && !allyMonster.isDeathBreathed) {
            if (specialRoll() == 1) {
                Debug.Log("enemy has death breath, ally not death breathed, is going to use it (crotone)");
                return "special";
            }
        }

        return "melee";
    }

    // TODO: elaborate on this based on what the monster is
    public int healRoll() {
        return r.Next(0, 4);
    }

    public int specialRoll() {
        return r.Next(0, 4);
    }

    public IEnumerator PlayerSpecialAbility() {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        dialogueText.text = allyMonster.monsterName + " tried " + allyMonster.specialAbilityName + "...";
        StartCoroutine(allyMonster.playSpecialAnimation());
        yield return new WaitForSeconds(messageDisplayTime);

        if (allyMonster.isSpecialPoison) {
            if (enemyMonster.isPoisoned) {
                dialogueText.text = "But " + enemyMonster.specialAbilityName + " is already poisoned!";
                yield return new WaitForSeconds(messageDisplayTime);
            } else {
                dialogueText.text = allyMonster.specialAbilityName + " was successful!";
                enemyMonster.playHurtAnimation();
                yield return new WaitForSeconds(messageDisplayTime);
                dialogueText.text = enemyMonster.monsterName + " became poisoned!";
                yield return new WaitForSeconds(messageDisplayTime);
                enemyMonster.isPoisoned = true;
                enemyMonster.poisonDamageTaken = allyMonster.specialPoisonDamage;
                enemyMonster.poisonTurnsLeft = allyMonster.specialPoisonDuration;
            }
        } else if (allyMonster.isSpecialDeathBreath) {
            if (enemyMonster.isDeathBreathed) {
                dialogueText.text = "But " + enemyMonster.specialAbilityName + " already smells the Death Breath!";
                yield return new WaitForSeconds(messageDisplayTime);
            } else {
                dialogueText.text = allyMonster.specialAbilityName + " was successful!";
                enemyMonster.playHurtAnimation();
                yield return new WaitForSeconds(messageDisplayTime);
                dialogueText.text = "Now " + enemyMonster.monsterName + " can smell the Death Breath!";
                yield return new WaitForSeconds(messageDisplayTime);
                enemyMonster.isDeathBreathed = true;
                enemyMonster.deathBreathTurnsLeft = allyMonster.specialDeathBreathDuration;
            }
        } else if (allyMonster.isSpecialHeals) {
            if (allyMonster.currentHP == allyMonster.maxHP) {
                dialogueText.text = "But " + allyMonster.monsterName + " is already at full health!";
                yield return new WaitForSeconds(messageDisplayTime);
            } else {
                dialogueText.text = allyMonster.specialAbilityName + " was successful!";
                yield return new WaitForSeconds(messageDisplayTime);
                int amountHealed = allyMonster.Heal(allyMonster.specialHealsAmount);
                allyHUD.SetHP(allyMonster.currentHP);
                dialogueText.text = allyMonster.monsterName + " healed for " + amountHealed + "HP!";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        } else if (allyMonster.isSpecialDebuff) {
            if (enemyMonster.isDebuffed) {
                dialogueText.text = "But " + enemyMonster.monsterName + " is already weakened!";
                yield return new WaitForSeconds(messageDisplayTime);
            } else {
                enemyMonster.isDebuffed = true;
                enemyMonster.debuffedTurnsLeft = allyMonster.specialDebuffDuration;
                enemyMonster.debuffedAttackAmount = allyMonster.specialDebuffAttackAmount;
                enemyMonster.debuffedDefenseAmount = allyMonster.specialDebuffDefenseAmount;
                enemyMonster.debuffedSpeedAmount = allyMonster.specialDebuffSpeedAmount;
                enemyMonster.updateMyStats();
                dialogueText.text = enemyMonster.monsterName + "'s attack, defense, and speed were lowered!";
                yield return new WaitForSeconds(messageDisplayTime);
            }
        }

        StartCoroutine(EnemyTurn());
    }

    public IEnumerator PlayerDefend() {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        Debug.Log("PlayerDefend()'s NOT allyMonsterDefense = " + allyMonster.defense);
        allyMonster.isDefending = true;
        allyMonster.defense = allyMonster.defense * 2;
        Debug.Log("PlayerDefend()'s updated allyMonsterDefense = " + allyMonster.defense);
        dialogueText.text = allyMonster.monsterName + " is defending.\nDefense raised to " + allyMonster.defense + " until next turn.";
        
        yield return new WaitForSeconds(messageDisplayTime);

        StartCoroutine(EnemyTurn());
    }

    public void PlayerItems() {
        //battleCanvas.GetComponent<ItemMenuScript>().gameObject.SetActive(true);
        Debug.Log("PlayerItems() begins");
        itemMenu.gameObject.SetActive(true);
        updateItemHUD();
        playerActions.gameObject.SetActive(false);
        


        // Hide player actions window
        // Show items list window

        // Wait for user to click an item or cancel
        // if clicked an item, show confirm window
        // if clicked Yes, hide Confirm window and use item
        // if clicked No, hide Confirm window
        // if clicked cancel hide items list window and show player actions window
    }

    private void spawnAllyMonster(int monsterIndex) {
        allyGameObject = Instantiate(allyTeamList[monsterIndex].gameObject, allySpawnTransform.position, allySpawnTransform.rotation);
        allyMonster = allyGameObject.GetComponent<Monster>();
        allyMonster.isPlayerMonster = true;
        allyHUD.SetHUD(allyMonster);
        allyHUD.SetHP(allyMonster.currentHP);
        allyHUD.SetMaxHP(allyMonster.maxHP);
        specialMoveDescriptionText.SetText("(" + allyMonster.specialAbilityName + " - " + allyMonster.specialAbilityDescription.ToString() + ")");
        dialogueText.text = "You sent out " + allyMonster.monsterName + ".";
        allyHUD.gameObject.SetActive(true);
    }

    public void spawnNextEnemyMonster() {
        enemyGameObject = Instantiate(currentEnemyTeamList[0].gameObject, enemySpawnTransform.position, enemySpawnTransform.rotation);
        enemyMonster = enemyGameObject.GetComponent<Monster>();
        enemyHUD.SetHUD(enemyMonster);
        enemyHUD.SetHP(enemyMonster.currentHP);
        enemyHUD.SetMaxHP(enemyMonster.maxHP);
        dialogueText.text = currentEnemyTrainer.firstName + " sent out " + enemyMonster.monsterName + ".";
        enemyHUD.gameObject.SetActive(true);
    }

    public IEnumerator monsterDied(Monster monster) { // TODO: implement for poison and death breath; testing
        StartCoroutine(monster.playDeathAnimation());
        dialogueText.text = monster.monsterName + " has died!";
        yield return new WaitForSeconds(4f);

        if (monster.isPlayerMonster) {
            Debug.Log("Ally monster died");
            lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
            Destroy(allyGameObject);
            allyTeamList.RemoveAt(0);

            if (allyTeamList.Count > 0) {
                spawnAllyMonster(0); //TODO: Add a way to select which of 2 remaining monsters to send out
                yield return new WaitForSeconds(messageDisplayTime);
            } else {
                GameOverLost();
            }
        } else {
            Debug.Log("Enemy monster died");
            lastMonster = Instantiate(currentEnemyTeamList[0], lastMonsterTransform);

            Destroy(enemyGameObject);
            currentEnemyTeamList.RemoveAt(0);

            if (currentEnemyTeamList.Count > 0) {
                spawnNextEnemyMonster(); //TODO: Add a way to select which of 2 remaining monsters to send out
                yield return new WaitForSeconds(messageDisplayTime);
            } else {
                GameOverLost();
            }
        }

        StartCoroutine(checkSpeedAndContinue());
    }

    public IEnumerator allyMonsterDied(Monster monster) {
        Debug.Log("Ally monster died!");
        //StartCoroutine(allyMonster.playDeathAnimation());
        StartCoroutine(monster.playDeathAnimation());
        dialogueText.text = allyMonster.monsterName + " has died!";
        yield return new WaitForSeconds(4f);
        lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
        Destroy(allyGameObject);
        allyTeamList.RemoveAt(0);

        if (allyTeamList.Count > 0) {
            yield return new WaitForSeconds(messageDisplayTime);
            spawnAllyMonster(0); //TODO: Add a way to select which of 2 remaining monsters to send out
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(checkSpeedAndContinue());
        } else {
            GameOverLost();
        }
    }

    public IEnumerator enemyMonsterDied(Monster monster) {
        Debug.Log("Enemy monster died!");
        //StartCoroutine(enemyMonster.playDeathAnimation());
        StartCoroutine(monster.playDeathAnimation());
        dialogueText.text = enemyMonster.monsterName + " has died!";
        yield return new WaitForSeconds(4f);
        lastMonster = Instantiate(currentEnemyTeamList[0], lastMonsterTransform);

        Destroy(enemyGameObject);
        currentEnemyTeamList.RemoveAt(0);

        if (currentEnemyTeamList.Count > 0) {
            yield return new WaitForSeconds(messageDisplayTime);
            spawnNextEnemyMonster(); //TODO: Add a way to select which of 2 remaining monsters to send out
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(checkSpeedAndContinue());
        } else {
            GameOverLost();
        }
    }

    public void GameOverVictory()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        dialogueText.text = "You have defeated all enemy trainers. You win!";
        BGMManager.playVictoryBGM();
        gameOverHUD.gameObject.SetActive(true);
    }

    public void GameOverLost()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        dialogueText.text = "You have no remaining monsters. You lose!";
        BGMManager.playDefeatBGM();
        gameOverHUD.gameObject.SetActive(true);
    }

    public void OnMeleeButton() {
        audioManager.playBlip();

        StartCoroutine(PlayerAttack());
    }

    public void OnSpecialButton() {
        audioManager.playBlip();

        StartCoroutine(PlayerSpecialAbility());
    }

    public void OnDefendButton() {
        audioManager.playBlip();

        StartCoroutine(PlayerDefend());
    }

    public void OnItemButton() {
        audioManager.playBlip();

        PlayerItems();
    }

    public bool allTrainersDefeated() {
        foreach (Trainer trainer in enemyTrainers) {
            if (!trainer.isDefeated) { //if a trainer IS NOT defeated
                return false; //All trainers defeated is false
            }
        }
        return true; //else All trainers are deafeated
    }

    public string isPlayerFaster() {
        if (allyMonster.getSpeed() > enemyMonster.getSpeed()) {
            return "yes";
        } else if (allyMonster.getSpeed() == enemyMonster.getSpeed()) {
            return "tie";
        } else {
            return "no";
        }
    }

    public IEnumerator useSmallLeafPotion() {
        itemMenu.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        dialogueText.text = "You used a small leaf potion on " + allyMonster.monsterName + ".";
        yield return new WaitForSeconds(messageDisplayTime);
        int amountHealed;
        amountHealed = allyMonster.Heal(300);
        allyHUD.SetHP(allyMonster.currentHP);
        dialogueText.text = allyMonster.monsterName + " was healed for " + amountHealed + "HP!";
        yield return new WaitForSeconds(messageDisplayTime);
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator useLargeLeafPotion() {
        itemMenu.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        dialogueText.text = "You used a large potion on " + allyMonster.monsterName + ".";
        yield return new WaitForSeconds(messageDisplayTime);
        allyMonster.currentHP = allyMonster.maxHP;
        allyHUD.SetHP(allyMonster.currentHP);
        dialogueText.text = allyMonster.monsterName + " was healed to max HP!";
        yield return new WaitForSeconds(messageDisplayTime);
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator useReviveLeaf() {
        itemMenu.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        if (allyTeamList.Count == 3) {
            dialogueText.text = "You do not have downed monster. You can't revive anything!";
            yield return new WaitForSeconds(messageDisplayTime);
            combatReadout.gameObject.SetActive(false);
            playerActions.gameObject.SetActive(true);
            yield break;
        } else {
            dialogueText.text = "You used a revive potion on " + lastMonster.monsterName + ". They're back on the team!";
            yield return new WaitForSeconds(messageDisplayTime);
            allyTeamList.Add(lastMonster);
            StartCoroutine(EnemyTurn());
        }
    }

    public IEnumerator useAntidote() {
        itemMenu.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        if (allyMonster.isPoisoned) {
            antidoteQty--;
            allyMonster.isPoisoned = false;
            int healed = allyMonster.Heal(allyMonster.poisonDamageTaken);
            dialogueText.text = allyMonster.monsterName + " has been cured of their poison and restored " + healed + "HP caused by poison.";
            yield return new WaitForSeconds(messageDisplayTime);
            //StartCoroutine(EnemyTurn());
        } else {
            dialogueText.text = allyMonster.monsterName + " is not poisoned, no need to use an antidote!";
            yield return new WaitForSeconds(messageDisplayTime);
            combatReadout.gameObject.SetActive(false);
            playerActions.gameObject.SetActive(true);
            yield break;
        }
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator usePowerGem() {
        itemMenu.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        if (allyMonster.isBuffed) {
            dialogueText.text = allyMonster.monsterName + " is already buffed, you can't buff them twice!";
            yield return new WaitForSeconds(messageDisplayTime);
            combatReadout.gameObject.SetActive(false);
            playerActions.gameObject.SetActive(true);
            yield break;
        } else {
            boostQty--;
            allyMonster.isBuffed = true;
            allyMonster.buffedAttackAmount = 100;
            allyMonster.buffedTurnsLeft = 2;
            allyMonster.updateMyStats();
            dialogueText.text = allyMonster.monsterName + " consumed the Power Gem!\nIt's attack is boosted by " + allyMonster.buffedAttackAmount + " and is now " + allyMonster.attack + ".";
            yield return new WaitForSeconds(messageDisplayTime);
        }
        StartCoroutine(EnemyTurn());
    }

    public void onSmallPotionButton() {
        audioManager.playBlip();
        smallPotionQty--;
        StartCoroutine(useSmallLeafPotion());
    }

    public void onLargePotionButton() {
        audioManager.playBlip();
        largePotionQty--;
        StartCoroutine(useLargeLeafPotion());
    }

    public void onReviveLeafButton() {
        audioManager.playBlip();
        reviveLeafQty--;
        StartCoroutine(useReviveLeaf());
    }

    public void onAntidoteButton() {
        audioManager.playBlip();
        StartCoroutine(useAntidote());
    }

    public void onPowerGemButton() {
        audioManager.playBlip();
        StartCoroutine(usePowerGem());

    }

    //void updateSpecialMoveChargesText() {
    //    specialMoveChargesText.text = "";

    //    for (int i=0; i<allyMonster.specialChargesLeft; i++) {
    //        if (i == 0) {
    //            specialMoveChargesText.text = "SQUARE HERE"; // TODO fix this LiberationSans square or add a font to project and put a the UNICODE square here to show full or empty charge
    //        }
    //        specialMoveChargesText.text += " ";
    //    }

    //}

    public void updateItemHUD() {
        smallPotionQtyText.text = smallPotionQty.ToString();
        largePotionQtyText.text = largePotionQty.ToString(); ;
        reviveLeafQtyText.text = reviveLeafQty.ToString(); ;
        antidoteQtyText.text = antidoteQty.ToString();
        boostQtyText.text = boostQty.ToString();

        if (smallPotionQty == 0) {
            smallPotionQtyText.GetComponentInParent<Button>().interactable = false;
        }
        if (largePotionQty == 0) {
            largePotionQtyText.GetComponentInParent<Button>().interactable = false;
        }
        if (reviveLeafQty == 0) {
            reviveLeafQtyText.GetComponentInParent<Button>().interactable = false;
        }
        if (antidoteQty == 0) {
            antidoteQtyText.GetComponentInParent<Button>().interactable = false;
        }
        if (boostQty == 0) {
            boostQtyText.GetComponentInParent<Button>().interactable = false;
        }
    }
}