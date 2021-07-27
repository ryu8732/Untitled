using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public float baseMaxHealth;
    public float baseMaxMana;
    public float baseManaRegeneration;
    public float baseDamage;
    public float baseCriticalChance;
    public float health;
    public float mana;

    public int exp;
    public int gold;

    public List<Item> hasItemList = new List<Item>();
    public List<Item> equipItemList = new List<Item>();

    public QuestContainer questContainer = new QuestContainer();

    public string currentSceneName;
    public Vector3 currentPosition;
    public Quaternion currentRotation;

}
