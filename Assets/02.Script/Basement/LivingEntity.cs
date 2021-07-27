using System;
using TMPro;
using UnityEngine;

// 생명체로서 동작할 게임 오브젝트들을 위한 뼈대를 제공
// 체력, 데미지 받아들이기, 사망 기능, 사망 이벤트를 제공
public abstract class LivingEntity : MonoBehaviour, IDamageable

{
    [Header("Base Stats")]
    public int level;
    public float baseMaxHealth;         // 베이스 스텟
    public float baseMaxMana;         // 베이스 스텟
    public float baseManaRegeneration;  
    public float baseDamage;
    public float baseCriticalChance;
    public float health;                // 현재 체력
    public float mana;                  // 현재 마나

    public bool dead;

    public Transform damageTextTr;

    protected virtual void SetStatus(int level, float baseMaxHealth, float baseMaxMana, float health, float mana, float baseManaRegeneration, float baseDamage)
    {
        this.level = level;
        this.baseMaxHealth = baseMaxHealth;
        this.baseMaxMana = baseMaxMana;
        this.health = health;
        this.mana = mana;
        this.baseManaRegeneration = baseManaRegeneration;
        this.baseDamage = baseDamage;

        dead = false;
    }

    // 데미지를 입는 기능
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal, bool isCrit = false)
    {

        GameObject damageText = ObjectPoolingManager.instance.GetQueue("floatingText");

        if (isCrit)
        {
            damage *= 1.5f;
            damageText.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }

        else
        {
            damageText.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        health -= damage;
        damageText.GetComponent<TextMeshProUGUI>().text = "-" + damage;
        damageText.transform.position = damageTextTr.position;
        damageText.transform.SetParent(damageTextTr);
        damageText.GetComponent<FloatingText>().DestroyObj(2.0f);

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    // 체력을 회복하는 기능
    public virtual void RestoreHealth(float amount)
    {
        if (dead)
        {
            return;
        }
    }

    public virtual void RestoreMana(float amount)
    {
        if (dead)
        {
            return;
        }
    }

    // 사망 처리
    public virtual void Die()
    {
        // 사망 상태를 참으로 변경
        dead = true;
    }
}