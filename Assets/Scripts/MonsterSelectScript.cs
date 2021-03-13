using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterSelectScript : MonoBehaviour
{
    
    public Monster monster1;
    public Monster monster2;
    public Monster monster3;
    public Monster monster4;
    public Monster monster5;
    public Monster monster6;
    public Monster monster7;
    public Monster monster8;
    public Monster monster9;
    public Monster monster10;

    public Button monster1Button;
    public Button monster2Button;
    public Button monster3Button;
    public Button monster4Button;
    public Button monster5Button;
    public Button monster6Button;
    public Button monster7Button;
    public Button monster8Button;
    public Button monster9Button;
    public Button monster10Button;

    public TextMeshProUGUI monster1ButtonText;
    public TextMeshProUGUI monster2ButtonText;
    public TextMeshProUGUI monster3ButtonText;
    public TextMeshProUGUI monster4ButtonText;
    public TextMeshProUGUI monster5ButtonText;
    public TextMeshProUGUI monster6ButtonText;
    public TextMeshProUGUI monster7ButtonText;
    public TextMeshProUGUI monster8ButtonText;
    public TextMeshProUGUI monster9ButtonText;
    public TextMeshProUGUI monster10ButtonText;

    public Button selectButton;
    public Button cancelButton;

    public Canvas mainMenuCanvas;
    public Canvas teamSelectCanvas;

    public TeamSelectionScript teamSelectionScript;

    public TextMeshProUGUI monsterName;
    public TextMeshProUGUI monsterAttack;
    public TextMeshProUGUI monsterDefense;
    public TextMeshProUGUI monsterSpeed;
    public TextMeshProUGUI monsterSpecialAbilityName;
    public TextMeshProUGUI monsterSpecialAbilityDescription;

    public GameObject monsterPreview;
    public Transform monsterPreviewTransform;

    public int currentSlot;
    public Monster currentMonster;

    List<Monster> monsterList = new List<Monster>();
    List<Button> monsterButtonList = new List<Button>();
    List<TextMeshProUGUI> monsterButtonTextList = new List<TextMeshProUGUI>();


    void Start()
    {
        GenerateLists();
        SetNamesToButtons();
        
    }


    void SetNamesToButtons()
    {
        for (int i=0; i<monsterList.Count; i++)
        {
            monsterButtonTextList[i].text = monsterList[i].monsterName;
        }
    }

    public void OnMonsterSlotButton(int slot)
    {
        Debug.Log("Monster " + slot + " button pressed");
        selectButton.interactable = true;

        currentMonster = monsterList[slot - 1];
        monsterName.text = currentMonster.monsterName;
        monsterAttack.text = currentMonster.attack.ToString();
        monsterDefense.text = currentMonster.defense.ToString();
        monsterSpeed.text = currentMonster.speed.ToString();
        monsterSpecialAbilityName.text = currentMonster.specialAbility;
        monsterSpecialAbilityDescription.text = currentMonster.specialAbilityDescription;


        //GameObject monsterPreview = Instantiate(monsterPreviewGameObject, monsterPreviewTransform.position, monsterPreviewTransform.rotation);
        if (monsterPreview)
        {
            Destroy(monsterPreview);
        }
        //monsterPreview = Instantiate(monsterList[slot - 1].gameObject, monsterPreviewTransform.position, monsterPreviewTransform.rotation);
        monsterPreview = Instantiate(currentMonster.gameObject, monsterPreviewTransform.position, monsterPreviewTransform.rotation);
    }

    public void OnCancelButton()
    {
        Debug.Log("Cancel button pressed.");
        currentSlot = 0;
        EmptyMonsterSelection();
        selectButton.interactable = false;
        gameObject.SetActive(false);
        teamSelectCanvas.gameObject.SetActive(true);
    }

    public void OnSelectButton()
    {
        Debug.Log("Select button pressed.");
        
        teamSelectionScript.teamList[currentSlot - 1] = currentMonster;

        gameObject.SetActive(false);
        EmptyMonsterSelection();

        teamSelectCanvas.gameObject.SetActive(true);
        teamSelectionScript.UpdateTeamPreviews();
    }

    public void EmptyMonsterSelection()
    {
        currentSlot = 0;
        currentMonster = null;
        selectButton.interactable = false;

        monsterName.text = "";
        monsterAttack.text = "";
        monsterDefense.text = "";
        monsterSpeed.text = "";
        monsterSpecialAbilityName.text = "";
        monsterSpecialAbilityDescription.text = "";


        //GameObject monsterPreview = Instantiate(monsterPreviewGameObject, monsterPreviewTransform.position, monsterPreviewTransform.rotation);
        if (monsterPreview)
        {
            Destroy(monsterPreview);
            monsterPreview = null;
        }
    }


    void GenerateLists()
    {
        monsterList.Add(monster1);
        monsterList.Add(monster2);
        monsterList.Add(monster3);
        monsterList.Add(monster4);
        monsterList.Add(monster5);
        monsterList.Add(monster6);
        monsterList.Add(monster7);
        monsterList.Add(monster8);
        monsterList.Add(monster9);
        monsterList.Add(monster10);

        monsterButtonList.Add(monster1Button);
        monsterButtonList.Add(monster2Button);
        monsterButtonList.Add(monster3Button);
        monsterButtonList.Add(monster4Button);
        monsterButtonList.Add(monster5Button);
        monsterButtonList.Add(monster6Button);
        monsterButtonList.Add(monster7Button);
        monsterButtonList.Add(monster8Button);
        monsterButtonList.Add(monster9Button);
        monsterButtonList.Add(monster10Button);

        monsterButtonTextList.Add(monster1ButtonText);
        monsterButtonTextList.Add(monster2ButtonText);
        monsterButtonTextList.Add(monster3ButtonText);
        monsterButtonTextList.Add(monster4ButtonText);
        monsterButtonTextList.Add(monster5ButtonText);
        monsterButtonTextList.Add(monster6ButtonText);
        monsterButtonTextList.Add(monster7ButtonText);
        monsterButtonTextList.Add(monster8ButtonText);
        monsterButtonTextList.Add(monster9ButtonText);
        monsterButtonTextList.Add(monster10ButtonText);
    }

}
