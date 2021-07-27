using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static InventoryManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<InventoryManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static InventoryManager m_instance; // 싱글톤이 할당될 static 변수

    public Inventory inventory;

    public GameObject inventoryTabFocus;
    public GameObject equipmentTabFocus;

    public GameObject currentInventoryTab;
    public string currentInventoryTabName = "Equipment";
    public GameObject currentEquipmentTab;
    public string currentEquipmentTabName = "All";

    public Transform equipmentTabTransform;
    public Transform allTabTranform;

    public GameObject equipmentUi;
    public GameObject usableUi;
    public GameObject etcUi;

    public GameObject slotPrefab;

    public Transform equipmentSlotContainer;
    public Transform usableSlotContainer;
    public Transform etcSlotContainer;

    public GameObject itemInfo;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemDamageText;
    public TextMeshProUGUI itemCriticalChanceText;
    public TextMeshProUGUI itemHealthText;
    public TextMeshProUGUI itemManaText;
    public TextMeshProUGUI itemManaRegenerationText;

    public GameObject infoButtons;

    public TextMeshProUGUI playerGoldText;

    public Transform itemGetPanel;
    public Queue<GameObject> itemGetSlotQueue = new Queue<GameObject>();
    private bool isRunning = false;

    private Item selectedItem;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }


    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public void AddItemGetPanel(string itemName, int amount)
    {
        GameObject itemGetSlot = itemGetPanel.GetChild(0).gameObject;
        itemGetSlot.transform.SetAsLastSibling();

        itemGetSlotQueue.Enqueue(itemGetSlot);
        itemGetSlot.transform.GetComponentInChildren<TextMeshProUGUI>().text = itemName + " X " + amount.ToString(); ;
        itemGetSlot.SetActive(true);

        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine("ItemGetSlotDisable");
        }
    }

    IEnumerator ItemGetSlotDisable()
    {
        while(itemGetSlotQueue.Count > 0)
        {
            yield return new WaitForSeconds(2.0f);

            itemGetSlotQueue.Dequeue().SetActive(false);
        }
        isRunning = false;
    }

    public void OnItemClicked(Item item)
    {
        selectedItem = item;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription;

        itemDamageText.gameObject.SetActive(false);
        itemCriticalChanceText.gameObject.SetActive(false);
        itemHealthText.gameObject.SetActive(false);
        itemManaText.gameObject.SetActive(false);
        itemManaRegenerationText.gameObject.SetActive(false);

        if(item.itemType > Item.ItemType.accessories)
        {
            infoButtons.SetActive(false);
        }

        else
        {
            if(item.isEquip)
            {
                // 0 : Equip 버튼 1: Unequip 버튼
                infoButtons.transform.GetChild(0).gameObject.SetActive(false);
                infoButtons.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                infoButtons.transform.GetChild(0).gameObject.SetActive(true);
                infoButtons.transform.GetChild(1).gameObject.SetActive(false);
            }
            infoButtons.SetActive(true);

            if (item.damage != 0)
            {
                itemDamageText.text = "Damage : +" + item.damage;
                itemDamageText.gameObject.SetActive(true);
            }

            if (item.criticalChance != 0)
            {
                itemCriticalChanceText.text = "Critical Chance : +" + item.criticalChance + "%";
                itemCriticalChanceText.gameObject.SetActive(true);
            }

            if (item.health != 0)
            {
                itemHealthText.text = "Health : +" + item.health;
                itemHealthText.gameObject.SetActive(true);
            }

            if (item.mana != 0)
            {
                itemManaText.text = "Mana : +" + item.mana;
                itemManaText.gameObject.SetActive(true);
            }

            if (item.manaRegeneration != 0)
            {
                itemManaRegenerationText.text = "ManaRegen : +" + item.manaRegeneration;
                itemManaRegenerationText.gameObject.SetActive(true);
            }
        }

        itemInfo.SetActive(true);
    }

    public void RefreshGoldText()
    {
        playerGoldText.text = inventory.gold.ToString();
    }

    public void RefreshInventoryItems()
    {
        RefreshGoldText();

        for(int i = 0; i < equipmentSlotContainer.childCount; i++)
        {
            Destroy(equipmentSlotContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < usableSlotContainer.childCount; i++)
        {
            Destroy(usableSlotContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < etcSlotContainer.childCount; i++)
        {
            Destroy(etcSlotContainer.GetChild(i).gameObject);
        }

        if (currentInventoryTabName == "Equipment")
        {
            equipmentUi.SetActive(true);
            usableUi.SetActive(false);
            etcUi.SetActive(false);

            equipmentSlotContainer.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1.0f;

            if (currentEquipmentTabName == "All")
            {
                // 장착한 아이템이 최상단에 나타난다.
                foreach (Item item in GameManager.instance.playerStatement.equipItemList.Values)
                {
                    GameObject slotObj = Instantiate(slotPrefab, equipmentSlotContainer);
                    slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                    slotObj.GetComponent<ItemSlot>().item = item;
                    slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClicked;

                    slotObj.transform.Find("IsEquipText").gameObject.SetActive(true);
                    slotObj.SetActive(true);
                }

                foreach (Item item in inventory.GetItemList())
                {
                    if (item.itemType <= Item.ItemType.accessories)
                    {
                        GameObject slotObj = Instantiate(slotPrefab, equipmentSlotContainer);
                        slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                        slotObj.GetComponent<ItemSlot>().item = item;
                        slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClicked;

                        slotObj.transform.Find("IsEquipText").gameObject.SetActive(false);
                        slotObj.SetActive(true);
                    }
                }
            }

            else
            {
                Item.ItemType tabType = 0;

                switch (currentEquipmentTabName)
                {
                    case "Weapon":
                        tabType = Item.ItemType.weapon;
                        break;
                    case "Helmet":
                        tabType = Item.ItemType.helmet;
                        break;
                    case "Accessories":
                        tabType = Item.ItemType.accessories;
                        break;
                }

                // 장착한 아이템이 최상단
                foreach (Item item in GameManager.instance.playerStatement.equipItemList.Values)
                {
                    if (item.itemType == tabType)
                    {
                        GameObject slotObj = Instantiate(slotPrefab, equipmentSlotContainer);
                        slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                        slotObj.GetComponent<ItemSlot>().item = item;
                        slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClicked;

                        slotObj.transform.Find("IsEquipText").gameObject.SetActive(true);
                        slotObj.SetActive(true);
                    }
                }

                foreach (Item item in inventory.GetItemList())
                {
                    if (item.itemType == tabType)
                    {
                        GameObject slotObj = Instantiate(slotPrefab, equipmentSlotContainer);
                        slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                        slotObj.GetComponent<ItemSlot>().item = item;
                        slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClicked;

                        slotObj.transform.Find("IsEquipText").gameObject.SetActive(false);
                        slotObj.SetActive(true);
                    }
                }
            }
        }

        else if (currentInventoryTabName == "Usable")
        {
            usableUi.SetActive(true);
            etcUi.SetActive(false);         
            equipmentUi.SetActive(false);

            usableSlotContainer.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1.0f;

            foreach (Item item in inventory.GetItemList())
            {
               
                if (item.itemType == Item.ItemType.usable)
                {
                    GameObject slotObj = Instantiate(slotPrefab, usableSlotContainer);
                    slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);
                    slotObj.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = item.amount.ToString();
                    slotObj.transform.Find("AmountText").gameObject.SetActive(true);

                    slotObj.GetComponent<ItemSlot>().item = item;
                    slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClicked;
                    slotObj.SetActive(true);
                }
            }
        }

        else
        {
            etcUi.SetActive(true);
            usableUi.SetActive(false);
            equipmentUi.SetActive(false);

            etcSlotContainer.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1.0f;

            foreach (Item item in inventory.GetItemList())
            {
                if (item.itemType == Item.ItemType.etc)
                {
                    GameObject slotObj = Instantiate(slotPrefab, etcSlotContainer);
                    slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);
                    slotObj.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = item.amount.ToString();
                    slotObj.transform.Find("AmountText").gameObject.SetActive(true);

                    slotObj.GetComponent<ItemSlot>().item = item;
                    slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClicked;
                    slotObj.SetActive(true);
                }
            }
        }
    }

    public void ResetInventory()
    {
        OnInventoryTabClick(equipmentTabTransform.gameObject);
        OnEquipmentTabClick(allTabTranform.gameObject);

        RefreshInventoryItems();
    }

    public void OnInventoryTabClick(GameObject tab)
    {
        currentInventoryTab.GetComponent<TextMeshProUGUI>().color = Color.white;
        tab.GetComponent<TextMeshProUGUI>().color = new Color(246f / 255f, 225f / 255f, 156f / 255f);

        inventoryTabFocus.transform.SetParent(tab.transform);
        inventoryTabFocus.transform.localPosition = new Vector3(0f, inventoryTabFocus.transform.localPosition.y, 0f);

        currentInventoryTab = tab;
        currentInventoryTabName = tab.name;

        RefreshInventoryItems();
    }

    public void OnEquipmentTabClick(GameObject tab)
    {
        if(currentEquipmentTabName == "All")
        {
            currentEquipmentTab.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            currentEquipmentTab.transform.GetChild(0).gameObject.SetActive(true);
            currentEquipmentTab.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (tab.name == "All")
        {
            tab.GetComponent<TextMeshProUGUI>().color = new Color(246f / 255f, 225f / 255f, 156f / 255f);
        }
        else
        {
            tab.transform.GetChild(0).gameObject.SetActive(false);
            tab.transform.GetChild(1).gameObject.SetActive(true);
        }

        equipmentTabFocus.transform.SetParent(tab.transform);
        equipmentTabFocus.transform.localPosition = new Vector3(0f, equipmentTabFocus.transform.localPosition.y, 0f);

        currentEquipmentTab = tab;
        currentEquipmentTabName = tab.name;

        RefreshInventoryItems();
    }

    public void OnEquipButtonClicked()
    {
        if (!selectedItem.isEquip)
        {
            GameManager.instance.playerStatement.EquipItem(selectedItem);
        }

        itemInfo.SetActive(false);
    }

    public void OnUnequipButtonClicked()
    {
        if (selectedItem.isEquip)
        {
            GameManager.instance.playerStatement.UnequipItem(selectedItem);
        }

        itemInfo.SetActive(false);
    }
}
