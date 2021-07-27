using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// UI의 백그라운드에 존재하는 검정색 배경으로, 이 배경을 클릭했을 경우에 가장 위에 있는 UI가 비활성화 된다.
public class BehindBackground : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.parent.gameObject.SetActive(false);
    }
}
