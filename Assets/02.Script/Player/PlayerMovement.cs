using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public JoystickController moveJoyStick;

    public float moveSpeed = 3.0f;

    private Animator playerAnimator;
    private Rigidbody playerRigidbody;

    private AudioSource audioSource;
    public AudioClip stepClip;

    private Vector3 currentTilePosition;

    private PlayerStatement playerStatement;

    public Transform followCam;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();

        playerStatement = GetComponent<PlayerStatement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerStatement.dead && (playerStatement.currentState == PlayerStatement.State.Idle || playerStatement.currentState == PlayerStatement.State.Move || playerStatement.currentState == PlayerStatement.State.MovableAttack))
        {
            Move();
        }
    }

    private void Move()
    {
        playerAnimator.SetFloat("Move", moveJoyStick.joyDisRatio);

        if (moveJoyStick.isMove)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, followCam.eulerAngles.y + Mathf.Atan2(moveJoyStick.JoyVec.x, moveJoyStick.JoyVec.y) * Mathf.Rad2Deg, transform.eulerAngles.z);

            transform.Translate(Vector3.forward * Time.deltaTime * (moveJoyStick.joyDisRatio * moveSpeed));

            if (playerStatement.currentState != PlayerStatement.State.MovableAttack)
            {
                playerStatement.currentState = PlayerStatement.State.Move;
            }

            if (!audioSource.isPlaying && playerStatement.currentState != PlayerStatement.State.Attack)
            {
                playerStatement.currentState = PlayerStatement.State.Move;
            }
        }

        else
        {
            //// ���̽�ƽ�� �������� �ʾ����� currentState�� Move�� ��� Idle�� ��������
            //if (playerStatement.currentState == PlayerStatement.State.Move)
            //{
            //    playerStatement.currentState = PlayerStatement.State.Idle;
            //}
        }
    }

    private void FootstepClipRun()
    {
        if (playerStatement.currentState == PlayerStatement.State.Move)
        {
            audioSource.PlayOneShot(stepClip, 0.1f);
        }
    }

    private void FootstepClipDash()
    {
        if (playerStatement.currentState == PlayerStatement.State.Move && playerAnimator.GetFloat("Move") == 1.0)
        {
            audioSource.PlayOneShot(stepClip, 0.1f);
        }
    }

    //public void Jump()
    //{
    //    // ����, ����, ��ų ���°� �ƴ� ��쿡�� ������ �����ϴ�.
    //    if (playerStatement.currentState != PlayerStatement.State.Jump && playerStatement.currentState != PlayerStatement.State.Attack && playerStatement.currentState != PlayerStatement.State.Skill)
    //    {
    //        playerAnimator.SetBool("Jump", true);
    //        // ������ ���� ����
    //        playerStatement.currentState = PlayerStatement.State.Jump;
    //        audioSource.PlayOneShot(jumpClip);

    //        // ���� ������ �ӵ��� 0���� ����� ���� ���� ������ ���� ���� �߻��� ������ �� �ִ�.
    //        playerRigidbody.velocity = Vector3.zero;
    //        playerRigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    //    }
    //}


    //private void OnCollisionEnter(Collision collision)
    //{
    //    // ������Ʈ�� ���� (ĳ���� �������� �ٴ�) �� ����� ��쿡�� (���� ���� ��츦 �����ϱ� ���ؼ�)
    //    if (collision.contacts[0].normal.y > 0.7f)
    //    {
    //        // �ֱ� ���°� ���� ���ٸ� ���� �� �ٴڿ� ���� ���·� Idle ���·� ����
    //        if (playerStatement.currentState == PlayerStatement.State.Jump)
    //        {
    //            playerAnimator.SetBool("Jump", false);
    //            playerStatement.currentState = PlayerStatement.State.Idle;
    //        }

    //        // �� ���� ��� ��� ���� �������� �԰ԵǴµ�, �� �� ĳ���͸� �̵� ��ų ��ġ�� ���� �������� ���� Ÿ�Ϸ� �����Ͽ����� �� Ÿ���� ��ġ�� ����ȴ�.
    //        currentTilePosition = collision.gameObject.transform.position;
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    // ĳ���Ͱ� �����Ͽ� Ư�� ��ġ�� �����Ͽ� FallZone ������Ʈ�� ����� ���
    //    if (other.transform.tag == "Fall")
    //    {
    //        // ĳ���͸� �������� ���� Ÿ�Ϸ� �̵���Ų��.
    //        playerRigidbody.velocity = Vector3.zero;
    //        transform.position = currentTilePosition + new Vector3(0.0f, 0.55f, 0.0f);

    //        // ĳ���Ϳ��� �ִ�ü���� ������ ���ظ� ������.
    //        LivingEntity livingEntity = GetComponent<LivingEntity>();
    //        //livingEntity.photonView.RPC("OnDamage", RpcTarget.MasterClient, livingEntity.startingHealth / 2.0f, Vector3.zero, Vector3.zero);
    //    }
    //}
}
