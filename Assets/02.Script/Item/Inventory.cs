using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    public List<Item> itemList;
    public int gold;

    public Inventory()
    {
        itemList = new List<Item>();
        gold = 0;
    }

    public void AddItem(Item item, int amount)
    {
        bool hasItem = false;

        // ��� �������� �ƴϸ鼭, �̹� �������� ������ ���� ��� Add���� �ʰ� amount�� �÷��ش�.
        if (item.itemType > Item.ItemType.accessories)
        {
            foreach (Item itemInList in itemList)
            {
                if (itemInList.itemNo == item.itemNo)
                {
                    itemInList.amount += amount;
                    hasItem = true;

                    break;
                }
            }

            if (!hasItem)
            {
                item.amount += amount;
                itemList.Add(item);
            }
        }
        else
        {
            itemList.Add(item);
        }

        InventoryManager.instance.AddItemGetPanel(item.itemName, amount);
        QuestManager.instance.questChecker();
    }

    public int GetItemAmount(int itemNo)
    {
        foreach (Item itemInList in itemList)
        {
            if (itemInList.itemNo == itemNo)
            {
                return itemInList.amount;
            }
        }

        return -1;
    }

    public void AddGold(int amount)
    {
        gold += amount;

        if (amount > 0)
        {
            InventoryManager.instance.AddItemGetPanel("Gold", amount);
        }
    }

    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
    }

    public void ReduceItem(int itemNo, int amount)
    {
        for(int i = 0; i < itemList.Count; i++)
        {
            if(itemList[i].itemNo == itemNo)
            {
                itemList[i].amount -= amount;

                if(itemList[i].amount <= 0)
                {
                    itemList.RemoveAt(i);
                }

                break;
            }
        }
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }
}
