using System;
using UnityEngine;

[Serializable]
public class NextToConsumable : IIrritationCondition
{
    [field: SerializeField] Consumable Consumable { get; set; }

    public bool IsIrritated(int cardIndex)
    {
        var hasPreviousCard = false;
        if (cardIndex < GameManager.Instance.CardsInn.Count - 1)
        {
            var previousCard = GameManager.Instance.CardsInn[cardIndex+1].CardDataRef;
            hasPreviousCard = previousCard.Consumable == Consumable;
        }
        var hasNextCard = false;
        if (cardIndex > 0)
        {
            var nextCard = GameManager.Instance.CardsInn[cardIndex - 1].CardDataRef;
            hasNextCard = nextCard.Consumable == Consumable;
        }
        return hasPreviousCard || hasNextCard;
    }
}