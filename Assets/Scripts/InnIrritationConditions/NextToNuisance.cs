using System;
using UnityEngine;

[Serializable]
public class NextToNuisance : IIrritationCondition
{
    [field: SerializeField] NuisanceType Nuisance { get; set; }

    public bool IsIrritated(int cardIndex)
    {
        var hasPreviousCard = false;
        if (cardIndex+1 <= GameManager.Instance.CardsInn.Count - 1 && cardIndex+1 > 0)
        {
            var previousCard = GameManager.Instance.CardsInn[cardIndex+1].CardDataRef;
            hasPreviousCard = previousCard.Nuisance == Nuisance;
        }
        var hasNextCard = false;
        if (cardIndex-1 <= GameManager.Instance.CardsInn.Count - 1 && cardIndex-1 > 0)
        {
            var nextCard = GameManager.Instance.CardsInn[cardIndex-1].CardDataRef;
            hasNextCard = nextCard.Nuisance == Nuisance;
        }
        return hasPreviousCard || hasNextCard;
    }
}