using System;

[Serializable]
public class EffectQuits : IIrrationEffect
{
    public void ActivateEffect(int cardIndex)
    {
         GameManager.Instance.CardLeaveInn(cardIndex);
    }
}