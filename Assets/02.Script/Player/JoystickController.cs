using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour
{
    public Transform Stick; // ���̽�ƽ

    private Vector3 StickFirstPos;  // ���̽�ƽ ó�� ��ġ
    public Vector3 JoyVec;         // ���̽�ƽ ����
    private float Radius;           // ���̽�ƽ Outline�� �� ����
    public float joyDisRatio;
    public bool isMove;

    public GameObject focusTl, focusTr, focusBl, focusBr;

    void Awake()
    {
        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        StickFirstPos = Stick.transform.position;

        isMove = false;

        // ĵ���� ũ�⿡ ���Ͽ� �������� �����Ѵ�.
        float Can = transform.parent.GetComponent<RectTransform>().localScale.x;
        Radius *= Can;
    }

    public void BeginDrag()
    {
        isMove = true;
    }

    public void Drag(BaseEventData baseEventData)
    {
        if (isMove)
        {
            PointerEventData Data = baseEventData as PointerEventData;
            Vector3 Pos = Data.position;

            // ���̽�ƽ ������ ���Ѵ�. ��ƽ �ʱ���ġ->������ġ�� ���� ���⺤��
            JoyVec = (Pos - StickFirstPos).normalized;

            FocusActivate();

            float Dis = Vector3.Distance(Pos, StickFirstPos);

            // ���������� ū ��� (OutLine�� ��� ���) ���� ��������ŭ�� �̵��ϵ��� �Ѵ�.
            if (Dis > Radius)
            {
                Dis = Radius;

            }

            // �������� �������� DIs�� ������ ���Ѵ�. (PlayerMovement ��ũ��Ʈ���� �÷��̾��� �̵� �ӵ��� ���� �� ���)
            joyDisRatio = (float)Dis / Radius;
            Stick.position = StickFirstPos + JoyVec * Dis;
        }
    }

    public void DragEnd()
    {
        if (GameManager.instance.playerStatement.dead)
        {
            return;
        }

        // �巡�׸� ����ġ�� ���� �ʱ�ȭ
        Stick.position = StickFirstPos;
        JoyVec = Vector3.zero;
        joyDisRatio = 0.0f;
        isMove = false;

        DisableAllFocus();
    }

    private void FocusActivate()
    {
        if (JoyVec.x >= 0)
        {
            if(JoyVec.y >= 0)
            {
                if (!focusTr.activeSelf)
                {
                    DisableAllFocus();
                    focusTr.SetActive(true);
                }
            }
            else
            {
                if (!focusBr.activeSelf)
                {
                    DisableAllFocus();
                    focusBr.SetActive(true);
                }
            }
        }
        else
        {
            if (JoyVec.y >= 0)
            {
                if (!focusTl.activeSelf)
                {
                    DisableAllFocus();
                    focusTl.SetActive(true);
                }
            }
            else
            {
                if (!focusBl.activeSelf)
                {
                    DisableAllFocus();
                    focusBl.SetActive(true);
                }
            }
        }
    }

    private void DisableAllFocus()
    {
        focusTl.SetActive(false);
        focusTr.SetActive(false);
        focusBl.SetActive(false);
        focusBr.SetActive(false);
    }
}

