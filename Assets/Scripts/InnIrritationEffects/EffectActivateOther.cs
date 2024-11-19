using System;
using UnityEngine;

[Serializable]
public class EffectActivateOther : IIrrationEffect
{
    [field: SerializeField] public int OtherOffset { get; private set; } = 1;

    public void ActivateEffect(int cardIndex)
    {
        // PSEUDO CODE
        // var index = cardIndex+OtherOffset;
        // var other = MainGame.Instance.GetCardAt(index);
        // other.ActivateEffect(index);
    }
}