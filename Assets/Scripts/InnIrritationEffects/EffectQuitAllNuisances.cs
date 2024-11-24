using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class EffectQuitAllNuisances : IIrrationEffect
{
    [SerializeField] private NuisanceType _nuisance;
    
    public IEnumerator ActivateEffect(int cardIndex)
    {
        GameManager.Instance.CardLeaveInn(cardIndex);
        
        yield return new WaitForSeconds(0.2f);

        yield return GameManager.Instance.CheckNuisance(_nuisance);
    }
}