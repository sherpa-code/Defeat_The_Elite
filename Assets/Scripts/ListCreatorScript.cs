using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListCreatorScript : MonoBehaviour
{
    BattleSystem battleSystem;

    private Transform SpawnPoint = null;

    private GameObject item = null;

    private RectTransform content = null;

    private int numberOfItems = 3;

    public string[] itemNames = null;
    public Sprite[] itemImages = null;

    // Use this for initialization
    void Start()
    {

        //setContent Holder Height;
        content.sizeDelta = new Vector2(0, numberOfItems * 60);

        for (int i = 0; i < numberOfItems; i++)
        {
            // 60 width of item
            float spawnY = i * 60;
            //newSpawn Position
            Vector3 pos = new Vector3(SpawnPoint.position.x, -spawnY, SpawnPoint.position.z);
            //instantiate item
            GameObject SpawnedInventoryItem = Instantiate(item, pos, SpawnPoint.rotation);
            //setParent
            SpawnedInventoryItem.transform.SetParent(SpawnPoint, false);
            //get ItemDetails Component
            ItemScript itemDetails = SpawnedInventoryItem.GetComponent<ItemScript>();
            //set name
            //itemDetails.itemDisplayName.text = itemNames[i];
            //set image
            itemDetails.itemImage = itemImages[i];


        }
    }


}
