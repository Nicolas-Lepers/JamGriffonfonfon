using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] 
public class EffectPositionSwap : IIrrationEffect
{
    public enum TargetPosition
    {
        FIRST = 0,
        RANDOM = 1,
    }
    
    private Dictionary<TargetPosition, Func<int, int>> _cardIndexGetter =
        new Dictionary<TargetPosition, Func<int, int>>()
        {
            { TargetPosition.FIRST, cardsAmount=> 0 },
            { TargetPosition.RANDOM, cardsAmount => UnityEngine.Random.Range(0,cardsAmount) },
        };

    [SerializeField] private TargetPosition _positionType;

    public void ActivateEffect(int cardIndex)
    {
        var otherIndex = _cardIndexGetter[_positionType](GameManager.Instance.CardsInn.Count);
        GameManager.Instance.SwitchCard(GameManager.Instance.CardsInn[cardIndex], otherIndex);
    }
}