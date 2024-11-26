using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class EffectQuitsAndNewComes : IIrrationEffect
{
    public enum PositionType
    {
        SAME = 0,
        FIRST = 1,
    }

    [field: SerializeField] public PositionType Position { get; set; }

    public IEnumerator ActivateEffect(int cardIndex)
    {
        var gameManager = GameManager.Instance;
        gameManager.CardsInn[cardIndex].CardDataRef.RestartCheckCondition = true;

        Vector2 posToMoveCard = gameManager.CardsInn[cardIndex].transform.position;
        gameManager.CardGoDeck(cardIndex,false);
        var index = Position == PositionType.SAME ? cardIndex : 0;

        yield return new WaitForSeconds(.2f);

        gameManager.AddCardFromDeskToInn(index, posToMoveCard);
        gameManager.CheckConditionInInnAtIndex(index);
    }
}