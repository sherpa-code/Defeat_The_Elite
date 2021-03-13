using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectionScript : MonoBehaviour
{

    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;
    public Button confirmButton;
    public Button cancelButton;
    public Canvas mainMenuCanvas;

    public TextMeshProUGUI slot1MonsterNameText;
    public TextMeshProUGUI slot2MonsterNameText;
    public TextMeshProUGUI slot3MonsterNameText;

    public GameObject slot1Preview;
    public GameObject slot2Preview;
    public GameObject slot3Preview;

    public Canvas monsterSelectCanvas;
    public MonsterSelectScript monsterSelectScript;

    public Transform slot1PreviewTransform;
    public Transform slot2PreviewTransform;
    public Transform slot3PreviewTransform;

    public List<Monster> teamList = new List<Monster>() { null, null, null };
    //public List<TextMeshProUGUI> teamListNameTextList = new List<TextMeshProUGUI>() { null, null, null };

    public void OnSlotButton(int slot)
    {
        Debug.Log("Slot #" + slot + " button pressed");
        monsterSelectScript.EmptyMonsterSelection();
        monsterSelectScript.currentSlot = slot;

        gameObject.SetActive(false);
        monsterSelectCanvas.gameObject.SetActive(true);
    }

    public void OnConfirmButton()
    {
        Debug.Log("Confirm button pressed");

        // start combat
    }

    public void OnCancelButton()
    {
        Debug.Log("Cancel button pressed");

        ResetTeam();
        UpdateTeamPreviews();
        gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);

    }


    public void UpdateTeamPreviews()
    {

        if (teamList[0] != null)
        {
            slot1MonsterNameText.text = teamList[0].monsterName;
            Destroy(slot1Preview);
            slot1Preview = Instantiate(teamList[0].gameObject, slot1PreviewTransform.position, slot1PreviewTransform.rotation);
        }
        else
        {
            slot1MonsterNameText.text = "Empty";
        }

        if (teamList[1] != null)
        {
            slot2MonsterNameText.text = teamList[1].monsterName;
            Destroy(slot2Preview);
            slot2Preview = Instantiate(teamList[1].gameObject, slot2PreviewTransform.position, slot2PreviewTransform.rotation);
        } else
        {
            slot2MonsterNameText.text = "Empty";
        }

        if (teamList[2] != null)
        {
            slot3MonsterNameText.text = teamList[2].monsterName;
            Destroy(slot3Preview);
            slot3Preview = Instantiate(teamList[2].gameObject, slot3PreviewTransform.position, slot3PreviewTransform.rotation);
        } else
        {
            slot3MonsterNameText.text = "Empty";
        }
        

        isTeamFullCheck();
    }

    public void ResetTeam()
    {
        Debug.Log("ResetTeam() fired");

        slot1MonsterNameText.text = "Empty";
        slot2MonsterNameText.text = "Empty";
        slot3MonsterNameText.text = "Empty";


        if (teamList[0])
        {
            Destroy(slot1Preview);
            //slot1Monster = null;
        }

        if (teamList[1])
        {
            Destroy(slot2Preview);
            //slot2Monster = null;
        }

        if (teamList[2])
        {
            Destroy(slot3Preview);
            //slot3Monster = null;
        }

        //teamList = [ slot1Monster, slot2Monster, slot3Monster ];
        //teamList = [slot1Monster, slot2Monster, slot3Monster];

        //teamList.Clear();
        //teamList.Add

        //teamList = new List<Monster> { slot1Monster, slot2Monster, slot3Monster };
        teamList = new List<Monster> { null, null, null };
    }

    //public void ResetTeam()
    //{
    //    Debug.Log("ResetTeam() fired");

    //    slot1MonsterNameText.text = "Empty";
    //    slot2MonsterNameText.text = "Empty";
    //    slot3MonsterNameText.text = "Empty";


    //    if (slot1Monster)
    //    {
    //        Destroy(slot1Preview);
    //        //slot1Monster = null;
    //    }

    //    if (slot2Monster)
    //    {
    //        Destroy(slot2Preview);
    //        //slot2Monster = null;
    //    }

    //    if (slot3Monster)
    //    {
    //        Destroy(slot3Preview);
    //        //slot3Monster = null;
    //    }

    //    //teamList = [ slot1Monster, slot2Monster, slot3Monster ];
    //    //teamList = [slot1Monster, slot2Monster, slot3Monster];

    //    //teamList.Clear();
    //    //teamList.Add

    //    //teamList = new List<Monster> { slot1Monster, slot2Monster, slot3Monster };
    //    teamList = new List<Monster> { null, null, null };
    //}

    //public void ResetTeam()
    //{
    //    for (int i = 0; i < 3; i++)
    //    {
    //        if (teamList[i] == null)
    //        {
    //            if (i == 0)
    //            {
    //                slot1MonsterNameText.text = "";
    //                // destroy monster prefab model
    //            }
    //            else if (i == 1)
    //            {
    //                slot2MonsterNameText.text = "";
    //                // destroy monster prefab model
    //            }
    //            else if (i == 2)
    //            {
    //                slot3MonsterNameText.text = "";
    //                // destroy monster prefab model
    //            }
    //        }
    //    }
    //}

    public void isTeamFullCheck()
    {
        if (teamList[0] != null && teamList[1] != null && teamList[2] != null)
        {
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
    }

}
