﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public FadeToBlackManager fade;
    //private AudioSource audioSource;
    //public AudioClip menuBlipSound;

    
    public void OnStartGameButton() {
        //SceneManager.LoadScene("TestScene");
        audioManager.playBlip();
        //StartCoroutine(fade.FadeBlackOutSquare());
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