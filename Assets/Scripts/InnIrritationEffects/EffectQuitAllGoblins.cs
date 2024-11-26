using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class EffectQuitAllGoblins : IIrrationEffect
{
    public IEnumerator ActivateEffect(int cardIndex)
    {
        var wait = new WaitForSeconds(.2f);
        for (int i = GameManager.Instance.CardsInn.Count - 1; i >= 0; i--)
        {
            if(i >= GameManager.Instance.CardsInn.Count)
                i = GameManager.Instance.CardsInn.Count;

            var card = GameManager.Instance.CardsInn[i];
            if (card.CardDataRef.IsGoblin) GameManager.Instance.CardLeaveInn(card);
            yield return wait;
        }
        yield return null;
    }
}