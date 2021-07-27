using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : LivingEntity
{
    public EnemyData.EnemyType enemyType;
    public int enemyId;
    public int itemNo;
    public float dropChance;
    public int maxAmount;
    public int exp;

    public float rotateSpeed = 2.0f;

    [HideInInspector]
    public GameObject targetObj;
    private Vector3 homePos, homeRotation;

    public EnemySlider enemySlider;

    public StateMachine stateMachine;
    public Dictionary<EnemyState, IState> stateDictionary = new Dictionary<EnemyState, IState>();

    [HideInInspector]
    public EnemySpawn enemySpawn;

    [HideInInspector]
    public GameObject playerDetectArea, traceArea;

    protected Animator animator;
    protected AudioSource audioSource;
    public AudioClip hitClip;

    // Enemy ��ü�� ������ ����
    public enum EnemyState
    {
        Idle,
        Chase,
        Back,
        Attack,
        Skill1,
        Skill2,
        Die
    };

    protected class EnemyIdle : IState
    {
        private Enemy owner;

        public EnemyIdle(Enemy owner)
        {
            this.owner = owner;
        }

        public void OperateEnter()
        {

        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
            if(owner.targetObj != null && !owner.targetObj.GetComponent<LivingEntity>().dead)
            {
                owner.stateMachine.SetState(owner.stateDictionary[EnemyState.Chase]);
            }
        }
    }

    protected class EnemyChase : IState
    {
        private Enemy owner;
        private NavMeshAgent navMeshAgent;
        private Animator animator;

        public EnemyChase(Enemy owner)
        {
            this.owner = owner;
            this.navMeshAgent = owner.GetComponent<NavMeshAgent>();
            this.animator = owner.GetComponent<Animator>();
        }

        public void OperateEnter()
        {
            animator.SetFloat("Move", 1.0f);
        }

        public void OperateExit()
        {
            animator.SetFloat("Move", 0.0f);
        }

        public void OperateUpdate()
        {
            // Ÿ���� �������� ���ų� Ÿ���� ���� ��쿣 Home���� ���ư���.
            if(owner.targetObj == null || owner.targetObj.GetComponent<LivingEntity>().dead)
            {
                owner.stateMachine.SetState(owner.stateDictionary[EnemyState.Back]);
            }

            else
            {
                TraceTarget();
            }
        }

        private void TraceTarget()
        {
            Vector3 dir = owner.targetObj.transform.position - owner.transform.position;
            navMeshAgent.SetDestination(owner.targetObj.transform.position);

            // Ÿ�ٰ��� �Ÿ��� ����� �������� Ÿ���� �ٶ󺸰� ���� ���� ���, Ÿ���� ���� ȸ���Ѵ�.
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * owner.rotateSpeed);
            }
        }
    }

    protected class EnemyBack : IState
    {
        private Enemy owner;
        private NavMeshAgent navMeshAgent;
        private Animator animator;

        public EnemyBack(Enemy owner)
        {
            this.owner = owner;
            this.navMeshAgent = owner.GetComponent<NavMeshAgent>();
            this.animator = owner.GetComponent<Animator>();
        }

        public void OperateEnter()
        {
            navMeshAgent.stoppingDistance = 0.0f;

            navMeshAgent.SetDestination(owner.homePos);
            animator.SetFloat("Move", 1.0f);
        }

        public void OperateExit()
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.stoppingDistance = 1.0f;

            // Home�� �����ϸ�, ó�� ���·� �ǵ��� ���� ���� ȸ��, ü�� ���� ó�� ���·� �ʱ�ȭ�Ѵ�.
            owner.transform.rotation = Quaternion.Euler(owner.homeRotation);
            owner.enemySlider.gameObject.SetActive(false);
            owner.health = owner.baseMaxHealth;
            animator.SetFloat("Move", 0.0f);
        }

        public void OperateUpdate()
        {
            BackToHome();
        }

        private void BackToHome()
        {
            // Home���� �Ÿ��� ����� ����� ����, Idle ���·� �����Ѵ�.
            if (Vector3.Distance(owner.transform.position, owner.homePos) <= 1.0f)
            {
                owner.stateMachine.SetState(owner.stateDictionary[EnemyState.Idle]);
            }
        }
    }

    protected class EnemyDie : IState
    {
        private Enemy owner;
        private Animator animator;

        public EnemyDie(Enemy owner)
        {
            this.owner = owner;
            this.animator = owner.GetComponent<Animator>();
        }

        public void OperateEnter()
        {
            animator.SetTrigger("Die");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }

    protected virtual void OnEnable()
    {
        homePos = transform.position;
        homeRotation = transform.rotation.eulerAngles;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerDetectArea.SetActive(true);
        traceArea.SetActive(true);

        playerDetectArea.GetComponent<TriggerCallback>().CollisionStayEvent += PlayerDetectAreaStay;
        traceArea.GetComponent<TriggerCallback>().CollisionExitEvent += TraceAreaExit;

        IState idle = new EnemyIdle(this);
        IState chase = new EnemyChase(this);
        IState back = new EnemyBack(this);
        IState die = new EnemyDie(this);

        stateMachine = new StateMachine(idle);

        stateDictionary[EnemyState.Idle] = idle;
        stateDictionary[EnemyState.Chase] = chase;
        stateDictionary[EnemyState.Back] = back;
        stateDictionary[EnemyState.Die] = die;
    }

    protected void Update()
    {
        if (!dead)
        {
            stateMachine.DoOperateUpdate();
        }
    }

    // �÷��̾ ���� �������� ������ ��� Ÿ������ �����ϰ� �����Ѵ�.
    private void PlayerDetectAreaStay(Collider col)
    {
        if (col.tag == "Player" && !col.GetComponent<LivingEntity>().dead)
        {
            targetObj = col.gameObject;
        }
    }

    // �÷��̾ ���� ������ ��� ��� Ÿ���� null�� �ϰ�, ������ �����Ѵ�.
    private void TraceAreaExit(Collider col)
    {
        if (col.tag == "Player")
        {
            targetObj = null;
        }
    }


    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal, bool isCrit = false)
    {
        if (!dead)
        {
            base.OnDamage(damage, hitPoint, hitNormal, isCrit);
            audioSource.PlayOneShot(hitClip, 0.1f);
            GameManager.instance.followCam.ShakeCamera(0.1f, 0.2f);

            if (enemyType == EnemyData.EnemyType.boss)
            {
                enemySlider.isBoss = true;
                enemySlider.bossNameText.text = name;
            }

            enemySlider.gameObject.SetActive(true);
            enemySlider.SetSliderValue(this);

            // ������Ʈ Ǯ���� Ÿ�� ����Ʈ�� ������ ����Ʈ�� ������ ��, 2�� �� ������Ʈ Ǯ�� �ǵ���������.
            GameObject effect = ObjectPoolingManager.instance.GetQueue("hitEffect");
            effect.transform.position = hitPoint;
            effect.GetComponent<ParticleSystem>().Play();

            StartCoroutine(DestroyEffect(effect, 2.0f));
        }
    }

    public override void Die()
    {
        base.Die();

        DropItem();
        GameManager.instance.GetExp(exp);
        QuestManager.instance.TagetEnemyKilled(enemyId);

        stateMachine.SetState(stateDictionary[EnemyState.Die]);

        // Enemy�� ������, enemySpawn�� ��ϵ� �ð��� ���� �� �������Ѵ�.
        enemySpawn.Respawn(transform.parent.transform, enemySpawn.respawnTime);
        StartCoroutine(DestroyEnemy(2.0f));
    }

    // Enemy�� ��ϵ� ��� ������, ��� Ȯ���� ���� �������� ����Ѵ�.
    private void DropItem()
    {
        int rollDrop = Random.Range(1, 100 + 1);

        if(rollDrop <= dropChance)
        {
            int rollAmount = Random.Range(1, maxAmount + 1);

            GameObject dropedItem = ObjectPoolingManager.instance.GetQueue("dropItem");
            dropedItem.GetComponent<ItemObject>().SetItem(itemNo, rollAmount);

            dropedItem.transform.position = this.transform.position;
            dropedItem.GetComponent<ItemObject>().RandomForce();
            dropedItem.GetComponent<ItemObject>().StartCoroutine("DestroyItemObj", 10f);
        }
    }

    public IEnumerator DestroyEffect(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);

        ObjectPoolingManager.instance.InsertQueue(effect, "hitEffect");
    }

    public IEnumerator DestroyEnemy(float time)
    {
        yield return new WaitForSeconds(time);

        ObjectPoolingManager.instance.InsertQueue(this.gameObject, enemySpawn.enemyData.enemyName);
    }
}
