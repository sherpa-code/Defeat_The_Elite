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
    public System.Single messageDisplayTime = 2f;
    

    public void beginGame() {
        gameObject.SetActive(true);
        battleCanvas.gameObject.SetActive(true);
        enemyHUD.gameObject.SetActive(false);
        allyHUD.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        currentEnemyTrainer = enemyTrainers[r.Next(0, enemyTrainers.Count)];
        enemyNameText.text = currentEnemyTrainer.firstName + " " + currentEnemyTrainer.lastName;
        playerNameText.text = playerName;

        currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);

        updateSpecialMoveChargesText();

        StartCoroutine(beginBattle());
    }

    public IEnumerator beginBattle() {
        dialogueText.text = "The battle begins!";
        //dialogueText.text = "Long multiple line string for testing text wrapping and spacing in the section."; // DEBUG
        yield return new WaitForSeconds(3f);

        spawnAllyMonster(0);
        yield return new WaitForSeconds(messageDisplayTime);

        spawnNextEnemyMonster();
        yield return new WaitForSeconds(messageDisplayTime);

        StartCoroutine(checkSpeedAndContinue());
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

    public IEnumerator checkSpeedAndContinue() {
        if (isPlayerFaster() == "yes") {
            dialogueText.text = allyMonster.monsterName + " is faster and acts first!";
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(PlayerTurn());
        } else if (isPlayerFaster() == "tie") {
            dialogueText.text = "Both monsters are equally fast!";
            yield return new WaitForSeconds(messageDisplayTime);
            dialogueText.text = "Your " + allyMonster.monsterName + " acts first!";
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(PlayerTurn());
        } else {
            dialogueText.text = enemyMonster.monsterName + " is faster and acts first!";
            yield return new WaitForSeconds(messageDisplayTime);
            StartCoroutine(EnemyTurn());
        }
    }


    public IEnumerator PlayerTurn() {
        if (allyMonster.isPoisoned) {
            dialogueText.text = allyMonster.name + " is still poisoned!";
            combatReadout.gameObject.SetActive(true);
            bool isDead = allyMonster.TakeDamage(allyMonster.poisonDamageTaken);
            allyHUD.SetHP(allyMonster.currentHP);
            allyMonster.playHurtAnimation();

            if (isDead) {
                StartCoroutine(allyMonster.playDeathAnimation());
                dialogueText.text = allyMonster.monsterName + " has died!";
                yield return new WaitForSeconds(5f);
                Destroy(allyGameObject);
                allyTeamList.RemoveAt(0);

                if (allyTeamList.Count > 0) {
                    yield return new WaitForSeconds(messageDisplayTime);
                    spawnAllyMonster(0); //TODO: Add a way to select 0 or 1, AKA select which of 2 remaining monsters to send out
                    yield return new WaitForSeconds(messageDisplayTime);
                    StartCoroutine(checkSpeedAndContinue());
                    yield break;
                } else {
                    GameOverLost();
                }
            } else { yield return new WaitForSeconds(messageDisplayTime); }
        }


        yield return new WaitForSeconds(messageDisplayTime);

        combatReadout.gameObject.SetActive(false);
        playerActions.gameObject.SetActive(true);

    }

    

    public IEnumerator PlayerAttack() {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        dialogueText.text = allyMonster.monsterName + " tried to melee attack...";

        StartCoroutine(allyMonster.playAttackAnimation());
        yield return new WaitForSeconds(1.5f);

        // TODO: check if attack was successful
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
                if (allTrainersDefeated()) { //if all trainers are defeated
                    GameOverVictory();
                } else { //if there are still trainers undeafeated
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
            Debug.Log("monster hurt animation");
            yield return new WaitForSeconds(3f);
            StartCoroutine(EnemyTurn());
        }
    }

    public bool allTrainersDefeated() {
        foreach (Trainer trainer in enemyTrainers) {
            if (!trainer.isDefeated) { //if a trainer IS NOT defeated
                return false; //All trainers defeated is false
            }
        }
        return true; //else All trainers are deafeated
    }


    

    //public bool poisonCheck(Monster monster) {
    //    if (monster.isPoison) {

    //    }
    //    dialogueText.text = "Condition check here";

    //}

    //public bool deathBreathCheck()
    //{

    //}

    public IEnumerator EnemyTurn() {
        bool isDead = false;
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        
        string decision = EnemyDecision();
        if (decision == "melee") {
            Debug.Log("Enemy Melee Decision");

            dialogueText.text = enemyMonster.monsterName + " attacks...";
            yield return new WaitForSeconds(messageDisplayTime);
            dialogueText.text = enemyMonster.monsterName + "'s attack hit!";
            StartCoroutine(enemyMonster.playAttackAnimation());
            yield return new WaitForSeconds(messageDisplayTime);
            isDead = allyMonster.TakeDamage(allyMonster.attack);
        } else {
            Debug.Log("Enemy Special Decision");

            dialogueText.text = enemyMonster.monsterName + " tries " + enemyMonster.specialAbilityName + "...";
            yield return new WaitForSeconds(messageDisplayTime);
            dialogueText.text = enemyMonster.specialAbilityName + " was successful.";
            StartCoroutine(enemyMonster.playSpecialAnimation());
            yield return new WaitForSeconds(messageDisplayTime);
            isDead = allyMonster.TakeDamage(allyMonster.specialDamage);
        }

        allyHUD.SetHP(allyMonster.currentHP);

        if (isDead) {
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

    public string EnemyDecision() {
        string decision;
        int choice;

        if (enemyMonster.specialChargesLeft > 0) {
            choice = r.Next(0, 1);
            if (choice == 1) { choice = r.Next(0, 1); } // reroll on special roll to give it only 25% activation chance

            if (choice == 0) {
                decision = "melee";
                //} else if (choice == 1) {
            } else {
                decision = "special";
                enemyMonster.specialChargesLeft--;
            }
        } else {
            decision = "melee";
        }

        return decision;
    }

    //public string EnemyDecision() {
    //    string decision;
    //    int choice = r.Next(0, 1);

    //    if (choice == 1) { choice = r.Next(0, 1); } // reroll on special to give it only 25% activation chance

    //    if (enemyMonster.specialChargesLeft > 0) { 
    //        if (choice == 0) {
    //            decision = "melee";
    //            //} else if (choice == 1) {
    //        } else {
    //            decision = "special";
    //        }
    //    } else {
    //        decision = "melee";
    //    }

    //    return decision;
    //}

    public IEnumerator PlayerSpecialAbility() {
        playerActions.gameObject.SetActive(false); // debug setting
        combatReadout.gameObject.SetActive(true); // debug setting

        dialogueText.text = allyMonster.monsterName + " tried " + allyMonster.specialAbilityName + "...";
        StartCoroutine(allyMonster.playSpecialAnimation());

        // TODO: add logic for distinct special abilities, including checking if they hit or not
        allyMonster.Heal(1);
        allyHUD.SetHP(allyMonster.currentHP);
        dialogueText.text = allyMonster.specialAbilityName + " was successful!";

        yield return new WaitForSeconds(messageDisplayTime);

        StartCoroutine(EnemyTurn());

    }

    public IEnumerator PlayerDefend() {
        playerActions.gameObject.SetActive(false); // debug setting
        combatReadout.gameObject.SetActive(true); // debug setting

        dialogueText.text = allyMonster.monsterName + " is defending.\nDefense raised to " + allyMonster.defense + " until next turn.";
        //StartCoroutine(allyMonster.playSpecialAnimation());

        allyMonster.defense = allyMonster.defense * 2;

        yield return new WaitForSeconds(messageDisplayTime);

        StartCoroutine(EnemyTurn());

    }


    public void PlayerItems() {
        //battleCanvas.GetComponent<ItemMenuScript>().gameObject.SetActive(true);
        Debug.Log("PlayerItems() begins");
        itemMenu.gameObject.SetActive(true);
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

    public string isPlayerFaster() {
        if (allyMonster.getSpeed() > enemyMonster.getSpeed()) {
            return "yes";
        } else if (allyMonster.getSpeed() == enemyMonster.getSpeed()) {
            return "tie";
        } else {
            return "no";
        }
    }

    public void GameOverVictory()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        dialogueText.text = "You have defeated all enemy trainers. You win!";

        gameOverHUD.gameObject.SetActive(true);
    }

    public void GameOverLost()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);

        dialogueText.text = "You have no remaining monsters. You lose!";

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
}