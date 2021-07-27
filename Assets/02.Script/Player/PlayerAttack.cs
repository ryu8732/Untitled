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
    protected AudioSource audioSource;      // 오디오 소스

    public int attackComboMax;
    protected int attackCombo;              // 기본 공격의 모션을 나누어 버튼 클릭마다 다른 모션이 나오도록 하기 위함
    protected float currentAttackTime;      // 기본 공격의 딜레이를 계산하기 위한 변수
    public AudioClip[] meleeAttackClip;     // 기본 공격 사운드 클립

    protected PlayerStatement playerStatement;

    protected bool canUseSkill1 = true;     // 스킬1에 대한 쿨타임을 확인하기 위한 변수
    protected bool canUseSkill2 = true;     // 스킬2에 대한 쿨타임을 확인하기 위한 변수

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

    // 패러미터로 받아온 cool만큼의 시간이 흐르면 쿨타임을 리셋한다.
    // 이때, CoolImage를 통해 쿨타임이 얼마나 남았는지를 UI를 통해 플레이어에게 보여준다.
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
