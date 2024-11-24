using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class EffectTest : IIrrationEffect
{
    public IEnumerator ActivateEffect(int cardIndex)
    {
        //GameManager.Instance.ReplaceCardToBar(GameManager.Instance.CardsInn[cardIndex]);
        yield return null;
    }
}
