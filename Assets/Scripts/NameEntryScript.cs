using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameEntryScript : MonoBehaviour
{
    public Button confirmButton;
    public InputField nameInputField;
    public BattleSystem battleSystem;
    public TeamSelectionScript teamSelectionScript;
    public MainMenuScript mainMenuScript;
    public AudioManager audioManager;


    public void enableConfirmButton()
    {
        if (nameInputField.text != "")
        {
            confirmButton.interactable = true;
        } else
        {
            confirmButton.interactable = false;
        }
    }

    public void saveUserName()
    {
        audioManager.playBlip();
        battleSystem.playerName = nameInputField.text;
    }

    public void showTeamSelect()
    {
        gameObject.SetActive(false);
        teamSelectionScript.gameObject.SetActive(true);
    }

    public void clearNameInputField()
    {
        nameInputField.text = "";
    }

    public void OnCancelButton()
    {
        Debug.Log("Cancel button pressed");
        audioManager.playBlip();
        mainMenuScript.returnToMainMenu();
    }

}
