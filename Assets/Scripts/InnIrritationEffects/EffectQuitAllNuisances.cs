using System;
using UnityEngine;


[Serializable]
public class EffectQuitAllNuisances : IIrrationEffect
{
    [SerializeField] private NuisanceType _nuisance;
    
    public void ActivateEffect(int cardIndex)
    {
        GameManager.Instance.CardLeaveInn(cardIndex);
        GameManager.Instance.CheckNuisance(_nuisance);
    }
}