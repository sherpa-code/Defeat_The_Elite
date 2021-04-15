using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour {
    private AudioSource audioSource;
    public AudioClip menuBGM;
    public AudioClip battleBGM;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    // Update is called once per frame
    void Update() {

    }

    public void playMenuBGM() {
        audioSource.Stop();
        audioSource.clip = menuBGM;
        audioSource.Play();
    }

    public void playBattleBGM() {
        audioSource.Stop();
        audioSource.clip = battleBGM;
        audioSource.Play();
    }

}