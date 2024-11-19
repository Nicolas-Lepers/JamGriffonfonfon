using System;
using UnityEngine;

[Serializable]
public class NextToConsumable : IIrritationCondition
{
    [field: SerializeField] Consumable Consumable { get; set; }

    public bool IsIrritated(int cardIndex)
    {
        // PSEUDO CODE
        //var previousCard = MainGame.Instance.GetCard(cardIndex+1);
        //var nextCard = MainGame.Instance.GetCard(cardIndex-1);
        //return previousCard.Consumable == Consumable || nextCard.Consumable == Consumable;

        return true;
    }
}