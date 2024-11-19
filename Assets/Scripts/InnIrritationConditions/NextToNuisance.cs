using System;
using UnityEngine;

[Serializable]
public class NextToNuisance : IIrritationCondition
{
    [field: SerializeField] NuisanceType Nuisance { get; set; }

    public bool IsIrritated(int cardIndex)
    {
        // PSEUDO CODE
        //var previousCard = MainGame.Instance.GetCard(cardIndex+1);
        //var nextCard = MainGame.Instance.GetCard(cardIndex-1);
        //return previousCard.Nuisance == Nuisance || nextCard.Nuisance == Nuisance;

        return true;
    }
}