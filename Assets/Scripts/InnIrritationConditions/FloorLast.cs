using System;

[Serializable]
public class FloorLast : IIrritationCondition
{
    public bool IsIrritated(int cardIndex)
    {
        return cardIndex == GameManager.Instance.CardsInn.Count - 1;
    }
}