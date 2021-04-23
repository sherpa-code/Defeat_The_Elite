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

    public Image itemMenu;

    public AudioManager audioManager;
    public BGMManager BGMManager;

    public static System.Single messageDisplayTime = 2.9f;
    public static System.Single attackAnimationTime = messageDisplayTime * 0.9f;
    public static System.Single hurtAnimationTime = messageDisplayTime * 0.85f;

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
        playerActions.gameObject.SetActive(false);

        //currentEnemyTrainer = enemyTrainers[r.Next(0, enemyTrainers.Count)];
        currentEnemyTrainer = enemyTrainers[2]; // DEBUG
        //0 = Albus Ommin (Steelupine, , )
        //1 = Bloise Sisko (Needles, , )
        //2 = Chun Doom (Spinion, , )
        //3 = Silvanus Reyes (Crotone, , )

        enemyNameText.text = currentEnemyTrainer.firstName + " " + currentEnemyTrainer.lastName;
        playerNameText.text = playerName;

        currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);

        //flagMonstersByTeam();
        //updateSpecialMoveChargesText();

        StartCoroutine(beginBattle());
    }





    public IEnumerator beginBattle() {
        yield return StartCoroutine(displayCombatMessage("The battle begins!"));
        yield return StartCoroutine(spawnAllyMonster(0));
        yield return StartCoroutine(spawnNextEnemyMonster());
        StartCoroutine(checkSpeedAndContinue());
    }

    public IEnumerator checkSpeedAndContinue() {
        if (isPlayerFaster() == "yes") {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is faster and acts first!"));
            StartCoroutine(PlayerTurn());
        } else if (isPlayerFaster() == "tie") {
            yield return StartCoroutine(displayCombatMessage("Both monsters are equally fast!"));
            if (r.Next(0, 2) == 0) {
                yield return StartCoroutine(displayCombatMessage("Your " + allyMonster.monsterName + " acts first!"));
                StartCoroutine(PlayerTurn());
            } else {
                yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + "'s " + enemyMonster.monsterName + " acts first!"));
                StartCoroutine(EnemyTurn());
            }
        } else {
            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " is faster and acts first!"));
            StartCoroutine(EnemyTurn());
        }
    }

    public IEnumerator PlayerTurn() {
        bool isDead = false;
        playerActions.gameObject.SetActive(false);

        if (allyMonster.isDefending) {
            allyMonster.defense = allyMonster.defense / 2;
            allyMonster.isDefending = false;
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " stopped defending.\nDefense returned to " + allyMonster.defense + "."));
        }

        // Start checking conditions
        if (allyMonster.isPoisoned) {
            isDead = allyMonster.TakeDamage(allyMonster.poisonDamageTaken);
            allyHUD.SetHP(allyMonster.currentHP);
            StartCoroutine(allyMonster.playHurtAnimation());
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is still poisoned!"));

            if (isDead) {
                StartCoroutine(allyMonsterDied(allyMonster));
                yield break;
            }
            allyMonster.poisonTurnsLeft--;
            if (allyMonster.poisonTurnsLeft <= 0) {
                allyMonster.poisonTurnsLeft = 0;
                allyMonster.isPoisoned = false;
                yield return StartCoroutine(displayCombatMessage("...and the poison wore off!"));
            }
        }

        if (allyMonster.isDeathBreathed) {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " still smells the Death Breath..."));
            if (r.Next(0, 10) == 8) {
                isDead = true;
                allyHUD.SetHP(0);
                StartCoroutine(allyMonster.playHurtAnimation());
                yield return StartCoroutine(displayCombatMessage("...and it was critical!"));
            } else {
                yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " survived!"));
            }

            if (isDead) {
                yield return StartCoroutine(allyMonsterDied(allyMonster));
                yield break;
            } else {
                allyMonster.deathBreathTurnsLeft--;
                if (allyMonster.deathBreathTurnsLeft <= 0 && !isDead) {
                    allyMonster.deathBreathTurnsLeft = 0;
                    allyMonster.isDeathBreathed = false;
                    yield return StartCoroutine(displayCombatMessage("...and the Death Breath wore off!"));
                }
            }
        }

        if (allyMonster.isDebuffed) {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is still weakened..."));
            allyMonster.debuffedTurnsLeft--;
            if (allyMonster.debuffedTurnsLeft <= 0) {
                allyMonster.debuffedTurnsLeft = 0;
                allyMonster.debuffedAttackAmount = -allyMonster.debuffedAttackAmount;
                allyMonster.debuffedDefenseAmount = -allyMonster.debuffedDefenseAmount;
                allyMonster.debuffedSpeedAmount = -allyMonster.debuffedSpeedAmount;
                allyMonster.isDebuffed = false;
                allyMonster.updateMyStats();
                yield return StartCoroutine(displayCombatMessage("...but the weakness wore off!"));
            }
        }

        if (allyMonster.isBuffed) {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is still buffed by the Power Gem!"));
            allyMonster.buffedTurnsLeft--;
            if (allyMonster.buffedTurnsLeft <= 0) {
                allyMonster.buffedTurnsLeft = 0;
                allyMonster.buffedAttackAmount = -allyMonster.buffedAttackAmount;
                allyMonster.buffedDefenseAmount = -allyMonster.buffedDefenseAmount;
                allyMonster.buffedSpeedAmount = -allyMonster.buffedSpeedAmount;
                allyMonster.isBuffed = false;
                allyMonster.updateMyStats();
                yield return StartCoroutine(displayCombatMessage("...but it finally wore off."));
            }
        }

        combatReadout.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(true);
    }

    public IEnumerator monsterDied(Monster monster) { // TODO: implement for poison and death breath; testing
        StartCoroutine(monster.playDeathAnimation());
        yield return StartCoroutine(displayCombatMessage(monster.monsterName + " has died!"));

        if (monster.isAllyMonster) {
            //Debug.Log("Ally monster died");
            lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
            //Destroy(allyGameObject);
            //allyTeamList.RemoveAt(0); // TODO: check if this is logical order to fix bug
            allyTeamList.RemoveAt(0);
            Destroy(allyGameObject);

            if (allyTeamList.Count > 0) {
                yield return StartCoroutine(spawnAllyMonster(0)); //TODO: Add a way to select which of 2 remaining monsters to send out
                StartCoroutine(checkSpeedAndContinue());
            } else {
                GameOverLost();
            }
        } else {
            //Debug.Log("Enemy monster died");
            lastMonster = Instantiate(currentEnemyTeamList[0], lastMonsterTransform);
            //Destroy(enemyGameObject);
            //currentEnemyTeamList.RemoveAt(0); // TODO: check if this is logical order to fix bug
            currentEnemyTeamList.RemoveAt(0);
            Destroy(enemyGameObject);

            if (currentEnemyTeamList.Count > 0) {
                yield return StartCoroutine(spawnNextEnemyMonster()); //TODO: Add a way to select which of 2 remaining monsters to send out
                //yield return new WaitForSeconds(messageDisplayTime);
                StartCoroutine(checkSpeedAndContinue());
            } else {
                GameOverLost();
            }
        }
    }



    public IEnumerator TakeDamage(Monster monster) { yield return new WaitForSeconds(0f); } // DEBUG placeholder

    public IEnumerator PlayerAttack() {
        StartCoroutine(allyMonster.playAttackAnimation());
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " tried to melee attack..."));
        enemyMonster.TakeDamage(allyMonster.attack);
        enemyHUD.SetHP(enemyMonster.currentHP);
        StartCoroutine(enemyMonster.playHurtAnimation());
        yield return StartCoroutine(displayCombatMessage("The attack was successful!"));
        yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " took " + enemyMonster.lastDamageTaken + " damage!"));

        if (enemyMonster.currentHP <= 0) {
            StartCoroutine(enemyMonster.playDeathAnimation());
            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " has died!"));
            Destroy(enemyGameObject);
            currentEnemyTeamList.RemoveAt(0);
            if (currentEnemyTeamList.Count > 0) { //if enemy trainer has monsters left
                yield return StartCoroutine(spawnNextEnemyMonster());
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
                    yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.getFullName() + " wants to battle!"));
                    yield return StartCoroutine(spawnNextEnemyMonster());
                    StartCoroutine(checkSpeedAndContinue());
                }
            }
        } else { //if monster lives
            StartCoroutine(EnemyTurn());
        }
    }

    //public IEnumerator IsPoisoned(Monster monster) { yield return new WaitForSeconds(0f); }

    public IEnumerator IsPoisoned(Monster monster) { yield return new WaitForSeconds(0f); }

    public IEnumerator EnemyTurn() {
        bool isDead = false;
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        if (enemyMonster.isPoisoned) {
            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " is still poisoned!"));
            isDead = enemyMonster.TakeDamage(enemyMonster.poisonDamageTaken);
            enemyHUD.SetHP(enemyMonster.currentHP);
            //StartCoroutine(enemyMonster.playHurtAnimation());
            yield return StartCoroutine(enemyMonster.playHurtAnimation());
            yield return new WaitForSeconds(messageDisplayTime);

            if (isDead) {
                //StartCoroutine(enemyMonsterDied(enemyMonster));
                yield return StartCoroutine(enemyMonsterDied());
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
            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " still smells the Death Breath..."));
            if (r.Next(0, 9) == 9) {
                isDead = enemyMonster.TakeDamage(enemyMonster.currentHP); // receive all remaining HP in damage for insta-kill
                //dialogueText.text = "...and it was critical!";
                enemyHUD.SetHP(enemyMonster.currentHP);
                StartCoroutine(enemyMonster.playHurtAnimation());
                yield return StartCoroutine(displayCombatMessage("...and it was critical!"));
            } else {
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " survived!"));
            }

            if (isDead) {
                //StartCoroutine(enemyMonsterDied(enemyMonster));
                yield return StartCoroutine(enemyMonsterDied());
                yield break;
            }
            enemyMonster.deathBreathTurnsLeft--;
            if (enemyMonster.deathBreathTurnsLeft <= 0) {
                enemyMonster.isDeathBreathed = false;
                yield return StartCoroutine(displayCombatMessage("...and the Death Breath wore off!"));
            }
        }

        if (enemyMonster.isDebuffed) {
            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " is still weakened..."));
            enemyMonster.debuffedTurnsLeft--;
            if (enemyMonster.debuffedTurnsLeft <= 0) {
                enemyMonster.debuffedAttackAmount = -enemyMonster.debuffedAttackAmount;
                enemyMonster.debuffedDefenseAmount = -enemyMonster.debuffedDefenseAmount;
                enemyMonster.debuffedSpeedAmount = -enemyMonster.debuffedSpeedAmount;
                enemyMonster.isDebuffed = false;
                enemyMonster.updateMyStats();
                yield return StartCoroutine(displayCombatMessage("...but "+enemyMonster.monsterName + " regained its strength!"));
            }
        }

        // ACTION BEGINS
        string decision = EnemyDecision();
        //decision = "special"; // DEBUG

        if (decision == "melee") {
            StartCoroutine(enemyMonster.playAttackAnimation());
            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " attacks..."));
            if (enemyMonster.monsterName.EndsWith("s")) {
                yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + "' attack hit!"));
            } else {
                yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + "'s attack hit!"));
            }
            isDead = allyMonster.TakeDamage(enemyMonster.attack);
            allyHUD.SetHP(allyMonster.currentHP);
            StartCoroutine(allyMonster.playHurtAnimation());
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " took " + allyMonster.lastDamageTaken + " damage!"));
            
        } else {
            StartCoroutine(enemyMonster.playSpecialAnimation());
            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " tries " + enemyMonster.specialAbilityName + "..."));
            if (enemyMonster.isSpecialDeathBreath) {
                if (allyMonster.isDeathBreathed) {
                    yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " already smells the Death Breath!"));
                } else {
                    StartCoroutine(allyMonster.playHurtAnimation());
                    yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful!"));
                    allyMonster.deathBreathTurnsLeft = enemyMonster.specialDeathBreathDuration;
                    allyMonster.isDeathBreathed = true;
                    yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is at risk of perishing from the smell..."));
                }
            } else if (enemyMonster.isSpecialPoison) {
                if (allyMonster.isSpecialPoison) {
                    yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " is immune to poison!"));
                } else if (allyMonster.isPoisoned) {
                    yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " is already poisoned!"));
                } else {
                    StartCoroutine(allyMonster.playHurtAnimation());
                    yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful!"));
                    allyMonster.poisonDamageTaken = enemyMonster.specialPoisonDamage;
                    allyMonster.poisonTurnsLeft = enemyMonster.specialPoisonDuration;
                    allyMonster.isPoisoned = true;
                    yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " became poisoned!"));
                }
            } else if (enemyMonster.isSpecialDebuff) {
                if (allyMonster.isDebuffed) {
                    yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " is still debuffed!"));
                } else {
                    yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful!"));
                    allyMonster.debuffedTurnsLeft = enemyMonster.specialDebuffDuration;
                    allyMonster.debuffedAttackAmount = enemyMonster.specialDebuffAttackAmount;
                    allyMonster.debuffedDefenseAmount = enemyMonster.specialDebuffDefenseAmount;
                    allyMonster.debuffedSpeedAmount = enemyMonster.specialDebuffSpeedAmount;
                    allyMonster.isDebuffed = true;
                    yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + "'s attack, defense, and speed\nwere lowered!"));
                }
            } else {
                StartCoroutine(allyMonster.playHurtAnimation());
                isDead = allyMonster.TakeDamage(enemyMonster.specialDamage);
                allyHUD.SetHP(allyMonster.currentHP);
                yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful..."));
            }
        }

        if (isDead) {
            lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
            StartCoroutine(allyMonster.playDeathAnimation());
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " has died!"));
            allyTeamList.RemoveAt(0);
            Destroy(allyGameObject);
            
            if (allyTeamList.Count > 0) {
                //yield return new WaitForSeconds(messageDisplayTime);
                yield return StartCoroutine(spawnAllyMonster(0));        // TODO: Add a way to select 0 or 1, AKA select which of 2 remaining monsters to send out
                yield return new WaitForSeconds(messageDisplayTime);
                StartCoroutine(checkSpeedAndContinue());
            } else {
                yield return new WaitForSeconds(messageDisplayTime);
                GameOverLost();
            }
        } else {
            StartCoroutine(PlayerTurn());
        }
    }


    public bool needsHeals(Monster monster) {
        //double currentHP = monster.currentHP
        if (monster.currentHP <= 450) { // DEBUG
            //if (monster.currentHP <= (monster.maxHP * 0.6)) { // TODO: test and confirm multiplying by double works here
            return true;
        }
        return false;
    }

    public string EnemyDecision() {
        if (needsHeals(enemyMonster) && enemyMonster.isSpecialHeals) { // TODO: add check for special charges remaining
            //if (healRoll() == 1) {
            //    Debug.Log("enemy needs heals and is going to use heal special (steelupine)");
            //    return "special";
            //}
            return "special";
        } else if (enemyMonster.isSpecialPoison && !allyMonster.isPoisoned) {
            //Debug.Log("SPECIAL IS POISON AND ENEMY ISNT");
            if (specialRoll() == 1) {
                //Debug.Log("enemy has poison special, ally not poisoned, is going to use it (gorimite and spinpion)");
                return "special";
            }
        } else if (enemyMonster.isSpecialDebuff && !allyMonster.isDebuffed) {
            if (specialRoll() == 1) {
                //Debug.Log("enemy has debuff, ally not debuffed, is going to use it (dragonewt, quivark, ramodo)");
                return "special";
            }
        } else if (enemyMonster.isSpecialDeathBreath && !allyMonster.isDeathBreathed) {
            if (specialRoll() == 1) {
                //Debug.Log("enemy has death breath, ally not death breathed, is going to use it (crotone)");
                return "special";
            }
        } else {
            if (specialRoll() == 1) {
                //Debug.Log("ordinary attack special");
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
        return r.Next(0, 3);
    }

    public IEnumerator PlayerSpecialAbility() {
        bool isDead = false;
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        StartCoroutine(allyMonster.playSpecialAnimation());
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " tried " + allyMonster.specialAbilityName + "..."));
        //yield return StartCoroutine(allyMonster.playSpecialAnimation());

        if (allyMonster.isSpecialPoison) {
            if (enemyMonster.isSpecialPoison) {
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " is immune to poison!"));
                //StartCoroutine(EnemyTurn());
            } else if (enemyMonster.isPoisoned) {
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " is already poisoned!"));
                //StartCoroutine(EnemyTurn());
            } else {
                yield return StartCoroutine(displayCombatMessage(allyMonster.specialAbilityName + " was successful!"));
                //StartCoroutine(enemyMonster.playHurtAnimation());
                //dialogueText.text = enemyMonster.monsterName + " became poisoned!";
                StartCoroutine(enemyMonster.playHurtAnimation());
                yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " became poisoned!"));
                //yield return new WaitForSeconds(messageDisplayTime);
                enemyMonster.isPoisoned = true;
                enemyMonster.poisonDamageTaken = allyMonster.specialPoisonDamage;
                enemyMonster.poisonTurnsLeft = allyMonster.specialPoisonDuration;
                //StartCoroutine(EnemyTurn());
            }
        } else if (allyMonster.isSpecialDeathBreath) {
            if (enemyMonster.isDeathBreathed) {
                //dialogueText.text = "...but " + enemyMonster.monsterName + " already smells the Death Breath!";
                //yield return new WaitForSeconds(messageDisplayTime);
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " already smells the Death Breath!"));
                //StartCoroutine(EnemyTurn());
            } else {
                dialogueText.text = allyMonster.specialAbilityName + " was successful!";
                //StartCoroutine(enemyMonster.playHurtAnimation());
                yield return StartCoroutine(enemyMonster.playHurtAnimation());
                //yield return new WaitForSeconds(messageDisplayTime);
                //dialogueText.text = "Now " + enemyMonster.monsterName + " can smell the Death Breath!";
                //yield return new WaitForSeconds(messageDisplayTime);
                yield return StartCoroutine(displayCombatMessage("Now " + enemyMonster.monsterName + " can smell the Death Breath!"));
                enemyMonster.isDeathBreathed = true;
                enemyMonster.deathBreathTurnsLeft = allyMonster.specialDeathBreathDuration;
                //StartCoroutine(EnemyTurn());
            }
        } else if (allyMonster.isSpecialHeals) {
            if (allyMonster.currentHP == allyMonster.maxHP) {
                //dialogueText.text = "...but " + allyMonster.monsterName + " is already at full health!";
                //yield return new WaitForSeconds(messageDisplayTime);
                yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " is already at full health!"));
                //StartCoroutine(EnemyTurn());
            } else {
                //dialogueText.text = allyMonster.specialAbilityName + " was successful!";
                //yield return new WaitForSeconds(messageDisplayTime);
                int amountHealed = allyMonster.Heal(allyMonster.specialHealsAmount);
                allyHUD.SetHP(allyMonster.currentHP);
                yield return StartCoroutine(displayCombatMessage(allyMonster.specialAbilityName + " was successful!"));
                yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " healed for " + amountHealed + "HP!"));
                //StartCoroutine(EnemyTurn());
            }
        } else if (allyMonster.isSpecialDebuff) {
            if (enemyMonster.isDebuffed) {
                //dialogueText.text = "...but " + enemyMonster.monsterName + " is already weakened!";
                //yield return new WaitForSeconds(messageDisplayTime);
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " is already weakened!"));
                //StartCoroutine(EnemyTurn());
            } else {
                enemyMonster.isDebuffed = true;
                enemyMonster.debuffedTurnsLeft = allyMonster.specialDebuffDuration;
                enemyMonster.debuffedAttackAmount = allyMonster.specialDebuffAttackAmount;
                enemyMonster.debuffedDefenseAmount = allyMonster.specialDebuffDefenseAmount;
                enemyMonster.debuffedSpeedAmount = allyMonster.specialDebuffSpeedAmount;
                enemyMonster.updateMyStats();
                //dialogueText.text = enemyMonster.monsterName + "'s attack, defense, and speed were lowered!";
                //yield return new WaitForSeconds(messageDisplayTime);
                yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + "'s attack, defense, and speed were lowered!"));
                //StartCoroutine(EnemyTurn());
            }
        } else {
            //Debug.Log("Was a normal attack special");
            isDead = enemyMonster.TakeDamage(allyMonster.specialDamage);
            enemyHUD.SetHP(enemyMonster.currentHP);
            StartCoroutine(enemyMonster.playHurtAnimation());
            yield return StartCoroutine(displayCombatMessage(allyMonster.specialAbilityName + " was successful!"));
            
            if (isDead) {
                //StartCoroutine(enemyMonster.playDeathAnimation());
                //yield return StartCoroutine(enemyMonster.playDeathAnimation());
                yield return StartCoroutine(enemyMonsterDied());
                //dialogueText.text = enemyMonster.monsterName + " has died!";
                //yield return new WaitForSeconds(5f);
                yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " has died!"));
                Destroy(enemyGameObject);
                currentEnemyTeamList.RemoveAt(0);
                if (currentEnemyTeamList.Count > 0) { //if enemy trainer has monsters left

                    yield return StartCoroutine(spawnNextEnemyMonster());
                    //yield return new WaitForSeconds(messageDisplayTime);
                    StartCoroutine(checkSpeedAndContinue());
                    yield break;
                } else {  // if enemy trainer has no monsters left
                    currentEnemyTrainer.isDefeated = true;
                    if (allTrainersDefeated()) {
                        GameOverVictory();
                        yield break;
                    } else { //if there are still trainers undefeated
                        while (currentEnemyTrainer.isDefeated) {
                            currentEnemyTrainer = enemyTrainers[Random.Range(0, enemyTrainers.Count)];
                        }
                        currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);
                        yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.getFullName() + " wants to battle!"));
                        //dialogueText.text = currentEnemyTrainer.getFullName() + " wants to battle!";
                        //enemyNameText.text = currentEnemyTrainer.getFullName();
                        //yield return new WaitForSeconds(messageDisplayTime);
                        yield return StartCoroutine(spawnNextEnemyMonster());
                        //yield return new WaitForSeconds(messageDisplayTime);
                        StartCoroutine(checkSpeedAndContinue());
                        yield break;
                    }
                }
            }
        }
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator PlayerDefend() {
        //Debug.Log("PlayerDefend()'s NOT allyMonsterDefense = " + allyMonster.defense);
        allyMonster.isDefending = true;
        allyMonster.defense = allyMonster.defense * 2;
        //Debug.Log("PlayerDefend()'s updated allyMonsterDefense = " + allyMonster.defense);
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is defending.\nDefense raised to " + allyMonster.defense + " until next turn."));
        StartCoroutine(EnemyTurn());
    }

    public void PlayerItems() {
        //battleCanvas.GetComponent<ItemMenuScript>().gameObject.SetActive(true);
        //Debug.Log("PlayerItems() begins");
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

    //private void spawnAllyMonster(int monsterIndex) {
    //    allyGameObject = Instantiate(allyTeamList[monsterIndex].gameObject, allySpawnTransform.position, allySpawnTransform.rotation);
    //    allyMonster = allyGameObject.GetComponent<Monster>();
    //    allyMonster.isAllyMonster = true;
    //    allyHUD.SetHUD(allyMonster);
    //    allyHUD.SetHP(allyMonster.currentHP);
    //    allyHUD.SetMaxHP(allyMonster.maxHP);
    //    specialMoveDescriptionText.SetText("(" + allyMonster.specialAbilityName + " - " + allyMonster.specialAbilityDescription.ToString() + ")");
    //    dialogueText.text = "You sent out " + allyMonster.monsterName + ".";
    //    allyHUD.gameObject.SetActive(true);
    //}

    public IEnumerator spawnAllyMonster(int monsterIndex) {
        allyGameObject = Instantiate(allyTeamList[monsterIndex].gameObject, allySpawnTransform.position, allySpawnTransform.rotation);
        allyMonster = allyGameObject.GetComponent<Monster>();
        allyMonster.isAllyMonster = true;
        allyHUD.SetHUD(allyMonster);
        allyHUD.SetHP(allyMonster.currentHP);
        allyHUD.SetMaxHP(allyMonster.maxHP);
        specialMoveDescriptionText.SetText("(" + allyMonster.specialAbilityName + " - " + allyMonster.specialAbilityDescription.ToString() + ")");
        //dialogueText.text = "You sent out " + allyMonster.monsterName + ".";
        allyHUD.gameObject.SetActive(true);
        yield return StartCoroutine(displayCombatMessage("You sent out " + allyMonster.monsterName + "."));
    }

    public IEnumerator spawnNextEnemyMonster() {
        enemyGameObject = Instantiate(currentEnemyTeamList[0].gameObject, enemySpawnTransform.position, enemySpawnTransform.rotation);
        enemyMonster = enemyGameObject.GetComponent<Monster>();
        enemyHUD.SetHUD(enemyMonster);
        enemyHUD.SetHP(enemyMonster.currentHP);
        enemyHUD.SetMaxHP(enemyMonster.maxHP);
        enemyHUD.gameObject.SetActive(true);
        yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + " sent out " + enemyMonster.monsterName + "."));
    }

    public IEnumerator allyMonsterDied(Monster monster) {
        StartCoroutine(monster.playDeathAnimation());
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " has died!"));
        lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
        Destroy(allyGameObject);
        allyTeamList.RemoveAt(0);

        if (allyTeamList.Count > 0) {
            //yield return new WaitForSeconds(messageDisplayTime);
            yield return StartCoroutine(spawnAllyMonster(0)); //TODO: Add a way to select which of 2 remaining monsters to send out
            //yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(checkSpeedAndContinue());
        } else {
            GameOverLost();
        }
    }

    //public IEnumerator enemyMonsterDied(Monster monster) {
    public IEnumerator enemyMonsterDied() {
        //Debug.Log("Enemy monster died!");
        //yield return StartCoroutine(enemyMonster.playDeathAnimation());
        StartCoroutine(enemyMonster.playDeathAnimation());
        lastMonster = Instantiate(currentEnemyTeamList[0], lastMonsterTransform);
        //dialogueText.text = enemyMonster.monsterName + " has died!";
        yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " has died!"));
        //lastMonster = Instantiate(currentEnemyTeamList[0], lastMonsterTransform);
        //yield return new WaitForSeconds(messageDisplayTime);

        Destroy(enemyGameObject);
        currentEnemyTeamList.RemoveAt(0);

        if (currentEnemyTeamList.Count > 0) {
            yield return StartCoroutine(spawnNextEnemyMonster()); //TODO: Add a way to select which of 2 remaining monsters to send out
            StartCoroutine(checkSpeedAndContinue());
        } else {
            GameOverLost();
        }
    }
    
    public void switchToUI(string ui) {
        if (ui == "actions") {
            playerActions.gameObject.SetActive(true);
            combatReadout.gameObject.SetActive(false);
        } else if (ui == "readout") {
            playerActions.gameObject.SetActive(false);
            combatReadout.gameObject.SetActive(true);
        }
    }

    public void GameOverVictory() {
        switchToUI("readout");

        dialogueText.text = "You have defeated all enemy trainers. You win!";
        BGMManager.playVictoryBGM();
        gameOverHUD.gameObject.SetActive(true);
    }

    public void GameOverLost() {
        switchToUI("readout");

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

    public void OnItemMenuExitButton() { // TODO: add to functionality
        audioManager.playBlip();
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
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        //audioManager.playBlip();
        //dialogueText.text = "You used a small leaf potion on " + allyMonster.monsterName + ".";
        //yield return new WaitForSeconds(messageDisplayTime);
        yield return StartCoroutine(displayCombatMessage("You used a small potion on " + allyMonster.monsterName + "."));
        int amountHealed;
        amountHealed = allyMonster.Heal(300);
        allyHUD.SetHP(allyMonster.currentHP);
        //dialogueText.text = allyMonster.monsterName + " was healed for " + amountHealed + "HP!";
        //yield return new WaitForSeconds(messageDisplayTime);
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " was healed for " + amountHealed + "HP!"));
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator useLargeLeafPotion() {
        itemMenu.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        //dialogueText.text = "You used a large potion on " + allyMonster.monsterName + ".";
        //yield return new WaitForSeconds(messageDisplayTime);
        yield return StartCoroutine(displayCombatMessage("You used a large potion on " + allyMonster.monsterName + "."));
        allyMonster.currentHP = allyMonster.maxHP;
        allyHUD.SetHP(allyMonster.currentHP);
        //dialogueText.text = allyMonster.monsterName + " was healed to max HP!";
        //yield return new WaitForSeconds(messageDisplayTime);
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " was healed to max HP!"));
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator useReviveLeaf() {
        itemMenu.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        if (allyTeamList.Count == 3) {
            //dialogueText.text = "You do not have downed monster. You can't revive anything!";
            //yield return new WaitForSeconds(messageDisplayTime);
            yield return StartCoroutine(displayCombatMessage("You do not have downed monster. You can't revive anything!"));
            switchToUI("actions");
            //combatReadout.gameObject.SetActive(false);
            //playerActions.gameObject.SetActive(true);
            yield break;
        } else {
            //dialogueText.text = "You used a revive potion on " + lastMonster.monsterName + ". They're back on the team!";
            //yield return new WaitForSeconds(messageDisplayTime);
            yield return StartCoroutine(displayCombatMessage("You used a revive potion on " + lastMonster.monsterName + ". They're back on the team!"));
            allyTeamList.Add(lastMonster);
            StartCoroutine(EnemyTurn());
        }
    }

    public IEnumerator useAntidote() {
        itemMenu.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        if (allyMonster.isPoisoned) {
            antidoteQty--;
            allyMonster.isPoisoned = false;
            int healed = allyMonster.Heal(allyMonster.poisonDamageTaken);
            //dialogueText.text = allyMonster.monsterName + " has been cured of their poison and restored " + healed + "HP caused by poison.";
            //yield return new WaitForSeconds(messageDisplayTime);
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " has been cured of their poison and restored " + healed + "HP caused by poison."));
            //StartCoroutine(EnemyTurn());
        } else {
            //dialogueText.text = allyMonster.monsterName + " is not poisoned, no need to use an antidote!";
            //yield return new WaitForSeconds(messageDisplayTime);
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is not poisoned, no need to use an antidote!"));
            switchToUI("actions");
            //combatReadout.gameObject.SetActive(false);
            //playerActions.gameObject.SetActive(true);
            yield break;
        }
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator usePowerGem() {
        itemMenu.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        if (allyMonster.isBuffed) {
            //dialogueText.text = allyMonster.monsterName + " is already buffed, you can't buff them twice!";
            //yield return new WaitForSeconds(messageDisplayTime);
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is already buffed, you can't buff them twice!"));
            //combatReadout.gameObject.SetActive(false);
            //playerActions.gameObject.SetActive(true);
            switchToUI("actions");
            yield break;
        } else {
            boostQty--;
            allyMonster.isBuffed = true;
            allyMonster.buffedAttackAmount = 120;
            allyMonster.buffedTurnsLeft = 4;
            allyMonster.updateMyStats();
            //dialogueText.text = allyMonster.monsterName + " consumed the Power Gem!\nIt's attack is boosted by " + allyMonster.buffedAttackAmount + " and is now " + allyMonster.attack + ".";
            //yield return new WaitForSeconds(messageDisplayTime);
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " consumed the Power Gem!\nIt's attack is boosted by " + allyMonster.buffedAttackAmount + " and is now " + allyMonster.attack + "."));
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

    public IEnumerator displayCombatMessage(string message) {
        switchToUI("readout");

        dialogueText.text = message;
        //dialogueText.text = "Long multiple line string for testing text wrapping and spacing in the section."; // DEBUG
        yield return new WaitForSeconds(messageDisplayTime);
    }
}


