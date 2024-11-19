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
        GameManager.Instance.CardLeaveInn(cardIndex);
         var index = Position == PositionType.SAME ? cardIndex : 0;
         GameManager.Instance.AddCardFromDeskInInn(index);
    }
}