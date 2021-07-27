using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public enum ItemType
    {
        weapon,
        helmet,
        accessories,
        etc,
        usable
    }

    public enum ItemParts
    {
        none,
        head,
        back,
        RH,
        LH,
        TH,
    }

    public Item(ItemType itemType, ItemParts itemParts, int itemNo, string itemName, int damage = 0, float criticalChance = 0f, int health = 0, int mana = 0, float manaRegeneration = 0f)
    {
        this.itemType = itemType;
        this.itemParts = itemParts;
        this.itemNo = itemNo;
        this.itemName = itemName;
        this.damage = damage;
        this.criticalChance = criticalChance;
        this.health = health;
        this.mana = mana;
        this.manaRegeneration = manaRegeneration;
    }

    public ItemType itemType;
    public ItemParts itemParts;
    public int itemNo;
    public string itemName;
    public int damage = 0;
    public float criticalChance = 0f;
    public int health = 0;
    public int mana = 0;
    public float manaRegeneration = 0f;

    public int amount = 0;
    public int price = 0;

    public string itemDescription;

    public bool isEquip = false;
}
