using System;
using UnityEngine;

[Serializable]
public class EffectQuitsOtherQuits : IIrrationEffect
{
    [field: SerializeField] public int OtherOffset { get; private set; } = 1;

    public void ActivateEffect(int cardIndex)
    {
        // PSEUDO CODE
        // GameManager.Instance.QuitAt(cardIndex);
        // GameManager.Instance.QuitAt(cardIndex+OtherOffset);
    }
}