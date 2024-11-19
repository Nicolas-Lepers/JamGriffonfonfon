using System;

[Serializable] 
public class EffectPositionSwap : IIrrationEffect
{
    public enum TargetPosition
    {
        FIRST = 0,
        RANDOM = 1,
    }

    public void ActivateEffect(int cardIndex)
    {
        throw new System.NotImplementedException();
    }
}