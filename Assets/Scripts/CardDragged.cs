using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragged : MonoBehaviour
{
    public static CardDragged Instance;

    private void Awake()
    {
        Instance = this;
    }
}
