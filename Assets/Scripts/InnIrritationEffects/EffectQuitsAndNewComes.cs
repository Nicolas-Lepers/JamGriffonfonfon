using System;
using UnityEngine;

[Serializable]
public class EffectQuitsAndNewComes : IIrrationEffect
{
    public enum PositionType
    {
        SAME = 0,
        FIRST = 1,
    }
    
    [field:SerializeField] public PositionType Position { get; set; }
    
    public void ActivateEffect(int cardIndex)
    {
        // PSEUDO CODE
        // GameManager.Instance.QuitAt(cardIndex);
        // var newCard = GameManager.Instance.GetFirstFromDeck();
        // var index = Position == SAME ? cardIndex : 0;
        // GameManager.Instance.InsertCardAt(cardIndex);
    }
}