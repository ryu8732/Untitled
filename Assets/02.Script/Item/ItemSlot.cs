using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public Item item;

    public delegate void ClickEvent(Item i);
    public ClickEvent itemClicked;

    public void OnPointerClick(PointerEventData eventData) { itemClicked(item); }
}
