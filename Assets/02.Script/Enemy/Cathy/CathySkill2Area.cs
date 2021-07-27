using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CathySkill2Area : MonoBehaviour
{
    public float skill2Damage = 0f;

    public float skill2Range = 0f;
    public float skill2Speed = 0f;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        GetComponent<SphereCollider>().enabled = false;
    }

    void Update()
    {
        // skill2Area만큼 커진다.
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(skill2Range, skill2Range, skill2Range), Time.deltaTime * skill2Speed);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            damageable.OnDamage(skill2Damage, Vector3.zero, Vector3.zero);
        }
    }
}
