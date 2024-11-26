using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class EffectActivateOther : IIrrationEffect
{
    [field: SerializeField] public int OtherOffset { get; private set; } = 1;

    public IEnumerator ActivateEffect(int cardIndex)
    {
        var index = cardIndex + OtherOffset;
        if (index < GameManager.Instance.CardsInn.Count && index >= 0)
        {
            CardInfo other = GameManager.Instance.CardsInn[index];
            other.CardDataRef.IrritationEffect.ActivateEffect(index);
        }
        yield return null;
    }
}