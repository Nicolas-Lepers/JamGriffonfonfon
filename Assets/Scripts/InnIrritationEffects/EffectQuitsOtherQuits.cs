using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class EffectQuitsOtherQuits : IIrrationEffect
{
    [field: SerializeField] public int OtherOffset { get; private set; } = 1;

    public IEnumerator ActivateEffect(int cardIndex)
    {
        GameManager.Instance.CardLeaveInn(cardIndex);


        if (GameManager.Instance.CardsInn.Count > cardIndex + OtherOffset && cardIndex + OtherOffset >= 0)
        {
            yield return new WaitForSeconds(.2f);
            GameManager.Instance.CardLeaveInn(cardIndex + OtherOffset);
        }
    }
}