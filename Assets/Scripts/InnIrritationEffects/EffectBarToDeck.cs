using System;
using System.Collections;


[Serializable]
public class EffectBarToDeck : IIrrationEffect
{
    public IEnumerator ActivateEffect(int cardIndex)
    {
        GameManager.Instance.MoveAllCardInDeck();
        yield return null;
    }
}