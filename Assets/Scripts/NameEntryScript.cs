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
    public System.Random r = new System.Random();
    public List<string> randomNames = new List<string>() { "Ramza", "Agrias", "Cidolfus", "Delita", "Dycedarg", "Folmarv", "Meliadoul", "Mustadio", "Gafgarion", "Argath", "Wiegraf", "Rapha", "Zalbaag", "Belias", "YoYo", "Sherpa" };


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

    public void OnCancelButton() {
        Debug.Log("Cancel button pressed");
        audioManager.playBlip();
        mainMenuScript.returnToMainMenu();
    }

    public void RandomName() {
        int roll = r.Next(0, randomNames.Count - 1);

        if (nameInputField.text != randomNames[roll]) {
            nameInputField.text = randomNames[roll];
        } else {
            RandomName();
        }
    }
}
