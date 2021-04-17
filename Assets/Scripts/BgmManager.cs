using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {
    private AudioSource audioSource;
    public AudioClip menuBGM;
    public AudioClip battleBGM;
    public AudioClip victoryBGM;
    public AudioClip defeatBGM;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    // Update is called once per frame
    void Update() {

    }



    public void playMenuBGM() {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = menuBGM;
        audioSource.Play();
    }



    public void playBattleBGM() {
        audioSource.Stop();
        audioSource.clip = battleBGM;
        audioSource.Play();
    }

    public void playVictoryBGM() {
        audioSource.Stop();
        audioSource.clip = victoryBGM;
        audioSource.Play();
    }

    public void playDefeatBGM() {
        audioSource.Stop();
        audioSource.clip = defeatBGM;
        audioSource.Play();
    }

}