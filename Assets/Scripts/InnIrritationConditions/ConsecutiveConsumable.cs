using System;
using UnityEngine;

[Serializable]
public class ConsecutiveConsumable : IIrritationCondition
{
    [SerializeField] private int _minConsecutiveConsumable;
    [SerializeField] private ConsumableBoth _targetConsumable;
    
    public bool IsIrritated(int cardIndex)
    {
         if (_targetConsumable == ConsumableBoth.BEER) return GameManager.Instance.GetNumberOfBeer(true) >= _minConsecutiveConsumable;
         if (_targetConsumable == ConsumableBoth.FOOD) return GameManager.Instance.GetNumberOfFood(true) >= _minConsecutiveConsumable;
         return Mathf.Max(GameManager.Instance.GetNumberOfBeer(true), GameManager.Instance.GetNumberOfBeer(true)) >= _minConsecutiveConsumable;
    }
}