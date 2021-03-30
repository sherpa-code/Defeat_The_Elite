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



    public void beginGame()
    {
        Debug.Log("Beginning game loop");

        // TODO: load random trainer. low priority TODO because fighting the same trainers in order doesnt really matter
        //currentEnemyTrainer = enemyTrainers[0];
        currentEnemyTrainer = enemyTrainers[r.Next(0, enemyTrainers.Count)];
        currentEnemyTeamList = new List<Monster>(currentEnemyTrainer.trainerTeam);

        enemyNameText.text = currentEnemyTrainer.firstName + " " + currentEnemyTrainer.lastName;
        playerNameText.text = playerName;

        battleState = BattleState.START;

        gameObject.SetActive(true);
        battleCanvas.gameObject.SetActive(true);

        StartCoroutine(SetupBattle());
    }

    public void loadNextEnemyTrainer()
    {
        if (enemyTrainers.Count > 0) // TODO: move this to the enemy pokemon dead function
        {
            enemyTrainers.RemoveAt(0);
        }

        if (enemyTrainers.Count > 0)
        {
            currentEnemyTrainer = enemyTrainers[0];
        } else
        {
            // victory function
        }
    }

    public void loadNextenemyMonster()
    {
        if (currentEnemyTrainer.trainerTeam.Count > 0)
        {
            // TODO: load next enemy monster
        }
    }

    IEnumerator SetupBattle()
    {
        //TODO: replace allyTeamList[0].gameObject with method that returns the next living player monster
        allyGameObject = Instantiate(allyTeamList[0].gameObject, allySpawnTransform.position, allySpawnTransform.rotation);
        allyMonster = allyGameObject.GetComponent<Monster>();

        //TODO: replace allyTeamList[0].gameObject with method that returns the next living enemy monster
        enemyGameObject = Instantiate(currentEnemyTeamList[0].gameObject, enemySpawnTransform.position, enemySpawnTransform.rotation);
        enemyMonster = enemyGameObject.GetComponent<Monster>();

        allyHUD.SetHUD(allyMonster);
        specialMoveDescriptionText.SetText("(" + allyMonster.specialAbilityName + " - " + allyMonster.specialAbilityDescription.ToString() + ")");
        enemyHUD.SetHUD(enemyMonster);

        dialogueText.text = "A wild " + enemyMonster.monsterName + " approaches!";
        //yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(0f); // debug setting for instant state change


        if (allyMonster.getSpeed() >= enemyMonster.getSpeed())
        {
            battleState = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        else
        {
            battleState = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        //battleState = BattleState.PLAYERTURN;
        //PlayerTurn(); // TODO: replace with whichever monster is faster
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
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerItems());
    }

    IEnumerator PlayerAttack()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        dialogueText.text = allyMonster.monsterName + " tried to melee attack...";
        yield return new WaitForSeconds(2f);

        // TODO: check if attack was successful
        dialogueText.text = "The attack was successful!";

        // Damage enemy and check if dead
        bool isDead = enemyMonster.TakeDamage(allyMonster.attack);

        enemyHUD.SetHP(enemyMonster.currentHP);
        
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            dialogueText.text = enemyMonster.monsterName + " has fainted!";
            yield return new WaitForSeconds(2f);
            Destroy(enemyGameObject);
            currentEnemyTeamList.RemoveAt(0);
            if (currentEnemyTeamList.Count > 0)
            {
                enemyGameObject = Instantiate(currentEnemyTeamList[0].gameObject, enemySpawnTransform.position, enemySpawnTransform.rotation);
                enemyMonster = enemyGameObject.GetComponent<Monster>();
                enemyHUD.SetHUD(enemyMonster);
                enemyHUD.SetHP(enemyMonster.currentHP);
                //dialogueText.text = "You sent out " + enemyMonster.monsterName;
                dialogueText.text = currentEnemyTrainer.getFullName()+" sent out " + enemyMonster.monsterName+".";
                yield return new WaitForSeconds(2f);
                if (allyMonster.getSpeed() >= enemyMonster.getSpeed())
                {
                    battleState = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
                else
                {
                    battleState = BattleState.ENEMYTURN;
                    StartCoroutine(EnemyTurn());
                }
            }
            else
            {
                battleState = BattleState.WON;
                EndBattle();
            }

        }
        else
        {

            battleState = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

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
        dialogueText.text = enemyMonster.monsterName+" attacks...";

        yield return new WaitForSeconds(1f);

        bool isDead = allyMonster.TakeDamage(enemyMonster.attack);

        allyHUD.SetHP(allyMonster.currentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            dialogueText.text = allyMonster.monsterName + " has died!";
            yield return new WaitForSeconds(2f);
            Destroy(allyGameObject);
            //TODO: Add a way to select which team member you want to select;
            allyTeamList.RemoveAt(0);
            if (allyTeamList.Count > 0)
            {
                allyGameObject = Instantiate(allyTeamList[0].gameObject, allySpawnTransform.position, allySpawnTransform.rotation);
                allyMonster = allyGameObject.GetComponent<Monster>();
                allyHUD.SetHUD(allyMonster);
                allyHUD.SetHP(allyMonster.currentHP);
                dialogueText.text ="You sent out " + allyMonster.monsterName;
                yield return new WaitForSeconds(2f);
                if (allyMonster.getSpeed() >= enemyMonster.getSpeed())
                {
                    battleState = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
                else
                {
                    battleState = BattleState.ENEMYTURN;
                    StartCoroutine(EnemyTurn());
                }
            }
            else
            {
                battleState = BattleState.LOST;
                EndBattle();
            }

        }
        else
        {

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
        yield return new WaitForSeconds(2f);

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

    IEnumerator PlayerItems()
    {
        // TODO: make items UI
        // TODO: make item prefabs
        // TODO: make way to track inventory
        yield return new WaitForSeconds(2f);

        // Hide player actions window
        // Show items list window
        
        // Wait for user to click an item or cancel
        // if clicked an item, show confirm window
            // if clicked Yes, hide Confirm window and use item
            // if clicked No, hide Confirm window
        // if clicked cancel hide items list window and show player actions window
    }

}