using System;
using TMPro;
using UnityEngine;

// ����ü�μ� ������ ���� ������Ʈ���� ���� ���븦 ����
// ü��, ������ �޾Ƶ��̱�, ��� ���, ��� �̺�Ʈ�� ����
public abstract class LivingEntity : MonoBehaviour, IDamageable

{
    [Header("Base Stats")]
    public int level;
    public float baseMaxHealth;         // ���̽� ����
    public float baseMaxMana;         // ���̽� ����
    public float baseManaRegeneration;  
    public float baseDamage;
    public float baseCriticalChance;
    public float health;                // ���� ü��
    public float mana;                  // ���� ����

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

    // �������� �Դ� ���
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

    // ü���� ȸ���ϴ� ���
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

    // ��� ó��
    public virtual void Die()
    {
        // ��� ���¸� ������ ����
        dead = true;
    }
}