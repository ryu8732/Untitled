using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadishFSM : Enemy
{
    public GameObject attackArea, attackDetectArea;

    protected class RadishAttack : IState
    {
        private RadishFSM owner;

        public RadishAttack(RadishFSM owner)
        {
            this.owner = owner;
        }

        public void OperateEnter()
        {
            owner.StartCoroutine("RadishAttackCoroutine");
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
        attackDetectArea.SetActive(true);

        IState attack = new RadishAttack(this);
        stateDictionary[EnemyState.Attack] = attack;

        attackArea.GetComponent<TriggerCallback>().CollisionEnterEvent += RadishAttackArea;
        attackDetectArea.GetComponent<TriggerCallback>().CollisionStayEvent += AttackDetectAreaStay;
    }

    private void AttackDetectAreaStay(Collider col)
    {
        if (col.tag == "Player" && !dead)
        {
            if (!targetObj.GetComponent<LivingEntity>().dead && stateMachine.CurrentState != stateDictionary[EnemyState.Attack] && stateMachine.CurrentState != stateDictionary[EnemyState.Back])
            {
                stateMachine.SetState(stateDictionary[EnemyState.Attack]);
            }
        }
    }

    private void RadishAttackArea(Collider col)
    {
        if(col.tag == "Player")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            damageable.OnDamage(baseDamage, Vector3.zero, Vector3.zero);
        }
    }

    IEnumerator RadishAttackCoroutine()
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
}
