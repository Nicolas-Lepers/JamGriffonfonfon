using System;


[Serializable]
public class EffectBarToDeck : IIrrationEffect
{
    public void ActivateEffect(int cardIndex)
    {
        GameManager.Instance.MoveAllCardInDeck();
    }
}