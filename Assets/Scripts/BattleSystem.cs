using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST
}

// TODO: replace references to Canvases with refs to Scripts within the code as well as in Unity

public class BattleSystem : MonoBehaviour
{
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

    public BattleState battleState;

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

    //public ItemMenuScript itemMenu;
    public Image itemMenu;



    public void beginGame()
    {

        //itemMenu.gameObject.SetActive(true);
        Debug.Log("Beginning game loop");

        // TODO: load random trainer. low priority TODO because fighting the same trainers in order doesnt really matter
        //currentEnemyTrainer = enemyTrainers[0];
        currentEnemyTrainer = enemyTrainers[r.Next(0, enemyTrainers.Count)];
        currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);
        Debug.Log("Spawned enemy");

        enemyNameText.text = currentEnemyTrainer.firstName + " " + currentEnemyTrainer.lastName;
        playerNameText.text = playerName;

        battleState = BattleState.START;

        gameObject.SetActive(true);
        battleCanvas.gameObject.SetActive(true);

        StartCoroutine(SetupBattle());
    }


    IEnumerator SetupBattle()
    {

        spawnAllyMonster(0);
        //yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(0f); // debug setting for instant state change

        spawnNextEnemyMonster();  
        //yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(0f); // debug setting for instant state change

        checkSpeedAndContinue();

    }

    void PlayerTurn()
    {
        //dialogueText.text = "Choose an action:";
        combatReadout.gameObject.SetActive(false); // debug setting
        playerActions.gameObject.SetActive(true); // debug setting
        
    }

    public void OnMeleeButton()
    {
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerAttack());
    }

    public void OnSpecialButton()
    {
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerSpecialAbility());
    }

    public void OnDefendButton()
    {
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerDefend());
    }

    public void OnItemButton()
    {
        //if (battleState != BattleState.PLAYERTURN) return;
        Debug.Log("Item Button clicked");
        //StartCoroutine(PlayerItems());

        
        PlayerItems();
    }

    IEnumerator PlayerAttack()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        dialogueText.text = allyMonster.monsterName + " tried to melee attack...";

        StartCoroutine(allyMonster.playAttackAnimation());
        yield return new WaitForSeconds(1.5f);

        // TODO: check if attack was successful
        dialogueText.text = "The attack was successful!";
        bool isDead = enemyMonster.TakeDamage(allyMonster.attack);

        enemyHUD.SetHP(enemyMonster.currentHP);
       

        if (isDead) //if monster dies
        {
            StartCoroutine(enemyMonster.playDeathAnimation());
            dialogueText.text = enemyMonster.monsterName + " has died!";
            yield return new WaitForSeconds(5f);
            Destroy(enemyGameObject);
            currentEnemyTeamList.RemoveAt(0);
            if (currentEnemyTeamList.Count > 0)//if enemy trainer has monsters left
            {

                spawnNextEnemyMonster();
                yield return new WaitForSeconds(2f);
                checkSpeedAndContinue();
            }
            else // if enemy trainer has no monsters left
            {
                currentEnemyTrainer.isDefeated = true;
                if (allTrainersDefeated()) //if all trainers are defeated
                {
                    battleState = BattleState.WON;
                    EndBattle();
                }
                else //if there are still trainers undeafeated
                {
                    while (currentEnemyTrainer.isDefeated)
                    {
                        currentEnemyTrainer = enemyTrainers[Random.Range(0, enemyTrainers.Count)];
                    }
                    currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);
                    dialogueText.text = currentEnemyTrainer.getFullName() + " wants to battle!";
                    enemyNameText.text = currentEnemyTrainer.getFullName();
                    yield return new WaitForSeconds(2f);
                    spawnNextEnemyMonster();
                    yield return new WaitForSeconds(2f);
                    checkSpeedAndContinue();
                }
            }

        }
        else //if monster lives
        {
            StartCoroutine(enemyMonster.playHurtAnimation());
            Debug.Log("monster hurt animation");
            yield return new WaitForSeconds(3f);
            battleState = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    public bool allTrainersDefeated()
    {
        foreach (Trainer trainer in enemyTrainers)
        {
            if (!trainer.isDefeated) //if a trainer IS NOT defeated
            {
                return false; //All trainers deafted is false
            }
        }
        return true; //else All trainers are deafeated
    }



void EndBattle()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        if (battleState == BattleState.WON)
        {
            dialogueText.text = "You have defeated "+currentEnemyTrainer.getFullName()+". You win!";
            
        } else if (battleState == BattleState.LOST)
        {
            dialogueText.text = "You have no remaining monsters. You lose!";
        }

        gameOverHUD.gameObject.SetActive(true);
    }

    IEnumerator EnemyTurn()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        StartCoroutine(enemyMonster.playAttackAnimation());
        dialogueText.text = enemyMonster.monsterName+" attacks...";
        yield return new WaitForSeconds(1.5f);

        bool isDead = allyMonster.TakeDamage(enemyMonster.attack);
        allyHUD.SetHP(allyMonster.currentHP);


        if (isDead)
        {
            StartCoroutine(allyMonster.playDeathAnimation());
            dialogueText.text = allyMonster.monsterName + " has died!";
            yield return new WaitForSeconds(5f);
            Destroy(allyGameObject);  
            allyTeamList.RemoveAt(0);
            if (allyTeamList.Count > 0)
            {

                spawnAllyMonster(0); //TODO: Add a way to select 0 or 1, AKA select which of 2 remaining monsters to send out
                yield return new WaitForSeconds(2f);
                checkSpeedAndContinue();
            }
            else
            {
                battleState = BattleState.LOST;
                EndBattle();
            }

        }
        else
        {
            StartCoroutine(allyMonster.playHurtAnimation());
            yield return new WaitForSeconds(2f);
            battleState = BattleState.PLAYERTURN;
            //StartCoroutine(PlayerTurn());
            PlayerTurn();
        }
    }

    IEnumerator PlayerSpecialAbility()
    {
        playerActions.gameObject.SetActive(false); // debug setting
        combatReadout.gameObject.SetActive(true); // debug setting

        dialogueText.text = allyMonster.monsterName + " tried "+allyMonster.specialAbilityName+"...";
        StartCoroutine(allyMonster.playSpecialAnimation());

        // TODO: add logic for distinct special abilities, including checking if they hit or not
        allyMonster.Heal(1);
        allyHUD.SetHP(allyMonster.currentHP);
        dialogueText.text = allyMonster.specialAbilityName+" was successful!";

        yield return new WaitForSeconds(2f);
        
        StartCoroutine(EnemyTurn());

    }

    IEnumerator PlayerDefend()
    {
        // TODO: Temporarily boost defense stat (until next turn)
        // TODO: Set Defense state
        // TODO: Change state based on above result
        yield return new WaitForSeconds(2f);

        
    }

    //IEnumerator PlayerItems()
    //{
    //    //battleCanvas.GetComponent<ItemMenuScript>().gameObject.SetActive(true);
    //    Debug.Log("PlayerItems() begins");
    //    itemMenu.gameObject.SetActive(true);

    //    // TODO: make items UI
    //    // TODO: make item prefabs
    //    // TODO: make way to track inventory
    //    yield return new WaitForSeconds(2f);

    //    // Hide player actions window
    //    // Show items list window

    //    // Wait for user to click an item or cancel
    //    // if clicked an item, show confirm window
    //        // if clicked Yes, hide Confirm window and use item
    //        // if clicked No, hide Confirm window
    //    // if clicked cancel hide items list window and show player actions window
    //}


    public void PlayerItems()
    {
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



    private void checkSpeedAndContinue()
    {
        if (allyMonster.getSpeed() >= enemyMonster.getSpeed()) //if players monster is faster
        {
            battleState = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        else// if enemy monster is faster
        {
            battleState = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    private void spawnAllyMonster(int monsterIndex)
    {
        allyGameObject = Instantiate(allyTeamList[monsterIndex].gameObject, allySpawnTransform.position, allySpawnTransform.rotation);
        allyMonster = allyGameObject.GetComponent<Monster>();
        allyHUD.SetHUD(allyMonster);
        allyHUD.SetHP(allyMonster.currentHP);
        specialMoveDescriptionText.SetText("(" + allyMonster.specialAbilityName + " - " + allyMonster.specialAbilityDescription.ToString() + ")");
        dialogueText.text = "You sent out " + allyMonster.monsterName;

    }

    private void spawnNextEnemyMonster()
    {
        enemyGameObject = Instantiate(currentEnemyTeamList[0].gameObject, enemySpawnTransform.position, enemySpawnTransform.rotation);
        enemyMonster = enemyGameObject.GetComponent<Monster>();
        enemyHUD.SetHUD(enemyMonster);
        enemyHUD.SetHP(enemyMonster.currentHP);
        dialogueText.text = currentEnemyTrainer.getFullName() + " sent out " + enemyMonster.monsterName;
        
    }

}