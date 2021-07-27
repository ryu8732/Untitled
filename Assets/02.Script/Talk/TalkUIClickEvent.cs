using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TalkUIClickEvent : MonoBehaviour, IPointerClickHandler
{
    public delegate void ClickEvent();
    public ClickEvent uiClicked;

    public void OnPointerClick(PointerEventData eventData) { uiClicked(); }
}
