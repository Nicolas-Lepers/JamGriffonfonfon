using System;

[Serializable]
public class EffectQuitsFirstFromDeckQuits : IIrrationEffect
{
    public void ActivateEffect(int cardIndex)
    {
        GameManager.Instance.CardLeaveInn(cardIndex);
        GameManager.Instance.MoveFirstDeckCardToDiscardPile();
    }
}