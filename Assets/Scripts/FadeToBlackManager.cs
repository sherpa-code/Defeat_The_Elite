using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlackManager : MonoBehaviour {

    public GameObject blackOutSquare;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            StartCoroutine(FadeBlackOutSquare());
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            StartCoroutine(FadeBlackOutSquare(false));
        }
    }
    

    // Start is called before the first frame update
    public IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, int fadeSpeed = 5) {
        Debug.Log("FadeBlackOutSquare");
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (fadeToBlack) {
            while (blackOutSquare.GetComponent<Image>().color.a < 1) {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        } else {
            while (blackOutSquare.GetComponent<Image>().color.a > 0) {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                
                yield return null;
            }
        }

        if (!fadeToBlack) {
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        //this.gameObject.SetActive(false);
    }
}
