using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameEntryScript : MonoBehaviour
{
    public Button confirmButton;
    public Button cancelButton;
    public InputField nameInputField;
    public BattleSystem battleSystem;
    public TeamSelectionScript teamSelectionScript;
    public MainMenuScript mainMenuScript;
    public AudioManager audioManager;
    public GameManagerScript gameManager;
    public System.Random r = new System.Random();
    public List<string> randomNames = new List<string>() {
        "Ramza", "Agrias", "Yomp", "Delita", "Dycedarg", "Sherpa", "Meliadoul", "Mustadio",
        "Gafgarion", "Argath", "Wiegraf", "Rapha", "Zalbaag", "Belias", "Yomp", "Sherpa" };


    public void enableConfirmButton() {
        if (nameInputField.text != "") {
            confirmButton.interactable = true;
        } else {
            confirmButton.interactable = false;
        }
    }

    public void saveUserName() {
        audioManager.playBlip();
        battleSystem.playerName = nameInputField.text;
    }

    public void showTeamSelect() {
        gameObject.SetActive(false);
        teamSelectionScript.gameObject.SetActive(true);
    }

    public void clearNameInputField() {
        nameInputField.text = "";
    }

    public void OnConfirmButton() {
        saveUserName();
        Debug.Log("Confirm button pressed");
        audioManager.playBlip();
        showTeamSelect();
    }

    public void OnCancelButton() {
        Debug.Log("Cancel button pressed");
        audioManager.playBlip();
        gameManager.gamePhaseChangeTo("mainMenu");
    }

    public void RandomName() {
        Debug.Log("RandomName button pressed");
        audioManager.playBlip();
        int roll = r.Next(0, randomNames.Count);

        if (nameInputField.text != randomNames[roll]) {
            nameInputField.text = randomNames[roll];
        } else {
            RandomName();
        }
    }
}
