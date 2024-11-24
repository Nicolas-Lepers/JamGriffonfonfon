using System;
using System.Collections;

[Serializable]
public class EffectQuits : IIrrationEffect
{
    public IEnumerator ActivateEffect(int cardIndex)
    {
         GameManager.Instance.CardLeaveInn(cardIndex);
        yield return null;
    }
}