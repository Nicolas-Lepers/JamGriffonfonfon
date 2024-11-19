using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDraggedHandler : MonoBehaviour
{
    public static CardDraggedHandler Instance;

    private void Awake()
    {
        Instance = this;
    }
}
