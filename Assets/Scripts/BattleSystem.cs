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
    public Monster actingMonster;

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

    //public static System.Single messageDisplayTime = 3.1f;
    public static System.Single messageDisplayTime = 2f; // DEBUG
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
        switchToUI("readout");

        //currentEnemyTrainer = enemyTrainers[r.Next(0, enemyTrainers.Count)];
        currentEnemyTrainer = enemyTrainers[0]; // DEBUG
        //0 = Albus Ommin (Dragonewt, , ) // starts with debuff
        //1 = Bloise Sisko (Needles, , ) // starts with almost fastest
        //2 = Chun Doom (Spinion, , ) // starts with fastest and poison
        //3 = Silvanus Reyes (Crotone, , ) // starts with death breath

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
        string isPlayerFaster = IsPlayerFaster();
        if (isPlayerFaster == "yes") {
            actingMonster = allyMonster;
            yield return StartCoroutine(displayCombatMessage("Your " + allyMonster.monsterName + " is faster and acts first!"));
        } else if (isPlayerFaster == "tie") {
            yield return StartCoroutine(displayCombatMessage("Both monsters are equally fast!"));
            if (r.Next(0, 2) == 0) {
                actingMonster = allyMonster;
                yield return StartCoroutine(displayCombatMessage("Your " + allyMonster.monsterName + " acts first!"));
            } else {
                actingMonster = enemyMonster;
                if (currentEnemyTrainer.firstName.EndsWith("s")) {
                    yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + "' " + enemyMonster.monsterName + " acts first!"));
                } else {
                    yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + "'s " + enemyMonster.monsterName + " acts first!"));
                }
            }
        } else {
            actingMonster = enemyMonster;
            if (currentEnemyTrainer.firstName.EndsWith("s")) {
                yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + "' " + enemyMonster.monsterName + " acts first!"));
            } else {
                yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + "'s " + enemyMonster.monsterName + " acts first!"));
            }
        }
        StartCoroutine(TrainerTurn());
    }

    public void SwapActingMonster() {
        if (actingMonster.isAllyMonster) {
            actingMonster = enemyMonster;
        } else {
            actingMonster = allyMonster;
        }
    }

    public IEnumerator TrainerTurn() { // TODO implement entirely
        switchToUI("readout");
        //yield return StartCoroutine(WaitForSeconds(0f));
        if (actingMonster.isDefending) {
            actingMonster.defense = actingMonster.defense / 2;
            actingMonster.isDefending = false;
            yield return StartCoroutine(displayCombatMessage(actingMonster.monsterName + " stopped defending.\nDefense returned to " + actingMonster.defense + "."));
        }

        yield return StartCoroutine(MonsterPoisoned(actingMonster));

        if (actingMonster.currentHP <= 0) {
            yield return StartCoroutine(MonsterDied(actingMonster));
            yield break;
        } else { // is alive
            if (actingMonster.poisonTurnsLeft > 0) {
                actingMonster.poisonTurnsLeft--;
                if (actingMonster.poisonTurnsLeft == 0) {
                    actingMonster.isPoisoned = false;
                    yield return StartCoroutine(displayCombatMessage("...and the poison wore off!"));
                }
            }
        }

        yield return StartCoroutine(MonsterDeathBreathed(actingMonster));

        if (actingMonster.currentHP <= 0) {
            yield return StartCoroutine(MonsterDied(actingMonster));
            yield break;
        } else {
            if (actingMonster.deathBreathTurnsLeft > 0) {
                actingMonster.deathBreathTurnsLeft--;
                if (actingMonster.deathBreathTurnsLeft == 0) {
                    actingMonster.isDeathBreathed = false;
                    yield return StartCoroutine(displayCombatMessage("...and the Death Breath wore off!"));
                }
            }
        }

        yield return StartCoroutine(MonsterDebuffed(actingMonster));

        yield return StartCoroutine(MonsterBuffed(actingMonster));

        if (actingMonster.isAllyMonster) {
            switchToUI("actions");
        } else {
            StartCoroutine(EnemyAction());
            //StartCoroutine(); TODO: new coroutine for enemy decisions only
        }
    }

    public IEnumerator EnemyAction() {
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
            //isDead = allyMonster.TakeDamage(enemyMonster.attack);
            allyMonster.TakeDamage(enemyMonster.attack);
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
                    allyMonster.deathBreathTurnsLeft = enemyMonster.specialDeathBreathDuration;
                    allyMonster.isDeathBreathed = true;
                    StartCoroutine(allyMonster.playHurtAnimation());
                    yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful!"));
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
                //isDead = allyMonster.TakeDamage(enemyMonster.specialDamage);
                allyMonster.TakeDamage(enemyMonster.specialDamage);
                allyHUD.SetHP(allyMonster.currentHP);
                yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful..."));
            }
        }

        if (enemyMonster.currentHP <= 0) {
            //if (isDead) {
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
            SwapActingMonster();
            StartCoroutine(TrainerTurn());
            //StartCoroutine(PlayerTurn());
        }
    }

    public IEnumerator MonsterDied(Monster monster) {
        StartCoroutine(monster.playDeathAnimation());
        yield return StartCoroutine(displayCombatMessage(monster.monsterName + " has died!"));
        if (monster.isAllyMonster) {
            lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
            allyTeamList.RemoveAt(0); Destroy(allyGameObject);
            if (allyTeamList.Count > 0) {
                yield return StartCoroutine(spawnAllyMonster(0)); //TODO: Add a way to select which of 2 remaining monsters to send out
                StartCoroutine(checkSpeedAndContinue());
            } else {
                GameOverLost();
            }
        } else {
            currentEnemyTeamList.RemoveAt(0);
            Destroy(enemyGameObject);

            if (currentEnemyTeamList.Count > 0) {
                yield return StartCoroutine(spawnNextEnemyMonster()); //TODO: Add a way to select which of 2 remaining monsters to send out
                StartCoroutine(checkSpeedAndContinue());
            } else {
                GameOverVictory();
            }
        }
    }

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
                if (AllTrainersDefeated()) {
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
            SwapActingMonster();
            StartCoroutine(TrainerTurn());
            //StartCoroutine(EnemyTurn());
        }
    }

    //public IEnumerator IsDeadCheck(Monster monster) {
    //    if (isDead) {
    //        if (monster.isAllyMonster) {
    //            StartCoroutine(allyMonsterDied(allyMonster));
    //            yield break;
    //        } else {
    //            //if (monster.currentHP <= 0) {
    //            //StartCoroutine(enemyMonsterDied(enemyMonster));
    //            yield return StartCoroutine(enemyMonsterDied());
    //            yield break;
    //        }
    //    } else {
    //        if (monster.isAllyMonster) {
    //            allyMonster.poisonTurnsLeft--;
    //            if (allyMonster.poisonTurnsLeft <= 0) {
    //                allyMonster.poisonTurnsLeft = 0;
    //                allyMonster.isPoisoned = false;
    //                yield return StartCoroutine(displayCombatMessage("...and the poison wore off!"));
    //            }
    //        } else {
    //            enemyMonster.poisonTurnsLeft--;
    //            if (enemyMonster.poisonTurnsLeft <= 0) {
    //                enemyMonster.poisonTurnsLeft = 0;
    //                enemyMonster.isPoisoned = false;
    //                yield return StartCoroutine(displayCombatMessage("...and the poison wore off!"));
    //                //dialogueText.text = "...and the poison wore off!";
    //                //yield return new WaitForSeconds(messageDisplayTime);
    //            }
    //        }
    //    }
    //}

    //public IEnumerator IsPoisoned(Monster monster) { yield return new WaitForSeconds(0f); }
    





    public IEnumerator MonsterPoisoned(Monster monster) {
        if (monster.isPoisoned) {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " is still poisoned!"));
            monster.TakeDamage(monster.poisonDamageTaken);
            if (monster.isAllyMonster) {
                allyHUD.SetHP(monster.currentHP);
            } else {
                enemyHUD.SetHP(monster.currentHP);
            }
            
            //StartCoroutine(enemyMonster.playHurtAnimation());
            StartCoroutine(monster.playHurtAnimation());
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " took " + monster.lastDamageTaken + " damage from poison!"));
        }
    }

    public IEnumerator MonsterDeathBreathed(Monster monster) {
        if (monster.isDeathBreathed) {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " still smells the Death Breath..."));
            //if (r.Next(0, 10) == 9) {
            if (r.Next(9, 10) == 9) { // DEBUG: always crit
                monster.TakeDamage(monster.currentHP);
                if (monster.isAllyMonster) {
                    allyHUD.SetHP(monster.currentHP);
                } else {
                    enemyHUD.SetHP(monster.currentHP);
                }
                StartCoroutine(monster.playHurtAnimation());
                yield return StartCoroutine(displayCombatMessage("...and it was critical!"));
            } else {
                yield return StartCoroutine(displayCombatMessage("...but " + monster.monsterName + " survived!"));
            }
        }
    }

    public IEnumerator MonsterBuffed(Monster monster) {
        if (monster.isBuffed) {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " is still buffed by the Power Gem!"));
            monster.buffedTurnsLeft--;
            if (monster.buffedTurnsLeft <= 0) {
                monster.buffedTurnsLeft = 0;
                monster.buffedAttackAmount = -monster.buffedAttackAmount;
                monster.buffedDefenseAmount = -monster.buffedDefenseAmount;
                monster.buffedSpeedAmount = -monster.buffedSpeedAmount;
                monster.isBuffed = false;
                monster.updateMyStats();
                yield return StartCoroutine(displayCombatMessage("...but it finally wore off."));
            }
        }
    }

    public IEnumerator MonsterDebuffed(Monster monster) {
        if (monster.isDebuffed) {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " is still weakened..."));
            monster.debuffedTurnsLeft--;
            if (monster.debuffedTurnsLeft <= 0) {
                monster.debuffedAttackAmount = -monster.debuffedAttackAmount;
                monster.debuffedDefenseAmount = -monster.debuffedDefenseAmount;
                monster.debuffedSpeedAmount = -monster.debuffedSpeedAmount;
                monster.isDebuffed = false;
                monster.updateMyStats();
                yield return StartCoroutine(displayCombatMessage("...but " + monster.monsterName + " regained its strength!"));
            }
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

    public int specialRoll() {
        return r.Next(0, 3);
    }

    public IEnumerator PlayerSpecialAbility() {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        StartCoroutine(allyMonster.playSpecialAnimation());
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " tried " + allyMonster.specialAbilityName + "..."));

        if (allyMonster.isSpecialPoison) {
            if (enemyMonster.isSpecialPoison) {
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " is immune to poison!"));
            } else if (enemyMonster.isPoisoned) {
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " is already poisoned!"));
            } else {
                StartCoroutine(enemyMonster.playHurtAnimation());
                yield return StartCoroutine(displayCombatMessage(allyMonster.specialAbilityName + " was successful!"));
                enemyMonster.isPoisoned = true;
                enemyMonster.poisonDamageTaken = allyMonster.specialPoisonDamage;
                enemyMonster.poisonTurnsLeft = allyMonster.specialPoisonDuration;
                yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " became poisoned!"));
            }
        } else if (allyMonster.isSpecialDeathBreath) {
            if (enemyMonster.isDeathBreathed) {
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " already smells the Death Breath!"));
            } else {
                yield return StartCoroutine(displayCombatMessage(allyMonster.specialAbilityName + " was successful!"));
                enemyMonster.isDeathBreathed = true;
                enemyMonster.deathBreathTurnsLeft = allyMonster.specialDeathBreathDuration;
                yield return StartCoroutine(displayCombatMessage("Now " + enemyMonster.monsterName + " can smell the Death Breath!"));
            }
        } else if (allyMonster.isSpecialHeals) {
            if (allyMonster.currentHP >= allyMonster.maxHP) {
                yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " is already at full health!"));
            } else {
                int amountHealed = allyMonster.Heal(allyMonster.specialHealsAmount);
                allyHUD.SetHP(allyMonster.currentHP);
                yield return StartCoroutine(displayCombatMessage(allyMonster.specialAbilityName + " was successful!"));
                yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " healed for " + amountHealed + "HP!"));
            }
        } else if (allyMonster.isSpecialDebuff) {
            if (enemyMonster.isDebuffed) {
                yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " is already weakened!"));
            } else {
                enemyMonster.isDebuffed = true;
                enemyMonster.debuffedTurnsLeft = allyMonster.specialDebuffDuration;
                enemyMonster.debuffedAttackAmount = allyMonster.specialDebuffAttackAmount;
                enemyMonster.debuffedDefenseAmount = allyMonster.specialDebuffDefenseAmount;
                enemyMonster.debuffedSpeedAmount = allyMonster.specialDebuffSpeedAmount;
                enemyMonster.updateMyStats();
                yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + "'s attack, defense, and speed were lowered!"));
            }
        } else {
            enemyMonster.TakeDamage(allyMonster.specialDamage);
            enemyHUD.SetHP(enemyMonster.currentHP);
            StartCoroutine(enemyMonster.playHurtAnimation());
            yield return StartCoroutine(displayCombatMessage(allyMonster.specialAbilityName + " was successful!"));
            
            if (enemyMonster.currentHP <= 0) {
                yield return StartCoroutine(MonsterDied(enemyMonster));
                //Destroy(enemyGameObject);
                //currentEnemyTeamList.RemoveAt(0);
                if (currentEnemyTeamList.Count > 0) { //if enemy trainer has monsters left
                    yield return StartCoroutine(spawnNextEnemyMonster());
                    //yield return new WaitForSeconds(messageDisplayTime);
                    StartCoroutine(checkSpeedAndContinue());
                    yield break;
                } else {  // if enemy trainer has no monsters left
                    currentEnemyTrainer.isDefeated = true;
                    if (AllTrainersDefeated()) {
                        GameOverVictory();
                        yield break;
                    } else { //if there are still trainers undefeated
                        while (currentEnemyTrainer.isDefeated) {
                            currentEnemyTrainer = enemyTrainers[Random.Range(0, enemyTrainers.Count)];
                        }
                        currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);
                        yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.getFullName() + " wants to battle!"));
                        yield return StartCoroutine(spawnNextEnemyMonster());
                        StartCoroutine(checkSpeedAndContinue());
                        yield break;
                    }
                }
            }
        }
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
        //StartCoroutine(EnemyTurn());
    }

    public IEnumerator PlayerDefend() {
        allyMonster.isDefending = true;
        allyMonster.defense = allyMonster.defense * 2;
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is defending.\nDefense raised to " + allyMonster.defense + " until next turn."));
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
        //StartCoroutine(EnemyTurn());
    }

    public void PlayerItems() {
        itemMenu.gameObject.SetActive(true);
        updateItemHUD();
        playerActions.gameObject.SetActive(false);
    }

    public IEnumerator spawnAllyMonster(int monsterIndex) {
        allyGameObject = Instantiate(allyTeamList[monsterIndex].gameObject, allySpawnTransform.position, allySpawnTransform.rotation);
        allyMonster = allyGameObject.GetComponent<Monster>();
        allyMonster.isAllyMonster = true;
        allyHUD.SetHUD(allyMonster);
        allyHUD.SetHP(allyMonster.currentHP);
        allyHUD.SetMaxHP(allyMonster.maxHP);
        specialMoveDescriptionText.SetText("(" + allyMonster.specialAbilityName + " - " + allyMonster.specialAbilityDescription.ToString() + ")");
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

    public void OnItemMenuExitButton() {
        audioManager.playBlip();
    }

    public bool AllTrainersDefeated() {
        foreach (Trainer trainer in enemyTrainers) {
            if (!trainer.isDefeated) { //if a trainer IS NOT defeated
                return false; //All trainers defeated is false
            }
        }
        return true; //else All trainers are deafeated
    }

    public string IsPlayerFaster() {
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
        audioManager.playBlip();
        yield return StartCoroutine(displayCombatMessage("You used a small potion on " + allyMonster.monsterName + "."));
        int amountHealed;
        amountHealed = allyMonster.Heal(300);
        allyHUD.SetHP(allyMonster.currentHP);
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " was healed for " + amountHealed + "HP!"));
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
        //StartCoroutine(EnemyTurn());
    }

    public IEnumerator useLargeLeafPotion() {
        itemMenu.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        yield return StartCoroutine(displayCombatMessage("You used a large potion on " + allyMonster.monsterName + "."));
        allyMonster.currentHP = allyMonster.maxHP;
        allyHUD.SetHP(allyMonster.currentHP);
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " was healed to max HP!"));
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
        //StartCoroutine(EnemyTurn());
    }

    public IEnumerator useReviveLeaf() {
        itemMenu.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        if (allyTeamList.Count == 3) {
            yield return StartCoroutine(displayCombatMessage("You do not have downed monster. You can't revive anything!"));
            switchToUI("actions");
            yield break;
        } else {
            yield return StartCoroutine(displayCombatMessage("You used a revive potion on " + lastMonster.monsterName + ". They're back on the team!"));
            allyTeamList.Add(lastMonster);
            SwapActingMonster();
            StartCoroutine(TrainerTurn());
            //StartCoroutine(EnemyTurn());
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
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " has been cured of their poison and restored " + healed + "HP caused by poison."));
        } else {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is not poisoned, no need to use an antidote!"));
            switchToUI("actions");
            yield break;
        }
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
        //StartCoroutine(EnemyTurn());
    }

    public IEnumerator usePowerGem() {
        itemMenu.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        audioManager.playBlip();
        if (allyMonster.isBuffed) {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is already buffed, you can't buff them twice!"));
            switchToUI("actions");
            yield break;
        } else {
            boostQty--;
            allyMonster.isBuffed = true;
            allyMonster.buffedAttackAmount = 120;
            allyMonster.buffedTurnsLeft = 4;
            allyMonster.updateMyStats();
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " consumed the Power Gem!\nIt's attack is boosted by " + allyMonster.buffedAttackAmount + " and is now " + allyMonster.attack + "."));
        }
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
        //StartCoroutine(EnemyTurn());
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

    //void updateSpecialMoveChargesText() {
    //    specialMoveChargesText.text = "";

    //    for (int i=0; i<allyMonster.specialChargesLeft; i++) {
    //        if (i == 0) {
    //            specialMoveChargesText.text = "SQUARE HERE"; // TODO fix this LiberationSans square or add a font to project and put a the UNICODE square here to show full or empty charge
    //        }
    //        specialMoveChargesText.text += " ";
    //    }

    //}

    //public IEnumerator EnemyTurn() {
    //    switchToUI("readout");

    //    yield return StartCoroutine(MonsterPoisoned(enemyMonster));

    //    if (enemyMonster.isPoisoned) {
    //        if (enemyMonster.currentHP <= 0) {
    //            yield return StartCoroutine(enemyMonsterDied());
    //            yield break;
    //        }
    //        enemyMonster.poisonTurnsLeft--;
    //        if (enemyMonster.poisonTurnsLeft <= 0) {
    //            enemyMonster.isPoisoned = false;
    //            yield return StartCoroutine(displayCombatMessage("...and the poison wore off!"));
    //        }
    //    }

    //    yield return StartCoroutine(MonsterDeathBreathed(enemyMonster));

    //    if (enemyMonster.isDeathBreathed) {
    //        if (enemyMonster.currentHP <= 0) {
    //            yield return StartCoroutine(enemyMonsterDied());
    //            yield break;
    //        }
    //        enemyMonster.deathBreathTurnsLeft--;
    //        if (enemyMonster.deathBreathTurnsLeft <= 0) {
    //            enemyMonster.isDeathBreathed = false;
    //            yield return StartCoroutine(displayCombatMessage("...and the Death Breath wore off!"));
    //        }
    //    }


    //    yield return StartCoroutine(MonsterDebuffed(enemyMonster)); // ADD BACK

    //    //if (enemyMonster.isDebuffed) {
    //    //    yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " is still weakened..."));
    //    //    enemyMonster.debuffedTurnsLeft--;
    //    //    if (enemyMonster.debuffedTurnsLeft <= 0) {
    //    //        enemyMonster.debuffedAttackAmount = -enemyMonster.debuffedAttackAmount;
    //    //        enemyMonster.debuffedDefenseAmount = -enemyMonster.debuffedDefenseAmount;
    //    //        enemyMonster.debuffedSpeedAmount = -enemyMonster.debuffedSpeedAmount;
    //    //        enemyMonster.isDebuffed = false;
    //    //        enemyMonster.updateMyStats();
    //    //        yield return StartCoroutine(displayCombatMessage("...but " + enemyMonster.monsterName + " regained its strength!"));
    //    //    }
    //    //}

    //    // ACTION BEGINS
    //    string decision = EnemyDecision();
    //    //decision = "special"; // DEBUG

    //    if (decision == "melee") {
    //        StartCoroutine(enemyMonster.playAttackAnimation());
    //        yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " attacks..."));
    //        if (enemyMonster.monsterName.EndsWith("s")) {
    //            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + "' attack hit!"));
    //        } else {
    //            yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + "'s attack hit!"));
    //        }
    //        //isDead = allyMonster.TakeDamage(enemyMonster.attack);
    //        allyMonster.TakeDamage(enemyMonster.attack);
    //        allyHUD.SetHP(allyMonster.currentHP);
    //        StartCoroutine(allyMonster.playHurtAnimation());
    //        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " took " + allyMonster.lastDamageTaken + " damage!"));

    //    } else {
    //        StartCoroutine(enemyMonster.playSpecialAnimation());
    //        yield return StartCoroutine(displayCombatMessage(enemyMonster.monsterName + " tries " + enemyMonster.specialAbilityName + "..."));
    //        if (enemyMonster.isSpecialDeathBreath) {
    //            if (allyMonster.isDeathBreathed) {
    //                yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " already smells the Death Breath!"));
    //            } else {
    //                allyMonster.deathBreathTurnsLeft = enemyMonster.specialDeathBreathDuration;
    //                allyMonster.isDeathBreathed = true;
    //                StartCoroutine(allyMonster.playHurtAnimation());
    //                yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful!"));
    //                yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is at risk of perishing from the smell..."));
    //            }
    //        } else if (enemyMonster.isSpecialPoison) {
    //            if (allyMonster.isSpecialPoison) {
    //                yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " is immune to poison!"));
    //            } else if (allyMonster.isPoisoned) {
    //                yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " is already poisoned!"));
    //            } else {
    //                StartCoroutine(allyMonster.playHurtAnimation());
    //                yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful!"));
    //                allyMonster.poisonDamageTaken = enemyMonster.specialPoisonDamage;
    //                allyMonster.poisonTurnsLeft = enemyMonster.specialPoisonDuration;
    //                allyMonster.isPoisoned = true;
    //                yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " became poisoned!"));
    //            }
    //        } else if (enemyMonster.isSpecialDebuff) {
    //            if (allyMonster.isDebuffed) {
    //                yield return StartCoroutine(displayCombatMessage("...but " + allyMonster.monsterName + " is still debuffed!"));
    //            } else {
    //                yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful!"));
    //                allyMonster.debuffedTurnsLeft = enemyMonster.specialDebuffDuration;
    //                allyMonster.debuffedAttackAmount = enemyMonster.specialDebuffAttackAmount;
    //                allyMonster.debuffedDefenseAmount = enemyMonster.specialDebuffDefenseAmount;
    //                allyMonster.debuffedSpeedAmount = enemyMonster.specialDebuffSpeedAmount;
    //                allyMonster.isDebuffed = true;
    //                yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + "'s attack, defense, and speed\nwere lowered!"));
    //            }
    //        } else {
    //            StartCoroutine(allyMonster.playHurtAnimation());
    //            //isDead = allyMonster.TakeDamage(enemyMonster.specialDamage);
    //            allyMonster.TakeDamage(enemyMonster.specialDamage);
    //            allyHUD.SetHP(allyMonster.currentHP);
    //            yield return StartCoroutine(displayCombatMessage(enemyMonster.specialAbilityName + " was successful..."));
    //        }
    //    }

    //    if (enemyMonster.currentHP <= 0) {
    //        //if (isDead) {
    //        lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
    //        StartCoroutine(allyMonster.playDeathAnimation());
    //        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " has died!"));
    //        allyTeamList.RemoveAt(0);
    //        Destroy(allyGameObject);

    //        if (allyTeamList.Count > 0) {
    //            //yield return new WaitForSeconds(messageDisplayTime);
    //            yield return StartCoroutine(spawnAllyMonster(0));        // TODO: Add a way to select 0 or 1, AKA select which of 2 remaining monsters to send out
    //            yield return new WaitForSeconds(messageDisplayTime);
    //            StartCoroutine(checkSpeedAndContinue());
    //        } else {
    //            yield return new WaitForSeconds(messageDisplayTime);
    //            GameOverLost();
    //        }
    //    } else {
    //        SwapActingMonster();
    //        StartCoroutine(TrainerTurn());
    //        //StartCoroutine(PlayerTurn());
    //    }
    //}
}