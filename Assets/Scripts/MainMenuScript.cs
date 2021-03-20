using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public BattleHUDScript allyHUD;
    public BattleHUDScript enemyHUD;
    public Image playerActions;
    public Image combatReadout;
    public Canvas MainMenu;
    public Canvas TeamSelectCanvas;
    public TeamSelectionScript teamSelectionScript;
    public Canvas monsterSelectCanvas;
    public Canvas battleCanvas;
    public BattleSystem battleSystem;
    public Canvas nameEntryCanvas;


    void Start()
    {
        returnToMainMenu();
    }

    public void returnToMainMenu()
    {
        //nameEntryCanvas.gameObject.nameInputField.text = "";
        //nameEntryCanvas.clearNameInputField();
        //nameEntryCanvas.gameObject.clearNameInputField();

        monsterSelectCanvas.gameObject.SetActive(false);
        battleCanvas.gameObject.SetActive(false);
        battleSystem.gameOverHUD.gameObject.SetActive(false);
        teamSelectionScript.ResetTeam();
        teamSelectionScript.UpdateTeamPreviews();

        if (battleSystem.allyGameObject)
        {
            Destroy(battleSystem.allyGameObject);
        }

        if (battleSystem.enemyGameObject)
        {
            Destroy(battleSystem.enemyGameObject);
        }

        //battleCanvas.gameOverHUD.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    
    public void OnStartGameButton()
    {
        Debug.Log("Start Game button pressed");
        gameObject.SetActive(false);
        nameEntryCanvas.gameObject.SetActive(true);
        //nameEntryCanvas.clearNameInputField();
        nameEntryCanvas.GetComponent<NameEntryScript>().clearNameInputField();

        //TeamSelectCanvas.gameObject.SetActive(true);

        //combatReadout.gameObject.SetActive(false); // debug setting
        //playerActions.gameObject.SetActive(true); // debug setting
    }

    public void OnQuitButton()
    {
        Debug.Log("Quit button pressed");

        UnityEditor.EditorApplication.isPlaying = false; // works in editor
        Application.Quit(); // works in build
    }

}
