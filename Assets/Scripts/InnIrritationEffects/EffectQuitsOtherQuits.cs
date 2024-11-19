using System;
using UnityEngine;

[Serializable]
public class EffectQuitsOtherQuits : IIrrationEffect
{
    [field: SerializeField] public int OtherOffset { get; private set; } = 1;

    public void ActivateEffect(int cardIndex)
    {
        GameManager.Instance.CardLeaveInn(cardIndex);
        if (GameManager.Instance.CardsInn.Count < cardIndex + OtherOffset && cardIndex + OtherOffset >= 0)
            GameManager.Instance.CardLeaveInn(cardIndex+OtherOffset);
    }
}