using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUDScript : MonoBehaviour
{

    public Text nameText;
    //public Text levelText;
    public Slider hpSlider;

    public void SetHUD(Monster monster)
    {
        nameText.text = monster.monsterName;
        //levelText.text = "Lvl " + monster.monsterLevel;
        hpSlider.maxValue = monster.maxHP;
        hpSlider.value = monster.currentHP;
    }


    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}
