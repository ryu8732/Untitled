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

    // Enemy 객체가 가지는 상태
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
            // 타겟이 범위내에 없거나 타겟이 죽은 경우엔 Home으로 돌아간다.
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

            // 타겟과의 거리는 충분히 가깝지만 타겟을 바라보고 있지 않은 경우, 타겟을 향해 회전한다.
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

            // Home에 도착하면, 처음 상태로 되돌아 가기 위해 회전, 체력 등을 처음 상태로 초기화한다.
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
            // Home과의 거리에 충분히 가까워 지면, Idle 상태로 전이한다.
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

    // 플레이어가 감지 범위내에 들어왔을 경우 타겟으로 설정하고 추적한다.
    private void PlayerDetectAreaStay(Collider col)
    {
        if (col.tag == "Player" && !col.GetComponent<LivingEntity>().dead)
        {
            targetObj = col.gameObject;
        }
    }

    // 플레이어가 추적 범위를 벗어날 경우 타겟을 null로 하고, 추적을 중지한다.
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

            // 오브젝트 풀에서 타격 이펙트를 가져와 이펙트를 실행한 뒤, 2초 후 오브젝트 풀로 되돌려보낸다.
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

        // Enemy가 죽으면, enemySpawn에 등록된 시간이 지난 후 리스폰한다.
        enemySpawn.Respawn(transform.parent.transform, enemySpawn.respawnTime);
        StartCoroutine(DestroyEnemy(2.0f));
    }

    // Enemy에 등록된 드롭 아이템, 드롭 확률에 따라 아이템을 드롭한다.
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
