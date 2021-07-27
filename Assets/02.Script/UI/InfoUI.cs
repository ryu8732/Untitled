using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoUI : MonoBehaviour
{
    public Image weapon, weaponDefault, helmet, helmetDefault, accessories, accessoriesDefault;
    public TextMeshProUGUI level, exp, hp, addHp, mp, addMp, damage, addDamage, manaRegen, addManaRegen, critChance, addCritChance;
    public Slider expSlider;

    private void OnEnable()
    {
        addHp.gameObject.SetActive(false);
        addMp.gameObject.SetActive(false);
        addDamage.gameObject.SetActive(false);
        addCritChance.gameObject.SetActive(false);
        addManaRegen.gameObject.SetActive(false);

        SetIcons();
        SetStatus();
    }

    private void Update()
    {
        mp.text = (int)GameManager.instance.playerStatement.mana + " / " + GameManager.instance.playerStatement.totalMaxMana;
    }

    public void SetIcons()
    {
        weapon.gameObject.SetActive(false);
        helmet.gameObject.SetActive(false);
        accessories.gameObject.SetActive(false);

        weaponDefault.gameObject.SetActive(true);
        helmetDefault.gameObject.SetActive(true);
        accessoriesDefault.gameObject.SetActive(true);

        foreach (Item item in GameManager.instance.playerStatement.equipItemList.Values)
        {
            switch (item.itemType)
            {
                case Item.ItemType.weapon:
                    weapon.sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                    weaponDefault.gameObject.SetActive(false);
                    weapon.gameObject.SetActive(true);

                    break;
                case Item.ItemType.helmet:
                    helmet.sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                    helmetDefault.gameObject.SetActive(false);
                    helmet.gameObject.SetActive(true);
                    break;
                case Item.ItemType.accessories:
                    accessories.sprite = SpriteManager.instance.LoadItemImage(item.itemNo);

                    accessoriesDefault.gameObject.SetActive(false);
                    accessories.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void SetStatus()
    {
        PlayerStatement playerStatement = GameManager.instance.playerStatement;

        level.text = playerStatement.level.ToString();
        float remainExp = playerStatement.level == 1 ? (float)playerStatement.exp / DataManager.instance.playerStatDict[playerStatement.level].exp : (float)(playerStatement.exp - DataManager.instance.playerStatDict[playerStatement.level - 1].exp) / (float)(DataManager.instance.playerStatDict[playerStatement.level].exp - DataManager.instance.playerStatDict[playerStatement.level - 1].exp);

        expSlider.value = remainExp;
        exp.text = (remainExp * 100f) + "%";

        if (playerStatement.totalMaxHealth != playerStatement.baseMaxHealth)
        {
            addHp.gameObject.SetActive(true);
        }

        if (playerStatement.totalMaxMana != playerStatement.baseMaxMana)
        {
            addMp.gameObject.SetActive(true);
        }

        if (playerStatement.totalDamage != playerStatement.baseDamage)
        {
            addDamage.gameObject.SetActive(true);
        }

        if (playerStatement.totalCriticalChance != playerStatement.baseCriticalChance)
        {
            addCritChance.gameObject.SetActive(true);
        }

        if (playerStatement.totalManaRegeneration != playerStatement.baseManaRegeneration)
        {
            addManaRegen.gameObject.SetActive(true);
        }

        hp.text = playerStatement.health + " / " + playerStatement.totalMaxHealth;
        addHp.text = "(+" + (playerStatement.totalMaxHealth - playerStatement.baseMaxHealth) + ")";

        mp.text = (int)playerStatement.mana + " / " + playerStatement.totalMaxMana;
        addMp.text = "(+" + (playerStatement.totalMaxMana - playerStatement.baseMaxMana) + ")";

        damage.text = playerStatement.totalDamage.ToString();
        addDamage.text = "(+" + (playerStatement.totalDamage - playerStatement.baseDamage) + ")";

        critChance.text = playerStatement.totalCriticalChance.ToString() + "%";
        addCritChance.text = "(+" + (playerStatement.totalCriticalChance - playerStatement.baseCriticalChance) + ")";

        manaRegen.text = playerStatement.totalManaRegeneration.ToString();
        addManaRegen.text = "(+" + (playerStatement.totalManaRegeneration - playerStatement.baseManaRegeneration) + ")";
    }
}
