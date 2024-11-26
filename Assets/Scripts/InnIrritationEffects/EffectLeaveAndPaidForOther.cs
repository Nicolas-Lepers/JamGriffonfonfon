using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class EffectLeaveAndPaidForOther : IIrrationEffect
{
    public IEnumerator ActivateEffect(int cardIndex)
    {
        var gameManager = GameManager.Instance;
        gameManager.CardsInn[cardIndex].CardDataRef.RestartCheckCondition = true;

        var targetIndex = gameManager.CardsInn.Count - 1;

        var card = gameManager.GetCardInBar();
        while (card == null)
            card = gameManager.GetCardInBar();

        gameManager.SwitchCardFromBar(card, targetIndex,cardIndex);
        yield return new WaitForSeconds(0.5f);
        gameManager.CheckConditionInInnAtIndex(targetIndex);
    }
}
