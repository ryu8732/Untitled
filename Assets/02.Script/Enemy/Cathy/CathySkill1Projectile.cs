using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CathySkill1Projectile : MonoBehaviour
{
    public float projectileDamage;
    public float projectileSpeed;

    public bool isFire = false;
    public Vector3 startPos;

    private void Awake()
    {
        startPos = transform.localPosition;
    }

    private void OnEnable()
    {
        isFire = false;
        GetComponent<Collider>().enabled = false;
        transform.localPosition = startPos;
    }

    private void Update()
    {
        if (isFire)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * projectileSpeed);
        }
    }

    // ����ü�� �÷��̾ ���� ��� �ش� �÷��̾�� �������� ������.
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            damageable.OnDamage(projectileDamage, Vector3.zero, Vector3.zero);
        }
    }
}
