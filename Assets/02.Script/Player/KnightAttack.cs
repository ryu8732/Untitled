using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAttack : PlayerAttack
{
    public GameObject meleeAttackArea, earthQuakeArea, whirlWindArea;
    public AudioClip earthQuakeClip, whirlWindClip;
    private string skill1Coroutine, skill2Coroutine;

    public float earthQuakeMana, whirlWindMana;

    protected override void Awake()
    {
        base.Awake();

        meleeAttackArea.GetComponent<TriggerCallback>().CollisionEnterEvent += MeleeAttackDetect;

        skills.Add(new Skill("EarthQuake", earthQuakeMana, 5f, earthQuakeArea, "EarthQuakeCoroutine"));
        earthQuakeArea.GetComponent<TriggerCallback>().CollisionEnterEvent += EarthQuakeDetect;

        skills.Add(new Skill("WhirlWind", whirlWindMana, 7f, whirlWindArea, "WhirlWindCoroutine"));
        whirlWindArea.GetComponent<TriggerCallback>().CollisionEnterEvent += WhirlWindDetect;

        selectedSkills[0] = skills[0];
        selectedSkills[1] = skills[1];

        skill1Coroutine = selectedSkills[0].coroutineName;
        skill2Coroutine = selectedSkills[1].coroutineName;
    }

    private IEnumerator EarthQuakeCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Earth");
        playerAnimator.SetTrigger("EarthQuakeTrigger");

        yield return new WaitForSeconds(1.0f);
        earthQuakeArea.SetActive(true);
        GameManager.instance.followCam.ShakeCamera(0.3f, 0.2f);

        yield return new WaitForSeconds(0.3f);
        earthQuakeArea.SetActive(false);

        yield return new WaitForSeconds(1.7f);

        playerStatement.currentState = PlayerStatement.State.Idle;
    }

    private IEnumerator WhirlWindCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        playerAnimator.SetTrigger("WhirlWindTrigger");

        yield return new WaitForSeconds(0.65f);
        audioSource.PlayOneShot(whirlWindClip, 0.3f);
        whirlWindArea.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        whirlWindArea.SetActive(false);

        yield return new WaitForSeconds(0.65f);
        audioSource.PlayOneShot(whirlWindClip, 0.3f);
        whirlWindArea.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        whirlWindArea.SetActive(false);

        yield return new WaitForSeconds(0.65f);
        audioSource.PlayOneShot(whirlWindClip, 0.3f);
        whirlWindArea.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        whirlWindArea.SetActive(false);

        yield return new WaitForSeconds(0.65f);
        audioSource.PlayOneShot(whirlWindClip, 0.3f);
        whirlWindArea.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        whirlWindArea.SetActive(false);

        yield return new WaitForSeconds(0.65f);
        audioSource.PlayOneShot(whirlWindClip, 0.3f);
        whirlWindArea.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        whirlWindArea.SetActive(false);

        yield return new WaitForSeconds(0.65f);

        playerStatement.currentState = PlayerStatement.State.Idle;
    }

    private void EarthQuakeDetect(Collider col)
    {
        if (col.tag == "Enemy")
        {
            Debug.Log("Enemy EarthQuake Triggered");
            IDamageable damageable = col.GetComponent<IDamageable>();

            Vector3 hitPos = col.ClosestPoint(meleeAttackArea.transform.position);
            damageable.OnDamage(playerStatement.totalDamage * 1.2f, hitPos, Vector3.zero, CalcCrit());
        }
    }

    private void WhirlWindDetect(Collider col)
    {
        if (col.tag == "Enemy")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            Vector3 hitPos = col.ClosestPoint(meleeAttackArea.transform.position);
            damageable.OnDamage(playerStatement.totalDamage * 0.7f, hitPos, Vector3.zero, CalcCrit());
        }
    }

    protected override void Attack()
    {
        if (!playerStatement.dead && playerStatement.equipItemList.ContainsKey(Item.ItemType.weapon) && (playerStatement.currentState == PlayerStatement.State.Idle || playerStatement.currentState == PlayerStatement.State.Move))
        {
            base.Attack();

            playerStatement.currentState = PlayerStatement.State.Attack;
            StartCoroutine("MeleeAttackCoroutine");
        }
    }

    protected override void Skill1()
    {
        if (!playerStatement.dead && canUseSkill1 && playerStatement.equipItemList.ContainsKey(Item.ItemType.weapon) && (playerStatement.currentState == PlayerStatement.State.Idle || playerStatement.currentState == PlayerStatement.State.Move) && playerStatement.mana >= selectedSkills[0].requireMana)
        {
            base.Skill1();

            playerStatement.currentState = PlayerStatement.State.Attack;
            canUseSkill1 = false;

            playerStatement.ReduceMana(selectedSkills[0].requireMana);
            StartCoroutine(CoolTimeSkill1());
            StartCoroutine(skill1Coroutine);
        }
    }

    protected override void Skill2()
    {
        if (!playerStatement.dead && canUseSkill2 && playerStatement.equipItemList.ContainsKey(Item.ItemType.weapon) && (playerStatement.currentState == PlayerStatement.State.Idle || playerStatement.currentState == PlayerStatement.State.Move) && playerStatement.mana >= selectedSkills[1].requireMana)
        {
            base.Skill2();

            playerStatement.currentState = PlayerStatement.State.MovableAttack;
            canUseSkill2 = false;

            playerStatement.ReduceMana(selectedSkills[1].requireMana);
            StartCoroutine(CoolTimeSkill2());
            StartCoroutine(skill2Coroutine);
        }
    }

    private void MeleeAttackDetect(Collider col)
    {
        if (col.tag == "Enemy")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            Vector3 hitPos = col.ClosestPoint(meleeAttackArea.transform.position);
            damageable.OnDamage(playerStatement.totalDamage, hitPos, Vector3.zero, CalcCrit());
        }
    }

    private IEnumerator MeleeAttackCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        // 공격 시 마다 지정된 순서로 콤보 공격이 나가도록 한다.
        attackCombo++;
        if (attackCombo > attackComboMax)
        {
            attackCombo = 1;
        }

        playerAnimator.SetTrigger("MeleeAttackTrigger");
        playerAnimator.SetFloat("MeleeAttack", attackCombo);
        audioSource.PlayOneShot(meleeAttackClip[attackCombo - 1], 0.7f);

        yield return new WaitForSeconds(0.3f);
        meleeAttackArea.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        meleeAttackArea.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        playerStatement.currentState = PlayerStatement.State.Idle;
    }
}
