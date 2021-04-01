using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenuScript : MonoBehaviour
{
    public BattleSystem battleSystem;

    public void smallPotionButton()
    {
        Debug.Log("smallPotionButton() fired");

    }

    public void largePotionButton()
    {
        Debug.Log("largePotionButton() fired");

    }

    public void reviveLeafButton()
    {
        Debug.Log("reviveLeafButton() fired");

    }

    public void powerGemButton()
    {
        Debug.Log("powerGemButton() fired");

    }

    public void antidoteButton()
    {
        Debug.Log("antidoteButton() fired");

    }

    public void closeInventoryButton()
    {
        Debug.Log("closeInventoryButton() fired");
        battleSystem.playerActions.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }


}
