using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualHandler : MonoBehaviour
{
    public static CardVisualHandler Instance;

    private void Awake()
    {
        Instance = this;
    }
}
