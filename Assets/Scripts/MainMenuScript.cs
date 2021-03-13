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

    public void OnStartGameButton()
    {
        Debug.Log("Start Game button pressed");
        gameObject.SetActive(false);
        TeamSelectCanvas.gameObject.SetActive(true);

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
