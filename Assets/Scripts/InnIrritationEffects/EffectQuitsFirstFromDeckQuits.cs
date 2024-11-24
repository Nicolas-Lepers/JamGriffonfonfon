using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class EffectQuitsFirstFromDeckQuits : IIrrationEffect
{
    public IEnumerator ActivateEffect(int cardIndex)
    {
        GameManager.Instance.CardLeaveInn(cardIndex);
        yield return new WaitForSeconds(.2f);
        GameManager.Instance.MoveFirstDeckCardToDiscardPile();
    }
}