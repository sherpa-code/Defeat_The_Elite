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

    public Random r = new Random();


    public void beginGame()
    {
        Debug.Log("Beginning game loop");

        // TODO: load random trainer. low priority TODO because fighting the same trainers in order doesnt really matter
        currentEnemyTrainer = enemyTrainers[0];
        currentEnemyTeamList = currentEnemyTrainer.trainerTeam;

        //currentEnemyTeamList.Clear();
        //for (int i=0; i<currentEnemyTrainer.trainerTeam.Count; i++)
        //{
        //    currentEnemyTeamList.Add(currentEnemyTrainer.trainerTeam[i]);
        //}

        enemyNameText.text = currentEnemyTrainer.firstName + " " + currentEnemyTrainer.lastName;
        playerNameText.text = playerName;

        battleState = BattleState.START;

        gameObject.SetActive(true);
        battleCanvas.gameObject.SetActive(true);

        //SetupBattle();
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

        dialogueText.text = "A wild " + enemyMonster.monsterName + " approaches!";

        allyHUD.SetHUD(allyMonster);
        enemyHUD.SetHUD(enemyMonster);

        //yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(0f); // debug setting for instant state change

        battleState = BattleState.PLAYERTURN;
        PlayerTurn(); // TODO: replace with whichever monster is faster
    }

    void PlayerTurn()
    {
        //dialogueText.text = "Choose an action:";
        combatReadout.gameObject.SetActive(false); // debug setting
        playerActions.gameObject.SetActive(true); // debug setting
    }

    //IEnumerator PlayerTurn()
    //{
    //    yield return new WaitForSeconds(0f);
    //    //dialogueText.text = "Choose an action:";
    //    combatReadout.gameObject.SetActive(false); // debug setting
    //    playerActions.gameObject.SetActive(true); // debug setting
    //}

    public void OnMeleeButton()
    {
        Debug.Log("Melee Button clicked");
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerAttack());
    }

    public void OnSpecialButton()
    {
        Debug.Log("Special Button clicked");
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerSpecialAbility());
    }

    public void OnDefendButton()
    {
        Debug.Log("Defend Button clicked");
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerDefend());
    }

    public void OnItemButton()
    {
        Debug.Log("Items Button clicked");
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
            Destroy(enemyGameObject);
            
            // check for remaining monsters
            // if monsters remaining: (if team length > 1)
            //      send out another (TODO: replace with a way to select from remaining)
            //      proceed to enemy turn
            //      state = BattleState.ENEMYTURN;
            // else:
            //      end battle
            //      state = BattleState.WON;

            battleState = BattleState.WON;
            EndBattle();
        }
        else
        {
            //      proceed to enemy turn
            //      state = BattleState.ENEMYTURN;
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
            dialogueText.text = "You have defeated all opponents. You win!";
            
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
            Destroy(allyGameObject);

            // check for remaining monsters
            // if ally monsters remaining: (if team length > 1)
            //      send out another (TODO: replace with a way to select from remaining)
            //      proceed to player turn
            //      state = BattleState.ENEMYTURN;
            // else:
            //      end battle
            //      state = BattleState.WON;

            battleState = BattleState.WON;
            EndBattle();

            battleState = BattleState.LOST;
            EndBattle();
        }
        else
        {
            //      proceed to enemy turn
            //      state = BattleState.ENEMYTURN;
            battleState = BattleState.PLAYERTURN;
            //StartCoroutine(PlayerTurn());
            PlayerTurn();
        }
    }

    IEnumerator PlayerSpecialAbility()
    {
        playerActions.gameObject.SetActive(false); // debug setting
        combatReadout.gameObject.SetActive(true); // debug setting

        dialogueText.text = allyMonster.monsterName + " tried to heal...";
        yield return new WaitForSeconds(2f);

        allyMonster.Heal(1);
        allyHUD.SetHP(allyMonster.currentHP);
        dialogueText.text = "The heal was successful!";

        yield return new WaitForSeconds(2f);
        
        StartCoroutine(EnemyTurn());

    }

    IEnumerator PlayerDefend()
    {
        // Damage enemy
        yield return new WaitForSeconds(2f);

        // Set Defense state
        // Change state based on above result
    }

    IEnumerator PlayerItems()
    {
        // Damage enemy
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