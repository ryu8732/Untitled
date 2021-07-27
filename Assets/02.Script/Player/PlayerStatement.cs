using UnityEngine;
using UnityEngine.UI; // UI ���� �ڵ�
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;

// �÷��̾� ĳ������ ����ü�μ��� ������ ���
public class PlayerStatement : LivingEntity
{
    [Header("Total Stats")]
    public float totalMaxHealth;
    public float totalMaxMana;
    public float totalManaRegeneration;
    public float totalDamage;
    public float totalCriticalChance;
    public int exp;

    [Header("Character Sliders")]
    public Slider healthSlider;
    public Slider manaSlider;

    [Header("Potion Buttons")]
    public Button healthPotionButton;
    private Image healthPotionCoolImage;
    private TextMeshProUGUI healthPotionCoolText;
    private float healthPotionCoolTime = 30f;
    private TextMeshProUGUI healthPotionAmountText;
    private bool canUseHealthPotion = true;

    public Button manaPotionButton;
    private Image manaPotionCoolImage;
    private TextMeshProUGUI manaPotionCoolText;
    private float manaPotionCoolTime = 30f;
    private TextMeshProUGUI manaPotionAmountText;
    private bool canUseManaPotion = true;

    [Header("Animation & Audio Components")]
    public AudioClip hitClip;
    private AudioSource playerAudioSource;
    private Animator playerAnimator;

    [HideInInspector]
    public PlayerMovement playerMovement;
    public PlayerAttack playerAttack;

    [Header("Invetory")]
    public Inventory inventory;
    public Dictionary<Item.ItemType, Item> equipItemList;

    [Header("Equipment Positions")]
    public Transform rightHand;
    public Transform leftHand;
    public Transform head;
    public Transform back;

    public enum State
    {
        Idle,
        Move,
        Attack,
        MovableAttack,
        Skill
    };

    public State currentState;

    private void Update()
    {
        DataManager.instance.SavePlayerDataToJson();
    }

    protected void OnEnable()
    {

        foreach (Item equipedItem in equipItemList.Values)
        {
            ApplyItemModel(equipedItem);
        }

        if(health <= 0)
        {
            dead = true;
            Die();
        }

        currentState = State.Idle;
        StartCoroutine("ManaRegeneration");
    }

    public void SetComponents()
    {
        playerAnimator = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        healthPotionButton.onClick.AddListener(OnHealthPotionButtonClicked);
        healthPotionCoolImage = healthPotionButton.gameObject.transform.GetChild(2).GetComponent<Image>();
        healthPotionCoolText = healthPotionButton.gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        healthPotionAmountText = healthPotionButton.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

        manaPotionButton.onClick.AddListener(OnManaPotionButtonClicked);
        manaPotionCoolImage = manaPotionButton.gameObject.transform.GetChild(2).GetComponent<Image>();
        manaPotionCoolText = manaPotionButton.gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        manaPotionAmountText = manaPotionButton.gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
    }

    // ĳ���� ���� ���� �Լ�
    protected void SetCharacterStatus(int level, float baseMaxHealth, float baseMaxMana, float health, float mana, float baseManaRegeneration, float baseDamage)
    {
        base.SetStatus(level, baseMaxHealth, baseMaxMana, health, mana, baseManaRegeneration, baseDamage);

        totalDamage = baseDamage;
        totalMaxHealth = baseMaxHealth;
        totalMaxMana = baseMaxMana;
        totalManaRegeneration = baseManaRegeneration;
    }

    public void ResetPlayerState()
    {
        RestoreHealth(1.0f);
        RestoreMana(1.0f);
        dead = false;
        playerAnimator.SetTrigger("Reset");
    }

    // ������ �������� ���ݿ� �����ϴ� �Լ�
    public void ApplyItems()
    {
        totalDamage = baseDamage;
        totalCriticalChance = baseCriticalChance;
        totalManaRegeneration = baseManaRegeneration;
        totalMaxHealth = baseMaxHealth;
        totalMaxMana = baseMaxMana;

        foreach (Item equipedItem in equipItemList.Values)
        {
            totalDamage += equipedItem.damage;
            totalCriticalChance += equipedItem.criticalChance;
            totalManaRegeneration += equipedItem.manaRegeneration;
            totalMaxHealth += equipedItem.health;
            totalMaxMana += equipedItem.mana;
        }

        if (health > totalMaxHealth)
        {
            health = totalMaxHealth;
        }

        if (mana > totalMaxMana)
        {
            mana = totalMaxMana;
        }

        healthSlider.maxValue = totalMaxHealth;
        manaSlider.maxValue = totalMaxMana;
    }

    private void ApplyItemModel(Item item)
    {
        Transform equipParts = null;

        // ������ �� ��ü
        switch (item.itemParts)
        {
            case Item.ItemParts.LH:
                equipParts = leftHand;
                playerAnimator.SetInteger("HandPos", 1);
                break;
            case Item.ItemParts.RH:
                equipParts = rightHand;
                playerAnimator.SetInteger("HandPos", 2);
                break;
            case Item.ItemParts.TH:
                equipParts = rightHand;
                playerAnimator.SetInteger("HandPos", 3);
                break;
            case Item.ItemParts.head:
                equipParts = head;
                break;
            case Item.ItemParts.back:
                equipParts = back;
                break;
            default:
                break;
        }

        GameObject equipObj = Instantiate(Resources.Load<GameObject>("Item/model_" + item.itemNo), equipParts);
        equipObj.name = item.itemNo.ToString();
    }

    public void EquipItem(Item selectedItem)
    {
        // �����Ϸ��� �����۰� ���� Ÿ���� �������� �̹� �������̶�� �����Ѵ�.
        if (equipItemList.ContainsKey(selectedItem.itemType))
        {
            UnequipItem(selectedItem);
        }

        // �κ��丮���� ����, ���� ��� �߰�
        inventory.RemoveItem(selectedItem);
        selectedItem.isEquip = true;

        equipItemList.Add(selectedItem.itemType, selectedItem);

        if (playerAnimator.isActiveAndEnabled)
        {
            ApplyItemModel(selectedItem);
        }

        ApplyItems();
        InventoryManager.instance.RefreshInventoryItems();
    }

    // ������ �Լ�
    public void UnequipItem(Item item)
    {
        Item.ItemType type = item.itemType;

        // �κ��丮�� �߰�, ���� ��񿡼� ����
        equipItemList[type].isEquip = false;
        inventory.itemList.Add(equipItemList[type]);
        //inventory.AddItem(equipItemList[type], 1);

        switch (type)
        {
            case Item.ItemType.weapon:
                playerAnimator.SetInteger("HandPos", 0);

                switch (equipItemList[type].itemParts)
                {
                    case Item.ItemParts.LH:
                        Destroy(leftHand.Find(equipItemList[type].itemNo.ToString()).gameObject);
                        break;
                    case Item.ItemParts.RH:
                        Destroy(rightHand.Find(equipItemList[type].itemNo.ToString()).gameObject);
                        break;
                    case Item.ItemParts.TH:
                        Destroy(rightHand.Find(equipItemList[type].itemNo.ToString()).gameObject);
                        break;
                }
                break;

            default:
                switch (equipItemList[type].itemParts)
                {
                    case Item.ItemParts.head:
                        Destroy(head.Find(equipItemList[type].itemNo.ToString()).gameObject);
                        break;
                    case Item.ItemParts.back:
                        Destroy(back.Find(equipItemList[type].itemNo.ToString()).gameObject);
                        break;
                }
                break;
        }
        equipItemList.Remove(type);

        ApplyItems();
        //InventoryManager.instance.RefreshInventoryItems();
        InventoryManager.instance.RefreshInventoryItems();
    }


    IEnumerator ManaRegeneration()
    {
        while (true)
        {
            if (mana > totalMaxMana)
            {
                mana = totalMaxMana;
            }
            else
            {
                if (!dead)
                {
                    mana += totalManaRegeneration * Time.deltaTime;
                    manaSlider.value = mana;
                }
            }


            yield return null;
        }
    }

    // ü�� ȸ��
    public override void RestoreHealth(float amount = 0.2f)
    {
        base.RestoreHealth(amount);

        health += totalMaxHealth * amount;

        if (health > totalMaxHealth)
        {
            health = totalMaxHealth;
        }

        healthSlider.value = health;
    }

    public override void RestoreMana(float amount = 0.2f)
    {
        base.RestoreMana(amount);

        mana += totalMaxMana * amount;

        if (mana > totalMaxMana)
        {
            mana = totalMaxMana;
        }

        manaSlider.value = mana;
    }

    public void ReduceMana(float amount)
    {
        mana -= amount;
        manaSlider.value = mana;
    }

    // ������ ó��
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection, bool isCrit = false)
    {
        if (!dead)
        {
            // LivingEntity�� OnDamage() ����(�ǵ����� ����)
            base.OnDamage(damage, hitPoint, hitDirection);
            playerAudioSource.PlayOneShot(hitClip, 0.2f);
            GameManager.instance.followCam.ShakeCamera(0.1f, 0.1f);
            healthSlider.value = health;
        }
    }

    // ��� ó��
    public override void Die()
    {
        // LivingEntity�� Die() ����(��� ����)
        base.Die();

        GameManager.instance.DieEnable();
        playerAnimator.SetTrigger("Die");
    }


    public void OnHealthPotionButtonClicked()
    {
        if (!dead && canUseHealthPotion && inventory.GetItemAmount(3000) > 0)
        {
            canUseHealthPotion = false;

            inventory.ReduceItem(3000, 1);
            PotionTextRefresh();

            RestoreHealth();
            StartCoroutine(CoolTimeHealthPotion());
        }
    }
    protected IEnumerator CoolTimeHealthPotion()
    {
        float temp = healthPotionCoolTime;

        healthPotionCoolImage.gameObject.SetActive(true);
        healthPotionCoolText.gameObject.SetActive(true);

        while (temp > 0.0f)
        {
            temp -= Time.deltaTime;

            healthPotionCoolImage.fillAmount = temp / healthPotionCoolTime;
            healthPotionCoolText.text = ((int)(temp)).ToString();

            yield return new WaitForFixedUpdate();
        }

        healthPotionCoolImage.gameObject.SetActive(false);
        healthPotionCoolText.gameObject.SetActive(false);

        canUseHealthPotion = true;
    }

    public void OnManaPotionButtonClicked()
    {
        if (!dead && canUseManaPotion && inventory.GetItemAmount(3001) > 0)
        {
            canUseManaPotion = false;

            inventory.ReduceItem(3001, 1);
            PotionTextRefresh();

            RestoreMana();
            StartCoroutine(CoolTimeManaPotion());
        }
    }
    protected IEnumerator CoolTimeManaPotion()
    {
        float temp = manaPotionCoolTime;

        manaPotionCoolImage.gameObject.SetActive(true);
        manaPotionCoolText.gameObject.SetActive(true);

        while (temp > 0.0f)
        {
            temp -= Time.deltaTime;

            manaPotionCoolImage.fillAmount = temp / manaPotionCoolTime;
            manaPotionCoolText.text = ((int)(temp)).ToString();

            yield return new WaitForFixedUpdate();
        }

        manaPotionCoolImage.gameObject.SetActive(false);
        manaPotionCoolText.gameObject.SetActive(false);

        canUseManaPotion = true;
    }

    public void PotionTextRefresh()
    {
        healthPotionAmountText.text = inventory.GetItemAmount(3000) == -1 ? "0" : inventory.GetItemAmount(3000).ToString();
        manaPotionAmountText.text = inventory.GetItemAmount(3001) == -1 ? "0" : inventory.GetItemAmount(3001).ToString();
    }
}