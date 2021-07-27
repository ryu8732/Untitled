using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{

    [System.Serializable]
    public class EffectInfo
    {
        public string EffectKey;
        public Transform StartPositionRotation;
        public float DestroyAfter = 10;
        public bool UseLocalPosition = true;
    }

    public Button attackButton;

    public Button skill1Button;
    private Image skill1CoolImage;
    private TextMeshProUGUI skill1CoolText;

    public Button skill2Button;
    private Image skill2CoolImage;
    private TextMeshProUGUI skill2CoolText;

    protected List<Skill> skills = new List<Skill>();
    protected Skill[] selectedSkills = new Skill[2];

    protected Animator playerAnimator;
    protected AudioSource audioSource;      // ����� �ҽ�

    public int attackComboMax;
    protected int attackCombo;              // �⺻ ������ ����� ������ ��ư Ŭ������ �ٸ� ����� �������� �ϱ� ����
    protected float currentAttackTime;      // �⺻ ������ �����̸� ����ϱ� ���� ����
    public AudioClip[] meleeAttackClip;     // �⺻ ���� ���� Ŭ��

    protected PlayerStatement playerStatement;

    protected bool canUseSkill1 = true;     // ��ų1�� ���� ��Ÿ���� Ȯ���ϱ� ���� ����
    protected bool canUseSkill2 = true;     // ��ų2�� ���� ��Ÿ���� Ȯ���ϱ� ���� ����

    public EffectInfo[] Effects;

    protected virtual void Awake()
    {
        skill1CoolImage = skill1Button.gameObject.transform.GetChild(2).GetComponent<Image>();
        skill1CoolText = skill1Button.gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        skill2CoolImage = skill2Button.gameObject.transform.GetChild(2).GetComponent<Image>();
        skill2CoolText = skill2Button.gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        attackButton.onClick.AddListener(OnAttackButtonClicked);
        skill1Button.onClick.AddListener(OnSkill1ButtonClicked);
        skill2Button.onClick.AddListener(OnSkill2ButtonClicked);

        attackCombo = 0;
        currentAttackTime = Time.time;

        playerAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerStatement = GetComponent<PlayerStatement>();
        
    }

    protected virtual void Attack()
    {
    }

    protected virtual void Skill1()
    {
    }

    protected virtual void Skill2()
    {
    }

    // �з����ͷ� �޾ƿ� cool��ŭ�� �ð��� �帣�� ��Ÿ���� �����Ѵ�.
    // �̶�, CoolImage�� ���� ��Ÿ���� �󸶳� ���Ҵ����� UI�� ���� �÷��̾�� �����ش�.
    protected IEnumerator CoolTimeSkill1()
    {
        float temp = selectedSkills[0].skillCooltime;

        skill1CoolImage.gameObject.SetActive(true);
        skill1CoolText.gameObject.SetActive(true);

        while (temp > 0.0f)
        {
            temp -= Time.deltaTime;

            skill1CoolImage.fillAmount = temp / selectedSkills[0].skillCooltime;
            skill1CoolText.text = ((int)(temp)).ToString();

            yield return new WaitForFixedUpdate();
        }

        skill1CoolImage.gameObject.SetActive(false);
        skill1CoolText.gameObject.SetActive(false);

        canUseSkill1 = true;
    }

    protected IEnumerator CoolTimeSkill2()
    {
        float temp = selectedSkills[1].skillCooltime;

        skill2CoolImage.gameObject.SetActive(true);
        skill2CoolText.gameObject.SetActive(true);

        while (temp > 0.0f)
        {
            temp -= Time.deltaTime;

            skill2CoolImage.fillAmount = temp / selectedSkills[1].skillCooltime;
            skill2CoolText.text = ((int)(temp)).ToString();

            yield return new WaitForFixedUpdate();
        }

        skill2CoolImage.gameObject.SetActive(false);
        skill2CoolText.gameObject.SetActive(false);

        canUseSkill2 = true;
    }

    public void instantiateEffect(int EffectNumber)
    {
        if (Effects == null || Effects.Length <= EffectNumber)
        {
            Debug.LogError("Incorrect effect number or effect is null");
        }

        var instance = ObjectPoolingManager.instance.GetQueue(Effects[EffectNumber].EffectKey);
        instance.transform.position = Effects[EffectNumber].StartPositionRotation.position;
        instance.transform.rotation = Effects[EffectNumber].StartPositionRotation.rotation;

        if (Effects[EffectNumber].UseLocalPosition)
        {
            instance.transform.parent = Effects[EffectNumber].StartPositionRotation.transform;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = new Quaternion();
        }

        StartCoroutine(DestroyEffect(instance, Effects[EffectNumber].DestroyAfter, Effects[EffectNumber].EffectKey));
    }

    public IEnumerator DestroyEffect(GameObject effect, float time, string key)
    {
        yield return new WaitForSeconds(time);

        ObjectPoolingManager.instance.InsertQueue(effect, key);
    }

    public void OnAttackButtonClicked()
    {
        Attack();
    }

    public void OnSkill1ButtonClicked()
    {
        Skill1();
    }

    public void OnSkill2ButtonClicked()
    {
        Skill2();
    }

    public bool CalcCrit()
    {
        float rand = Random.Range(1f, 100f);
        return rand <= playerStatement.totalCriticalChance;
    }
}
