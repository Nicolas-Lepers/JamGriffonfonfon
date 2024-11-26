using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectPositionSwap : IIrrationEffect
{
    public enum TargetPosition
    {
        FIRST = 0,
        RANDOM = 1,
        LAST = 2,
    }

    private Dictionary<TargetPosition, Func<int, int>> _cardIndexGetter =
        new Dictionary<TargetPosition, Func<int, int>>()
        {
            { TargetPosition.FIRST, cardsAmount=> 0 },
            { TargetPosition.RANDOM, cardsAmount => UnityEngine.Random.Range(0,cardsAmount) },
            { TargetPosition.LAST, cardsAmount => GameManager.Instance.CardsInn.Count-1 },
        };

    [SerializeField] private TargetPosition _positionType;

    public IEnumerator ActivateEffect(int cardIndex)
    {
        var gameManager = GameManager.Instance;
        gameManager.CardsInn[cardIndex].CardDataRef.RestartCheckCondition = true;

        var otherIndex = _cardIndexGetter[_positionType](gameManager.CardsInn.Count);
        gameManager.SwitchCard(gameManager.CardsInn[cardIndex], otherIndex);
        yield return new WaitForSeconds(0.5f);
        gameManager.CardLeaveInn(0);
    }
}