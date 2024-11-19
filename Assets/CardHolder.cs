using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        print("enter auberge");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("exit auberge");
    }
}
