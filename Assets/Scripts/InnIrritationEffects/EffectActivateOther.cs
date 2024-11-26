using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class EffectActivateOther : IIrrationEffect
{
    [field: SerializeField] public int OtherOffset { get; private set; } = 1;

    public IEnumerator ActivateEffect(int cardIndex)
    {
        if (cardIndex <= 2)
            yield break;

        var index = cardIndex + OtherOffset;
        CardInfo other = GameManager.Instance.CardsInn[index];
        other.CardDataRef.IrritationEffect.ActivateEffect(index);
        yield return null;
    }
}