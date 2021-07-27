using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cathy 몬스터의 FSM
public class CathyFSM : Enemy
{
    public GameObject attackArea, attackDetectArea, skill2Area;
    public Transform skill1Projectiles;
    public float skill1Damage, skill1Speed, skill2Range, skill2Speed, skill2Damage;

    protected class CathyAttack : IState
    {
        private CathyFSM owner;

        public CathyAttack(CathyFSM owner)
        {
            this.owner = owner;
        }

        public void OperateEnter()
        {
            owner.StartCoroutine("CathyAttackCoroutine");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }

    protected class CathySkill1 : IState
    {
        private CathyFSM owner;

        public CathySkill1(CathyFSM owner)
        {
            this.owner = owner;
        }

        public void OperateEnter()
        {
            owner.StartCoroutine("CathySkill1Coroutine");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }

    protected class CathySkill2 : IState
    {
        private CathyFSM owner;

        public CathySkill2(CathyFSM owner)
        {
            this.owner = owner;
        }

        public void OperateEnter()
        {
            owner.StartCoroutine("CathySkill2Coroutine");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        IState attack = new CathyAttack(this);
        IState skill1 = new CathySkill1(this);
        IState skill2 = new CathySkill2(this);

        stateDictionary[EnemyState.Attack] = attack;
        stateDictionary[EnemyState.Skill1] = skill1;
        stateDictionary[EnemyState.Skill2] = skill2;

        attackArea.GetComponent<TriggerCallback>().CollisionEnterEvent += CathyAttackArea;
        attackDetectArea.GetComponent<TriggerCallback>().CollisionStayEvent += AttackDetectAreaStay;
    }

    private void AttackDetectAreaStay(Collider col)
    {
        if (col.tag == "Player" && !dead)
        {
            if (stateMachine.CurrentState != stateDictionary[EnemyState.Attack] && stateMachine.CurrentState != stateDictionary[EnemyState.Back] && stateMachine.CurrentState != stateDictionary[EnemyState.Skill1] && stateMachine.CurrentState != stateDictionary[EnemyState.Skill2])
            {
                // 세 가지의 패턴(일반 공격, 스킬1, 스킬2) 중 하나를 랜덤하게 사용
                int rand = Random.Range((int)EnemyState.Attack, (int)EnemyState.Skill2 + 1);
                stateMachine.SetState(stateDictionary[(EnemyState)rand]);
            }
        }
    }

    private void CathyAttackArea(Collider col)
    {
        if (col.tag == "Player")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            damageable.OnDamage(baseDamage, Vector3.zero, Vector3.zero);
        }
    }

    IEnumerator CathyAttackCoroutine()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        attackArea.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        if (targetObj != null)
        {
            stateMachine.SetState(stateDictionary[EnemyState.Chase]);
        }
        else
        {
            stateMachine.SetState(stateDictionary[EnemyState.Back]);
        }
    }

    IEnumerator CathySkill1Coroutine()
    {
        transform.LookAt(targetObj.transform);

        animator.SetTrigger("Skill1Ready");

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < skill1Projectiles.childCount; i++)
        {
            skill1Projectiles.GetChild(i).gameObject.SetActive(true);
            skill1Projectiles.GetChild(i).GetComponent<CathySkill1Projectile>().projectileDamage = baseDamage * skill1Damage;
            skill1Projectiles.GetChild(i).GetComponent<CathySkill1Projectile>().projectileSpeed = skill1Speed;
        }

        yield return new WaitForSeconds(1.5f);

        animator.SetTrigger("Skill1Fire");

        yield return new WaitForSeconds(0.7f);

        for (int i = 0; i < skill1Projectiles.childCount; i++)
        {
            skill1Projectiles.GetChild(i).GetComponent<CathySkill1Projectile>().isFire = true;
            skill1Projectiles.GetChild(i).GetComponent<Collider>().enabled = true;
        }

        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < skill1Projectiles.childCount; i++)
        {
            skill1Projectiles.GetChild(i).gameObject.SetActive(false);
        }

        if (targetObj != null)
        {
            stateMachine.SetState(stateDictionary[EnemyState.Chase]);
        }
        else
        {
            stateMachine.SetState(stateDictionary[EnemyState.Back]);
        }
    }

    IEnumerator CathySkill2Coroutine()
    {
        skill2Area.GetComponent<CathySkill2Area>().skill2Range = this.skill2Range;
        skill2Area.GetComponent<CathySkill2Area>().skill2Speed = this.skill2Speed;
        skill2Area.GetComponent<CathySkill2Area>().skill2Damage = baseDamage * this.skill2Damage;

        skill2Area.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        animator.SetTrigger("Skill2");
        yield return new WaitForSeconds(1.0f);

        skill2Area.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        skill2Area.GetComponent<SphereCollider>().enabled = false;

        skill2Area.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        if (targetObj != null)
        {
            stateMachine.SetState(stateDictionary[EnemyState.Chase]);
        }
        else
        {
            stateMachine.SetState(stateDictionary[EnemyState.Back]);
        }
    }
}
