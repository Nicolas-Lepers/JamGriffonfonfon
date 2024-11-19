using System;

[Serializable]
public class FloorLast : IIrritationCondition
{
    public bool IsIrritated(int cardIndex)
    {
        return true;
        // PSEUDO CODE
        // return cardIndex == MainGame.Instance.LastFloorIndex;
    }
}