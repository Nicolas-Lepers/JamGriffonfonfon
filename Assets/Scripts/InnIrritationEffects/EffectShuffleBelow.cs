using System;
using UnityEngine;

[Serializable]
public class EffectShuffleBelow : IIrrationEffect
{
    [SerializeField] private int _amountToShuffle;
    public void ActivateEffect(int cardIndex)
    {
        GameManager.Instance.ShuffleInnCardsInRange(cardIndex-1-_amountToShuffle, cardIndex-1);
    }
}