using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class EffectShuffleBelow : IIrrationEffect
{
    [SerializeField] private int _amountToShuffle;
    public IEnumerator ActivateEffect(int cardIndex)
    {
        GameManager.Instance.ShuffleInnCardsInRange(cardIndex-_amountToShuffle, cardIndex-1);
        CardHolderInn.Instance.ReplaceCardInOrder(GameManager.Instance.CardsInn[GameManager.Instance.CardsInn.Count - 1].CardMovement);

        yield return null;
    }
}