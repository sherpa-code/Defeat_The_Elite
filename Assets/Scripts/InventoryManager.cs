using System.Collections;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public int smallPotionQty = 3;
    public int largePotionQty = 1;
    public int reviveLeafQty = 1;
    public int antidoteQty = 2;
    public int boostQty = 3;

    public GameManagerScript gameManager;
    public BattleSystem battleSystem;
    public ParticleManagerScript particleManager;

    public Monster allyMonster;
    

    public void UpdateAllyMonster()
    {
        allyMonster = battleSystem.allyMonster;
    }

    public IEnumerator UsePotionSmall()
    {
        Debug.Log("Use small potion");
        UpdateAllyMonster();
        gameManager.audioManager.playBlip();
        //audioManager.playBlip();
        //Debug.Log("current = " + actingMonster.currentHP);
        //Debug.Log("max = " + actingMonster.maxHP);
        battleSystem.itemMenu.gameObject.SetActive(false);
        //itemMenu.gameObject.SetActive(false);
        if (battleSystem.actingMonster.currentHP >= battleSystem.actingMonster.maxHP)
        {
            yield return StartCoroutine(battleSystem.displayCombatMessage(allyMonster.monsterName + " is already at full health!"));
            battleSystem.switchToUI("actions");
        }
        else
        {
            smallPotionQty--;
            battleSystem.itemMenu.gameObject.SetActive(false);
            battleSystem.switchToUI("readout");
            battleSystem.currentParticle = Instantiate(particleManager.buff, battleSystem.allySpawnTransform.position, Quaternion.identity);
            yield return StartCoroutine(battleSystem.DestroyParticle(3.5f));
            yield return StartCoroutine(battleSystem.displayCombatMessage("You used a small potion on " + allyMonster.monsterName + "."));
            int amountHealed;
            amountHealed = allyMonster.Heal(300);
            allyMonster.updateMyStats();
            battleSystem.allyHUD.SetHP(battleSystem.allyMonster.currentHP);
            yield return StartCoroutine(battleSystem.displayCombatMessage(allyMonster.monsterName + " was healed for " + amountHealed + "HP!"));
            battleSystem.SwapActingMonster();
            StartCoroutine(battleSystem.TrainerTurn());
        }
    }

    public IEnumerator UsePotionLarge()
    {
        UpdateAllyMonster();
        Debug.Log("Use large potion");
        battleSystem.audioManager.playBlip();
        //Debug.Log("current = " + actingMonster.currentHP);
        //Debug.Log("max = " + actingMonster.maxHP);
        battleSystem.itemMenu.gameObject.SetActive(false);
        if (battleSystem.actingMonster.currentHP >= battleSystem.actingMonster.maxHP)
        {
            yield return StartCoroutine(battleSystem.displayCombatMessage(allyMonster.monsterName + " is already at full health!"));
            battleSystem.switchToUI("actions");
        }
        else
        {
            largePotionQty--;
            battleSystem.itemMenu.gameObject.SetActive(false);
            battleSystem.switchToUI("readout");
            battleSystem.currentParticle = Instantiate(particleManager.buff, battleSystem.allySpawnTransform.position, Quaternion.identity);
            yield return StartCoroutine(battleSystem.DestroyParticle(3.5f));
            yield return StartCoroutine(battleSystem.displayCombatMessage("You used a large potion on " + allyMonster.monsterName + "."));
            allyMonster.currentHP = battleSystem.allyMonster.maxHP;
            allyMonster.updateMyStats();
            battleSystem.allyHUD.SetHP(battleSystem.allyMonster.currentHP);
            yield return StartCoroutine(battleSystem.displayCombatMessage(allyMonster.monsterName + " was healed to max HP!"));
            battleSystem.SwapActingMonster();
            StartCoroutine(battleSystem.TrainerTurn());
        }
    }

    public IEnumerator UseReviveLeaf()
    {
        UpdateAllyMonster();
        battleSystem.itemMenu.gameObject.SetActive(false);
        battleSystem.switchToUI("readout");
        gameManager.audioManager.playBlip();
        if (battleSystem.allyTeamList.Count == 3)
        {
            yield return StartCoroutine(battleSystem.displayCombatMessage("You do not have downed monster.\nYou can't revive a monster!"));
            battleSystem.switchToUI("actions");
        }
        else
        {
            reviveLeafQty--;
            battleSystem.allyTeamList.Add(battleSystem.lastMonster);
            yield return StartCoroutine(battleSystem.displayCombatMessage("You used a revive potion on " + battleSystem.lastMonster.monsterName + ". They're back on the team!"));
            battleSystem.SwapActingMonster();
            StartCoroutine(battleSystem.TrainerTurn());
        }
    }

    public IEnumerator UseAntidote()
    {
        UpdateAllyMonster();
        battleSystem.switchToUI("readout");
        battleSystem.itemMenu.gameObject.SetActive(false);
        gameManager.audioManager.playBlip();
        if (allyMonster.isPoisoned)
        {
            antidoteQty--;
            allyMonster.isPoisoned = false;
            battleSystem.currentParticle = Instantiate(particleManager.sparking2, battleSystem.allySpawnTransform.position, Quaternion.identity);
            yield return StartCoroutine(battleSystem.DestroyParticle(3.5f));
            yield return StartCoroutine(battleSystem.displayCombatMessage(allyMonster.monsterName + " was cured of poison!"));
        }
        else
        {
            yield return StartCoroutine(battleSystem.displayCombatMessage(allyMonster.monsterName + " is not poisoned,\nno need to use an antidote!"));
            battleSystem.switchToUI("actions");
            yield break;
        }
        battleSystem.SwapActingMonster();
        StartCoroutine(battleSystem.TrainerTurn());
    }

    public IEnumerator UsePowerGem()
    {
        UpdateAllyMonster();
        battleSystem.itemMenu.gameObject.SetActive(false);
        battleSystem.switchToUI("readout");
        battleSystem.audioManager.playBlip();
        if (allyMonster.isBuffed)
        {
            yield return StartCoroutine(battleSystem.displayCombatMessage(allyMonster.monsterName + " is already buffed,\nyou can't buff them twice!"));
            battleSystem.switchToUI("actions");
            yield break;
        }
        else
        {
            boostQty--;
            allyMonster.isBuffed = true;
            allyMonster.buffedAttackAmount = 120;
            allyMonster.buffedTurnsLeft = 4;
            allyMonster.updateMyStats();
            StartCoroutine(battleSystem.displayCombatMessage(allyMonster.monsterName + " consumed the Power Gem!\nIt's attack is boosted by " + allyMonster.buffedAttackAmount + " and is now " + allyMonster.attack + "."));
            battleSystem.currentParticle = Instantiate(particleManager.fantasyEffect, battleSystem.allySpawnTransform.position, Quaternion.identity);
            yield return StartCoroutine(battleSystem.DestroyParticle(3.25f));
            //yield return new WaitForSeconds(4f);
            //Destroy(currentParticle);
            //StartCoroutine(new WaitForSeconds(yield Destroy(currentParticle)));

        }
        battleSystem.SwapActingMonster();
        StartCoroutine(battleSystem.TrainerTurn());
    }

    /* Wrappers for Button OnClick()s */
    public void onSmallPotionButton()
    {
        StartCoroutine(UsePotionSmall());
    }

    public void onLargePotionButton()
    {
        StartCoroutine(UsePotionLarge());
    }

    public void onReviveLeafButton()
    {
        StartCoroutine(UseReviveLeaf());
    }

    public void onAntidoteButton()
    {
        StartCoroutine(UseAntidote());
    }

    public void onPowerGemButton()
    {
        StartCoroutine(UsePowerGem());
    }
}
