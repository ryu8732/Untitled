using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// UI�� ��׶��忡 �����ϴ� ������ �������, �� ����� Ŭ������ ��쿡 ���� ���� �ִ� UI�� ��Ȱ��ȭ �ȴ�.
public class BehindBackground : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.parent.gameObject.SetActive(false);
    }
}
