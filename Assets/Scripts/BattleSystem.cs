using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BattleState {START,PLAYERTURN,ENEMYTURN,WON,LOST }
public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Monster playerMonster;
    Monster enemyMonster;

    public Text battleText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;


    public BattleState state;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerMonster = playerGO.GetComponent<Monster>();
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyMonster = enemyGO.GetComponent<Monster>();

        battleText.text = "The enemey sent out " + enemyMonster.monsterName + "!";

        playerHUD.SetHUD(playerMonster);
        enemyHUD.SetHUD(enemyMonster);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        battleText.text = "Choose an action:";
    }

    IEnumerator PlayerMeleeAttack()
    {
        bool isDead = enemyMonster.TakeDamage(playerMonster.meleeDamage);
        enemyHUD.SetHP(enemyMonster.currentHP);

        battleText.text = playerMonster.monsterName + " hits " + enemyMonster.monsterName + " for " + playerMonster.meleeDamage + " damage!";
        yield return new WaitForSeconds(2f);
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerMagicAttack()
    {
        bool isDead = enemyMonster.TakeDamage(playerMonster.magicDamage);
        enemyHUD.SetHP(enemyMonster.currentHP);

        battleText.text = playerMonster.monsterName + " blasts " + enemyMonster.monsterName + " for " + playerMonster.magicDamage + " damage!";
        yield return new WaitForSeconds(2f);
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator EnemyTurn()
    {//Enemy always attacks with melee, nothing else.
        battleText.text = enemyMonster.monsterName + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerMonster.TakeDamage(enemyMonster.meleeDamage);

        playerHUD.SetHP(playerMonster.currentHP);
        battleText.text = enemyMonster.monsterName + " hits " + playerMonster.monsterName + " for " + enemyMonster.meleeDamage + " damage!";

        yield return new WaitForSeconds(1f);


        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
    void EndBattle()
    {
        if (state == BattleState.WON){
            battleText.text = "You won the battle!";
        } else if (state == BattleState.LOST)
        {
            battleText.text = "You lost the battle...";
        }
    }
    public void OnMeleeAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerMeleeAttack());
    }

    public void OnMagicAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerMagicAttack());
    }
}
