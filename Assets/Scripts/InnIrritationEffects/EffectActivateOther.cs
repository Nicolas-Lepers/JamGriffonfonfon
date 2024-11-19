using System;
using UnityEngine;

[Serializable]
public class EffectActivateOther : IIrrationEffect
{
    [field: SerializeField] public int OtherOffset { get; private set; } = 1;

    public void ActivateEffect(int cardIndex)
    {
         var index = cardIndex+OtherOffset;
         var other = GameManager.Instance.CardsInn[index];
         other.CardDataRef.IrritationEffect.ActivateEffect(index);
    }
}