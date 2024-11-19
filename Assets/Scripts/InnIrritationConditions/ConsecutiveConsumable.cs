using System;
using UnityEngine;

[Serializable]
public class ConsecutiveConsumable : IIrritationCondition
{
    [SerializeField] private int _minConsecutiveConsumable;
    [SerializeField] private Consumable _targetConsumable;
    
    public bool IsIrritated(int cardIndex)
    {
        var followingConsumable = _targetConsumable == Consumable.BEER ? GameManager.Instance.GetNumberOfBeer(true) : GameManager.Instance.GetNumberOfFood(true);
        return followingConsumable >= _minConsecutiveConsumable;
    }
}