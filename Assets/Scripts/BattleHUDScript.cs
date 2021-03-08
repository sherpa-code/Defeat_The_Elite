using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUDScript : MonoBehaviour
{

    public Text nameText;
    //public Text levelText;
    public Slider hpSlider;

    public void SetHUD(Combatant combatant)
    {
        nameText.text = combatant.combatantName;
        //levelText.text = "Lvl " + combatant.combatantLevel;
        hpSlider.maxValue = combatant.maxHP;
        hpSlider.value = combatant.currentHP;
    }


    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}
