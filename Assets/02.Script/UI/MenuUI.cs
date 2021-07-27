using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public GameObject inventory;
    public GameObject info;
    public GameObject others;

    public void OnInfoButtonClicked()
    {
        info.SetActive(true);
    }

    public void OnInventoryButtonClicked()
    {
        InventoryManager.instance.ResetInventory();
        inventory.SetActive(true);
    }
    public void OnQuestButtonClicked()
    {
        QuestManager.instance.ShowProgressQuest();
        QuestManager.instance.questInfoPanel.SetActive(false);
    }


    public void OnQuitButtonClicked()
    {
        GameManager.instance.quitPanel.SetActive(true);
    }
}
