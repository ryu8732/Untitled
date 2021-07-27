using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour
{
    public Transform Stick; // 조이스틱

    private Vector3 StickFirstPos;  // 조이스틱 처음 위치
    public Vector3 JoyVec;         // 조이스틱 벡터
    private float Radius;           // 조이스틱 Outline의 반 지름
    public float joyDisRatio;
    public bool isMove;

    public GameObject focusTl, focusTr, focusBl, focusBr;

    void Awake()
    {
        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        StickFirstPos = Stick.transform.position;

        isMove = false;

        // 캔버스 크기에 대하여 반지름을 조절한다.
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

            // 조이스틱 방향을 구한다. 스틱 초기위치->현재위치에 대한 방향벡터
            JoyVec = (Pos - StickFirstPos).normalized;

            FocusActivate();

            float Dis = Vector3.Distance(Pos, StickFirstPos);

            // 반지름보다 큰 경우 (OutLine을 벗어난 경우) 에는 반지름만큼만 이동하도록 한다.
            if (Dis > Radius)
            {
                Dis = Radius;

            }

            // 반지름을 기준으로 DIs의 비율을 구한다. (PlayerMovement 스크립트에서 플레이어의 이동 속도를 구할 때 사용)
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

        // 드래그를 끝마치면 값을 초기화
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

