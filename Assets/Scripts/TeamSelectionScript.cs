﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectionScript : MonoBehaviour
{
    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;
    public Button confirmButton;
    public Button cancelButton;
    public Canvas mainMenuCanvas;

    public TextMeshProUGUI slot1MonsterNameText;
    public TextMeshProUGUI slot2MonsterNameText;
    public TextMeshProUGUI slot3MonsterNameText;

    public GameObject slot1Preview;
    public GameObject slot2Preview;
    public GameObject slot3Preview;

    public Canvas monsterSelectCanvas;
    public MonsterSelectScript monsterSelectScript;

    public Transform slot1PreviewTransform;
    public Transform slot2PreviewTransform;
    public Transform slot3PreviewTransform;

    public MainMenuScript mainMenuScript;

    public System.Random r = new System.Random();

    public List<Monster> teamList = new List<Monster>() { null, null, null };

    public BattleSystem battleSystem;

    void Start()
    {
        monsterSelectScript.GenerateLists();
    }

    public void OnSlotButton(int slot)
    {
        monsterSelectScript.EmptyMonsterSelection();
        monsterSelectScript.currentSlot = slot;

        gameObject.SetActive(false);
        monsterSelectCanvas.gameObject.SetActive(true);
    }

    public void OnConfirmButton()
    {
        battleSystem.allyTeamList = teamList;
        gameObject.SetActive(false);
        battleSystem.beginGame();
        
    }

    public void OnCancelButton()
    {
        ResetTeam();
        UpdateTeamPreviews();
        mainMenuScript.returnToMainMenu();
    }

    public void OnRandomButton()
    {
        for (int i=0; i<3; i++)
        {
            battleSystem.allyTeamList[i] = monsterSelectScript.monsterList[r.Next(0, 9)];
            teamList[i] = battleSystem.allyTeamList[i];
            Debug.Log(battleSystem.allyTeamList[i].monsterName);
        }

        UpdateTeamPreviews();
    }

    public void UpdateTeamPreviews()
    {

        if (teamList[0] != null)
        {
            slot1MonsterNameText.text = teamList[0].monsterName;
            Destroy(slot1Preview);
            slot1Preview = Instantiate(teamList[0].gameObject, slot1PreviewTransform.position, slot1PreviewTransform.rotation);
        }
        else
        {
            slot1MonsterNameText.text = "Empty";
        }

        if (teamList[1] != null)
        {
            slot2MonsterNameText.text = teamList[1].monsterName;
            Destroy(slot2Preview);
            slot2Preview = Instantiate(teamList[1].gameObject, slot2PreviewTransform.position, slot2PreviewTransform.rotation);
        } else
        {
            slot2MonsterNameText.text = "Empty";
        }

        if (teamList[2] != null)
        {
            slot3MonsterNameText.text = teamList[2].monsterName;
            Destroy(slot3Preview);
            slot3Preview = Instantiate(teamList[2].gameObject, slot3PreviewTransform.position, slot3PreviewTransform.rotation);
        } else
        {
            slot3MonsterNameText.text = "Empty";
        }
        
        isTeamFullCheck();
    }

    public void ResetTeam()
    {
        slot1MonsterNameText.text = "Empty";
        slot2MonsterNameText.text = "Empty";
        slot3MonsterNameText.text = "Empty";

        Destroy(slot1Preview);
        Destroy(slot2Preview);
        Destroy(slot3Preview);

        teamList = new List<Monster> { null, null, null };
    }

    public void isTeamFullCheck()
    {
        if (teamList[0] != null && teamList[1] != null && teamList[2] != null)
        {
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
    }

}