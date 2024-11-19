using System;

[Serializable]
public class EffectQuitAllGoblins : IIrrationEffect
{
    public void ActivateEffect(int cardIndex)
    {
        for (int i = GameManager.Instance.CardsInn.Count - 1; i >= 0; i--)
        {
            var card = GameManager.Instance.CardsInn[i];
            if (card.CardDataRef.IsGoblin) GameManager.Instance.CardLeaveInn(card);
        }
    }
}