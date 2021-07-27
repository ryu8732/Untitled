
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreUI : MonoBehaviour
{
    [Header ("Trader")]
    public Trader trader;

    [Header("Item Slot Containers")]
    public Transform buySlotContainer;
    public Transform sellSlotContainer;
    public ScrollRect contentScroll;

    [Header("Item Slots")]
    public GameObject buySlot;
    public GameObject sellSlot;

    [Header("Tab Focusses")]
    public GameObject storeTabFocus;
    public GameObject itemTabFocus;

    [Header("Initial Focus Transform")]
    public Transform buyTabTr;
    public Transform weaponTabTr;

    [Header("Currnet Tab Datas")]
    public GameObject currentStoreTab;
    string currentStoreTabName = "Buy";
    public GameObject currentItemTab;
    string currentItemTabName = "Weapon";

    [Header("Player Gold")]
    public TextMeshProUGUI playerGoldText;

    [Header("Item Infomation Datas")]
    public GameObject itemInfo;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDamageText;
    public TextMeshProUGUI itemCriticalChanceText;
    public TextMeshProUGUI itemHealthText;
    public TextMeshProUGUI itemManaText;
    public TextMeshProUGUI itemManaRegenerationText;
    public TextMeshProUGUI itemDescriptionText;

    public Button sellButton;
    public TextMeshProUGUI sellPriceText;

    private Item selectedItem;

    private void OnEnable()
    {
        OnStoreTabClick(buyTabTr.gameObject);
        OnItemTabClick(weaponTabTr.gameObject);

        RefreshStore();
    }

    public void RefreshGoldText()
    {
        playerGoldText.text = string.Format("{0:#,0}", GameManager.instance.playerStatement.inventory.gold);
    }

    public void OnStoreTabClick(GameObject tab)
    {
        // 활성화 된 탭의 색을 변경해준다.
        currentStoreTab.GetComponent<TextMeshProUGUI>().color = Color.white;
        tab.GetComponent<TextMeshProUGUI>().color = new Color(246f / 255f, 225f / 255f, 156f / 255f);

        // 탭 포커스를 활성화 탭에 위치한다.
        storeTabFocus.transform.SetParent(tab.transform);
        storeTabFocus.transform.localPosition = new Vector3(0f, storeTabFocus.transform.localPosition.y, 0f);

        currentStoreTab = tab;
        currentStoreTabName = tab.name;

        RefreshStore();
    }

    public void OnItemTabClick(GameObject tab)
    {
        // child 0: 비활성화 아이콘, 1번째 child 1: 활성화 아이콘
        currentItemTab.transform.GetChild(0).gameObject.SetActive(true);
        currentItemTab.transform.GetChild(1).gameObject.SetActive(false);

        tab.transform.GetChild(0).gameObject.SetActive(false);
        tab.transform.GetChild(1).gameObject.SetActive(true);

        // 탭 포커스를 활성화 탭에 위치한다.
        itemTabFocus.transform.SetParent(tab.transform);
        itemTabFocus.transform.localPosition = new Vector3(0f, itemTabFocus.transform.localPosition.y, 0f);

        currentItemTab = tab;
        currentItemTabName = tab.name;

        RefreshStore();
    }

    // 상점 UI를 갱신한다.
    public void RefreshStore()
    {
        RefreshGoldText();
        contentScroll.verticalNormalizedPosition = 1.0f;

        for (int i = 0; i < buySlotContainer.childCount; i++)
        {
            Destroy(buySlotContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < sellSlotContainer.childCount; i++)
        {
            Destroy(sellSlotContainer.GetChild(i).gameObject);
        }

        if (currentStoreTabName == "Buy")
        {
            contentScroll.content = buySlotContainer.GetComponent<RectTransform>();

            RefreshStoreToBuy();
        }

        else
        {
            contentScroll.content = sellSlotContainer.GetComponent<RectTransform>();

            RefreshStoreToSell();
        }
    }

    public void RefreshStoreToBuy()
    {
        buySlotContainer.gameObject.SetActive(true);
        sellSlotContainer.gameObject.SetActive(false);

        Item.ItemType tabType = 0;

        switch (currentItemTabName)
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
            case "Usable":
                tabType = Item.ItemType.usable;
                break;
        }

        // 현재 상호작용 중인 상인을 등록한다.
        trader = GameManager.instance.npcList[GameManager.instance.currentInteractId].GetComponent<Trader>();

        foreach (Item item in trader.storeItemList.Values)
        {
            if (item.itemType == tabType)
            {
                GameObject slotObj = Instantiate(buySlot, buySlotContainer);

                slotObj.GetComponent<ItemSlot>().item = item;

                if (item.damage != 0)
                {
                    slotObj.transform.Find("StatTexts/DamageText").GetComponent<Text>().text = "Damage : +" + item.damage;
                    slotObj.transform.Find("StatTexts/DamageText").GetComponent<Text>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/DamageText").GetComponent<Text>().gameObject.SetActive(false);
                }

                if (item.criticalChance != 0)
                {
                    slotObj.transform.Find("StatTexts/CriticalChanceText").GetComponent<Text>().text = "Critical Chance : +" + item.criticalChance + "%";
                    slotObj.transform.Find("StatTexts/CriticalChanceText").GetComponent<Text>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/HealthText").GetComponent<Text>().gameObject.SetActive(false);
                }

                if (item.health != 0)
                {
                    slotObj.transform.Find("StatTexts/HealthText").GetComponent<Text>().text = "Health : +" + item.health;
                    slotObj.transform.Find("StatTexts/HealthText").GetComponent<Text>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/HealthText").GetComponent<Text>().gameObject.SetActive(false);
                }

                if (item.mana != 0)
                {
                    slotObj.transform.Find("StatTexts/ManaText").GetComponent<Text>().text = "Mana : +" + item.mana;
                    slotObj.transform.Find("StatTexts/ManaText").GetComponent<Text>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/ManaText").GetComponent<Text>().gameObject.SetActive(false);
                }

                if (item.manaRegeneration != 0)
                {
                    slotObj.transform.Find("StatTexts/ManaRegenerationText").GetComponent<Text>().text = "ManaRegeneration : +" + item.manaRegeneration;
                    slotObj.transform.Find("StatTexts/ManaRegenerationText").GetComponent<Text>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/ManaRegenerationText").GetComponent<Text>().gameObject.SetActive(false);
                }

                slotObj.transform.Find("ItemBackground/ItemImage").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);
                slotObj.transform.Find("Price/PriceText").GetComponent<TextMeshProUGUI>().text = string.Format("{0:#,0}", item.price);

                // 매개변수가 있는 메소드이므로 델리게이트를 통해 리스너를 등록한다.
                // selectedItem 변수에 아이템을 저장하는 방식으로는 델리게이트를 사용하지 않아도 됨
                slotObj.transform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(delegate { OnBuyButtonClicked(item); });

                slotObj.SetActive(true);
            }
        }
    }

    public void RefreshStoreToSell()
    {
        buySlotContainer.gameObject.SetActive(false);
        sellSlotContainer.gameObject.SetActive(true);

        Item.ItemType tabType = 0;

        switch (currentItemTabName)
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
            case "Usable":
                tabType = Item.ItemType.usable;
                break;
        }

        // 장착한 아이템이 최상단에 나타난다.
        foreach (Item item in GameManager.instance.playerStatement.equipItemList.Values)
        {
            if (item.itemType == tabType)
            {
                GameObject slotObj = Instantiate(sellSlot, sellSlotContainer);
                slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                slotObj.GetComponent<ItemSlot>().item = item;
                slotObj.GetComponent<ItemSlot>().itemClicked = OnSellItemClicked;

                slotObj.transform.Find("IsEquipText").gameObject.SetActive(true);
                slotObj.transform.Find("AmountText").gameObject.SetActive(false);
                slotObj.SetActive(true);
            }
        }

        foreach (Item item in GameManager.instance.playerStatement.inventory.GetItemList())
        {
            if (item.itemType == tabType)
            {
                GameObject slotObj = Instantiate(sellSlot, sellSlotContainer);
                slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                slotObj.GetComponent<ItemSlot>().item = item;
                slotObj.GetComponent<ItemSlot>().itemClicked = OnSellItemClicked;

                slotObj.transform.Find("IsEquipText").gameObject.SetActive(false);

                if (tabType == Item.ItemType.usable)
                {
                    slotObj.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = item.amount.ToString();
                    slotObj.transform.Find("AmountText").gameObject.SetActive(true);
                }

                else
                {
                    slotObj.transform.Find("AmountText").gameObject.SetActive(false);
                }

                slotObj.SetActive(true);
            }
        }
    }

    private void OnBuyButtonClicked(Item item)
    {
        if (item.price <= GameManager.instance.playerStatement.inventory.gold)
        {
            GameManager.instance.playerStatement.inventory.AddItem(item, 1);
            GameManager.instance.playerStatement.inventory.AddGold(-item.price);
            GameManager.instance.playerStatement.PotionTextRefresh();

            RefreshStore();
        }
    }

    public void OnSellItemClicked(Item item)
    {
        selectedItem = item;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription;

        itemDamageText.gameObject.SetActive(false);
        itemCriticalChanceText.gameObject.SetActive(false);
        itemHealthText.gameObject.SetActive(false);
        itemManaText.gameObject.SetActive(false);
        itemManaRegenerationText.gameObject.SetActive(false);

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

        sellPriceText.text = string.Format("{0:#,0}", item.price * 0.8);
        sellButton.gameObject.SetActive(true);
        itemInfo.SetActive(true);
    }

    public void OnSellButtonClicked()
    {
        // 장착중인 아이템 해제
        if (selectedItem.isEquip)
        {
            GameManager.instance.playerStatement.UnequipItem(selectedItem);
        }

        // 아이템 삭제 후 스텟 적용
        if (selectedItem.itemType < Item.ItemType.accessories)
        {
            GameManager.instance.playerStatement.inventory.RemoveItem(selectedItem);
            GameManager.instance.playerStatement.ApplyItems();
        }
        else
        {
            GameManager.instance.playerStatement.inventory.ReduceItem(selectedItem.itemNo, 1);
            GameManager.instance.playerStatement.PotionTextRefresh();
        }
       
        // UI 갱신
        GameManager.instance.playerStatement.inventory.AddGold((int)(selectedItem.price * 0.8));
        RefreshStore();

        itemInfo.SetActive(false);
    }
}
