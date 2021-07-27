using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Trader : MonoBehaviour
{
    string jsonPath;
    public string storeItemListFileName;
    public Dictionary<int, Item> storeItemList = new Dictionary<int, Item>();

    private void OnEnable()
    {
        jsonPath = Application.persistentDataPath;
        LoadStoreItemList();
    }

    public void LoadStoreItemList()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + storeItemListFileName);
        ItemList temp = JsonUtility.FromJson<ItemList>(jsonData.ToString());

        for (int i = 0; i < temp.itemList.Count; i++)
        {
            storeItemList[temp.itemList[i].itemNo] = temp.itemList[i];
        }
    }

}
