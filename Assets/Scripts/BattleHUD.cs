using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

    public Text nameText;
    public Slider hpSlider;

    public void SetHUD(Monster monster)
    {
        nameText.text = monster.monsterName;
        hpSlider.maxValue = monster.maxHP;
        hpSlider.value = monster.currentHP;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}
