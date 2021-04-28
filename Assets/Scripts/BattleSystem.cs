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
    public Monster waitingMonster;

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
    //public static System.Single messageDisplayTime = 1.5f; // DEBUG
    public static System.Single attackAnimationTime = messageDisplayTime * 0.9f;
    public static System.Single hurtAnimationTime = messageDisplayTime * 0.85f;
    public static System.Single fadeUpTime = 2.5f;
    public static System.Single fadeOutTime = 4f;

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

    public ParticleManagerScript particleManager;
    public GameObject currentParticle;
    //public static System.Single destroyTime;

    public void beginGame() {
        BGMManager.playBattleBGM();
        gameObject.SetActive(true);
        battleCanvas.gameObject.SetActive(true);
        enemyHUD.gameObject.SetActive(false);
        allyHUD.gameObject.SetActive(false);
        switchToUI("readout");

        currentEnemyTrainer = enemyTrainers[r.Next(0, enemyTrainers.Count)];
        //currentEnemyTrainer = enemyTrainers[3]; // DEBUG
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
            waitingMonster = enemyMonster;
            yield return StartCoroutine(displayCombatMessage("Your " + allyMonster.monsterName + " is faster and acts first!"));
        } else if (isPlayerFaster == "tie") {
            yield return StartCoroutine(displayCombatMessage("Both monsters are equally fast!"));
            if (r.Next(0, 2) == 0) {
                actingMonster = allyMonster;
                waitingMonster = enemyMonster;
                yield return StartCoroutine(displayCombatMessage("Your " + allyMonster.monsterName + " acts first!"));
            } else {
                actingMonster = enemyMonster;
                waitingMonster = allyMonster;
                if (currentEnemyTrainer.firstName.EndsWith("s")) {
                    yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + "' " + enemyMonster.monsterName + " acts first!"));
                } else {
                    yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + "'s " + enemyMonster.monsterName + " acts first!"));
                }
            }
        } else {
            actingMonster = enemyMonster;
            waitingMonster = allyMonster;
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
            waitingMonster = allyMonster;
        } else {
            actingMonster = allyMonster;
            waitingMonster = enemyMonster;
        }
    }

    public IEnumerator TrainerTurn() { // TODO implement entirely
        //GameObject firework = Instantiate(FireworksAll, position, Quaternion.identity);
        //currentParticle = Instantiate(particlePoison, allySpawnTransform.position, Quaternion.identity);
        //yield return StartCoroutine(particleManager.PlayAllParticles(allyMonster));
        //yield return StartCoroutine(particleManager.PlayAllParticles(enemyMonster)); // DEBUG

        for (int i=0; i < 47; i++) {

        }
        switchToUI("readout");
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
        }
    }

    public IEnumerator MonsterAttack(Monster monster) {
        StartCoroutine(monster.playAttackAnimation());
        yield return StartCoroutine(displayCombatMessage(monster.monsterName + " attacks..."));
        waitingMonster.TakeDamage(monster.attack);
        StartCoroutine(waitingMonster.playHurtAnimation());

        if (monster.isAllyMonster) {
            enemyHUD.SetHP(enemyMonster.currentHP);
        } else {
            allyHUD.SetHP(allyMonster.currentHP);
        }

        if (monster.monsterName.EndsWith("s")) {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + "' attack hit!"));
        } else {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + "'s attack hit!"));
        }


        yield return StartCoroutine(displayCombatMessage(waitingMonster.monsterName + " took " + waitingMonster.lastDamageTaken + " damage!"));
    }

    public IEnumerator MonsterDefend(Monster monster) {
        yield return new WaitForSeconds(0f);
    }

    public IEnumerator EnemyAction() {
        string decision = EnemyDecision();

        if (decision == "melee") {
            yield return StartCoroutine(MonsterAttack(actingMonster));
        } else {
            yield return StartCoroutine(MonsterSpecial(actingMonster));
        }

        if (allyMonster.currentHP <= 0) {
            yield return StartCoroutine(MonsterDied(allyMonster));
        } else {
            SwapActingMonster();
            StartCoroutine(TrainerTurn());
        }
    }

    public IEnumerator MonsterDied(Monster monster) {
        StartCoroutine(monster.playDeathAnimation());
        StartCoroutine(displayCombatMessage(monster.monsterName + " has died!"));
        currentParticle = Instantiate(particleManager.ball2, waitingMonster.transform.position, Quaternion.identity);
        yield return StartCoroutine(DestroyParticle(3.5f));
        if (monster.isAllyMonster) {

            lastMonster = Instantiate(allyTeamList[0], lastMonsterTransform);
            allyTeamList.RemoveAt(0);
            Destroy(allyGameObject);
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
        yield return StartCoroutine(MonsterAttack(allyMonster));
        if (enemyMonster.currentHP <= 0) {
            yield return StartCoroutine(MonsterDied(enemyMonster));
        } else { //if monster lives
            SwapActingMonster();
            StartCoroutine(TrainerTurn());
        }
    }

    public IEnumerator MonsterPoisoned(Monster monster) {
        if (monster.isPoisoned) {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " is still poisoned!"));
            monster.TakeDirectDamage(monster.poisonDamageTaken);
            if (monster.isAllyMonster) {
                allyHUD.SetHP(monster.currentHP);
            } else {
                enemyHUD.SetHP(monster.currentHP);
            }
            StartCoroutine(monster.playHurtAnimation());
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " took " + monster.lastDamageTaken + " damage from poison!"));
        }
    }

    public IEnumerator MonsterDeathBreathed(Monster monster) {
        if (monster.isDeathBreathed) {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " still smells the Death Breath..."));
            if (r.Next(0, 9) == 8) {
            //if (r.Next(9, 10) == 9) { // DEBUG: always crit
                monster.TakeDirectDamage(monster.currentHP);
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
                monster.updateMyStats();
                monster.buffedAttackAmount = 0;
                monster.buffedDefenseAmount = 0;
                monster.buffedSpeedAmount = 0;
                monster.updateMyStats();
                monster.isBuffed = false;
                yield return StartCoroutine(displayCombatMessage("...but it finally wore off."));
            }
        }
    }

    public IEnumerator MonsterDebuffed(Monster monster) {
        if (monster.isDebuffed) {
            yield return StartCoroutine(displayCombatMessage(monster.monsterName + " is still weakened..."));
            monster.debuffedTurnsLeft--;
            if (monster.debuffedTurnsLeft <= 0) {
                monster.debuffedTurnsLeft = 0;
                monster.debuffedAttackAmount = -monster.debuffedAttackAmount;
                monster.debuffedDefenseAmount = -monster.debuffedDefenseAmount;
                monster.debuffedSpeedAmount = -monster.debuffedSpeedAmount;
                monster.updateMyStats();
                monster.debuffedAttackAmount = 0;
                monster.debuffedDefenseAmount = 0;
                monster.debuffedSpeedAmount = 0;
                monster.updateMyStats();
                monster.isDebuffed = false;
                yield return StartCoroutine(displayCombatMessage("...but " + monster.monsterName + " regained its strength!"));
            }
        }
    }

    public string EnemyDecision() {
        enemyMonster.UpdateNeedsHeals();
        int specialRoll = SpecialRoll();
        if (enemyMonster.isSpecialHeals) {
            if (enemyMonster.needsHeals) {
                return "special";
            }
        } else if (enemyMonster.isSpecialPoison) {
            if (!waitingMonster.isPoisoned) {
               if (specialRoll == 1) {
                    return "special";
                }
            }
        } else if (enemyMonster.isSpecialDebuff) {
            if (!waitingMonster.isDebuffed) {
                if (specialRoll == 1) {
                    return "special";
                }
            }
        } else if (enemyMonster.isSpecialDeathBreath) {
            if (!waitingMonster.isDeathBreathed) {
                if (specialRoll == 1) {
                    return "special";
                }
            }
        } else {
            if (specialRoll == 1) {
                return "special";
            }
        }
        return "melee";
    }

    public int SpecialRoll() {
        return r.Next(0, 4);
    }

    public int SpecialHitRoll() {
        return r.Next(0, 9);
    }

    public IEnumerator DestroyParticle(System.Single destroyTime) {
        yield return new WaitForSeconds(destroyTime);
        Destroy(currentParticle);
    }

    public IEnumerator MonsterSpecial(Monster monster) {
        switchToUI("readout");
        StartCoroutine(monster.playSpecialAnimation());
        yield return StartCoroutine(displayCombatMessage(monster.monsterName + " tries " + monster.specialAbilityName + "..."));
        if (monster.isSpecialDeathBreath) {
            if (waitingMonster.isDeathBreathed) {
                yield return StartCoroutine(displayCombatMessage("...but " + waitingMonster.monsterName + " already smells the Death Breath!"));
            } else {
                currentParticle = Instantiate(particleManager.portal3, waitingMonster.transform.position, Quaternion.identity);
                StartCoroutine(DestroyParticle(3.5f));
                waitingMonster.deathBreathTurnsLeft = monster.specialDeathBreathDuration;
                waitingMonster.isDeathBreathed = true;
                StartCoroutine(waitingMonster.playHurtAnimation());
                yield return StartCoroutine(displayCombatMessage(monster.specialAbilityName + " was successful!"));
                yield return StartCoroutine(displayCombatMessage(waitingMonster.monsterName + " is at risk of perishing from the smell..."));
            }
        } else if (monster.isSpecialPoison) {
            Debug.Log("monster isSpecialPoison");
            //Debug.Log(");
            if (waitingMonster.isSpecialPoison) {
                yield return StartCoroutine(displayCombatMessage("...but " + waitingMonster.monsterName + " is immune to poison!"));
            } else if (waitingMonster.isPoisoned) {
                yield return StartCoroutine(displayCombatMessage("...but " + waitingMonster.monsterName + " is already poisoned!"));
            } else {
                //currentParticle = Instantiate(partparticlePoison, allySpawnTransform.position, Quaternion.identity);
                currentParticle = Instantiate(particleManager.ball, waitingMonster.transform.position, Quaternion.identity);
                StartCoroutine(DestroyParticle(3f));
                StartCoroutine(waitingMonster.playHurtAnimation());
                yield return StartCoroutine(displayCombatMessage(monster.specialAbilityName + " was successful!"));
                waitingMonster.poisonDamageTaken = monster.specialPoisonDamage;
                waitingMonster.poisonTurnsLeft = monster.specialPoisonDuration;
                waitingMonster.isPoisoned = true;
                yield return StartCoroutine(displayCombatMessage(waitingMonster.monsterName + " became poisoned!"));
            }
        } else if (monster.isSpecialDebuff) {
            if (waitingMonster.isDebuffed) {
                yield return StartCoroutine(displayCombatMessage("...but " + waitingMonster.monsterName + " is already weakened!"));
            } else {
                currentParticle = Instantiate(particleManager.laserBombardment2, waitingMonster.transform.position, Quaternion.identity);
                StartCoroutine(DestroyParticle(4f));
                yield return StartCoroutine(displayCombatMessage(monster.specialAbilityName + " was successful!"));
                waitingMonster.debuffedTurnsLeft = monster.specialDebuffDuration;
                waitingMonster.debuffedAttackAmount = monster.specialDebuffAttackAmount;
                waitingMonster.debuffedDefenseAmount = monster.specialDebuffDefenseAmount;
                waitingMonster.debuffedSpeedAmount = monster.specialDebuffSpeedAmount;
                waitingMonster.isDebuffed = true;
                waitingMonster.updateMyStats();
                yield return StartCoroutine(displayCombatMessage(waitingMonster.monsterName + "'s attack, defense, and speed\nwere lowered!"));
            }
        } else {
            int roll = SpecialHitRoll();
            if (roll > 0) {
                currentParticle = Instantiate(particleManager.explosion, waitingMonster.transform.position, Quaternion.identity);
                StartCoroutine(DestroyParticle(3f));
                StartCoroutine(waitingMonster.playHurtAnimation());
                waitingMonster.TakeDamage(monster.specialDamage);
                if (waitingMonster.isAllyMonster) {
                    allyHUD.SetHP(waitingMonster.currentHP);
                } else {
                    enemyHUD.SetHP(waitingMonster.currentHP);
                }
                yield return StartCoroutine(displayCombatMessage(monster.specialAbilityName + " was successful..."));
                yield return StartCoroutine(displayCombatMessage(monster.monsterName + " took " + monster.lastDamageTaken + " damage!"));
            } else {
                yield return StartCoroutine(displayCombatMessage("... but it missed!"));
            }
        }
    }

    public IEnumerator PlayerSpecialAbility() {
        yield return StartCoroutine(MonsterSpecial(allyMonster));
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
    }

    public IEnumerator PlayerDefend() {
        allyMonster.isDefending = true;
        allyMonster.defense = allyMonster.defense * 2;
        yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is defending.\nDefense raised to " + allyMonster.defense + " until next turn."));
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
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
        currentParticle = Instantiate(particleManager.cube, allyMonster.transform.position, Quaternion.identity);
        StartCoroutine(displayCombatMessage("You sent out " + allyMonster.monsterName + "."));
        yield return StartCoroutine(DestroyParticle(4.2f));
        //yield return StartCoroutine(displayCombatMessage("You sent out " + allyMonster.monsterName + "."));
    }

    public IEnumerator spawnNextEnemyMonster() {
        enemyGameObject = Instantiate(currentEnemyTeamList[0].gameObject, enemySpawnTransform.position, enemySpawnTransform.rotation);
        enemyMonster = enemyGameObject.GetComponent<Monster>();
        enemyHUD.SetHUD(enemyMonster);
        enemyHUD.SetHP(enemyMonster.currentHP);
        enemyHUD.SetMaxHP(enemyMonster.maxHP);
        enemyHUD.gameObject.SetActive(true);
        currentParticle = Instantiate(particleManager.cube, enemyMonster.transform.position, Quaternion.identity);
        StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + " sent out " + enemyMonster.monsterName + "."));
        yield return StartCoroutine(DestroyParticle(4.2f));
        //yield return StartCoroutine(displayCombatMessage(currentEnemyTrainer.firstName + " sent out " + enemyMonster.monsterName + "."));
    }
    
    public void switchToUI(string ui) {
        if (ui == "actions") {
            playerActions.gameObject.SetActive(true);
            combatReadout.gameObject.SetActive(false);
            itemMenu.gameObject.SetActive(false);
        } else if (ui == "readout") {
            playerActions.gameObject.SetActive(false);
            combatReadout.gameObject.SetActive(true);
            itemMenu.gameObject.SetActive(false);
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
            if (!trainer.isDefeated) {
                return false; // All trainers defeated is false
            }
        }
        return true; // All trainers are defeated
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

    public IEnumerator UsePotionSmall() {
        Debug.Log("Use small potion");
        audioManager.playBlip();
        Debug.Log("current = " + actingMonster.currentHP);
        Debug.Log("max = " + actingMonster.maxHP);
        itemMenu.gameObject.SetActive(false);
        if (actingMonster.currentHP >= actingMonster.maxHP) {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is already at full health!"));
            switchToUI("actions");
        } else {
            smallPotionQty--;
            itemMenu.gameObject.SetActive(false);
            switchToUI("readout");
            currentParticle = Instantiate(particleManager.buff, allySpawnTransform.position, Quaternion.identity);
            yield return StartCoroutine(DestroyParticle(3.5f));
            yield return StartCoroutine(displayCombatMessage("You used a small potion on " + allyMonster.monsterName + "."));
            int amountHealed;
            amountHealed = allyMonster.Heal(300);
            allyMonster.updateMyStats();
            allyHUD.SetHP(allyMonster.currentHP);
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " was healed for " + amountHealed + "HP!"));
            SwapActingMonster();
            StartCoroutine(TrainerTurn());
        }
    }

    public IEnumerator UsePotionLarge() {
        Debug.Log("Use large potion");
        audioManager.playBlip();
        Debug.Log("current = " + actingMonster.currentHP);
        Debug.Log("max = " + actingMonster.maxHP);
        itemMenu.gameObject.SetActive(false);
        if (actingMonster.currentHP >= actingMonster.maxHP) {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is already at full health!"));
            switchToUI("actions");
        } else {
            largePotionQty--;
            itemMenu.gameObject.SetActive(false);
            switchToUI("readout");
            currentParticle = Instantiate(particleManager.buff, allySpawnTransform.position, Quaternion.identity);
            yield return StartCoroutine(DestroyParticle(3.5f));
            yield return StartCoroutine(displayCombatMessage("You used a large potion on " + allyMonster.monsterName + "."));
            allyMonster.currentHP = allyMonster.maxHP;
            allyMonster.updateMyStats();
            allyHUD.SetHP(allyMonster.currentHP);
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " was healed to max HP!"));
            SwapActingMonster();
            StartCoroutine(TrainerTurn());
        }
    }

    public IEnumerator useReviveLeaf() {
        itemMenu.gameObject.SetActive(false);
        switchToUI("readout");
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
        }
    }

    public IEnumerator useAntidote() {
        itemMenu.gameObject.SetActive(false);
        switchToUI("readout");
        audioManager.playBlip();
        itemMenu.gameObject.SetActive(false);
        if (allyMonster.isPoisoned) {
            antidoteQty--;
            allyMonster.isPoisoned = false;
            //int healed = allyMonster.Heal(allyMonster.poisonDamageTaken);

            currentParticle = Instantiate(particleManager.sparking2, allySpawnTransform.position, Quaternion.identity);
            yield return StartCoroutine(DestroyParticle(3.5f));
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " has been cured of their poison"));
        } else {
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " is not poisoned, no need to use an antidote!"));
            switchToUI("actions");
            yield break;
        }
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
    }


    public IEnumerator usePowerGem() {
        itemMenu.gameObject.SetActive(false);
        switchToUI("readout");
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
            currentParticle = Instantiate(particleManager.fantasyEffect, allySpawnTransform.position, Quaternion.identity);
            yield return StartCoroutine(DestroyParticle(4.55f));
            //yield return new WaitForSeconds(4f);
            //Destroy(currentParticle);
            //StartCoroutine(new WaitForSeconds(yield Destroy(currentParticle)));
            yield return StartCoroutine(displayCombatMessage(allyMonster.monsterName + " consumed the Power Gem!\nIt's attack is boosted by " + allyMonster.buffedAttackAmount + " and is now " + allyMonster.attack + "."));
        }
        SwapActingMonster();
        StartCoroutine(TrainerTurn());
    }

    public void onSmallPotionButton() {
        StartCoroutine(UsePotionSmall());
    }

    public void onLargePotionButton() {
        StartCoroutine(UsePotionLarge());
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
}