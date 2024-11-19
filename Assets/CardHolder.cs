using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.CurrentCardDragging != null)
        {
            print("heklo");
        }
        print($"enter auberge : {GameManager.Instance.CurrentCardDragging}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("exit auberge");
    }
}
