using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {
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
    public AudioManager audioManager;
    public BGMManager BGMManager;
    //private AudioSource audioSource;
    //public AudioClip menuBlipSound;


    void Start() {
        returnToMainMenu();
        //battleCanvas.GetComponent<ItemMenuScript>().gameObject.SetActive(true);
    }

    //void Start() {
    //    StartCoroutine(yoyoMP());
    //    //battleCanvas.GetComponent<ItemMenuScript>().gameObject.SetActive(true);
    //}


    //IEnumerator Start() {
    //    StartCoroutine(yoyoMP());
    //    yield return new WaitForSeconds(0f);
    //    //battleCanvas.GetComponent<ItemMenuScript>().gameObject.SetActive(true);
    //}

    public IEnumerator yoyoMP() {
        yield return new WaitForSeconds(0.2f);
        returnToMainMenu();
    }

    public void returnToMainMenu() {
        BGMManager.playMenuBGM();
        monsterSelectCanvas.gameObject.SetActive(false);
        battleCanvas.gameObject.SetActive(false);
        battleSystem.gameOverHUD.gameObject.SetActive(false);
        teamSelectionScript.ResetTeam();
        teamSelectionScript.UpdateTeamPreviews();

        if (battleSystem.allyGameObject) {
            Destroy(battleSystem.allyGameObject);
        }

        if (battleSystem.enemyGameObject) {
            Destroy(battleSystem.enemyGameObject);
        }

        //battleCanvas.gameOverHUD.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    
    public void OnStartGameButton() {
        audioManager.playBlip();
      
        Debug.Log("Start Game button pressed");
        //battleSystem.itemMenu.gameObject.SetActive(true);
        
        gameObject.SetActive(false);
        nameEntryCanvas.gameObject.SetActive(true);
        nameEntryCanvas.GetComponent<NameEntryScript>().clearNameInputField();

        //TeamSelectCanvas.gameObject.SetActive(true);

        //combatReadout.gameObject.SetActive(false); // debug setting
        //playerActions.gameObject.SetActive(true); // debug setting
    }

    public void OnQuitButton() {
        audioManager.playBlip();
      
        //TODO: (optional) save any game data here

        #if UNITY_EDITOR // quit if in Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else // quit if in Build
            Application.Quit();
        #endif
    }
}