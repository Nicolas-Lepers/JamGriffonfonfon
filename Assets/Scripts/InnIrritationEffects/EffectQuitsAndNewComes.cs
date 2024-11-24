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
        Vector2 posToMoveCard = GameManager.Instance.CardsInn[cardIndex].transform.position;
        GameManager.Instance.CardGoDeck(cardIndex,false);
        var index = Position == PositionType.SAME ? cardIndex : 0;

        yield return new WaitForSeconds(.2f);

        GameManager.Instance.AddCardFromDeskToInn(index, posToMoveCard);
    }
}