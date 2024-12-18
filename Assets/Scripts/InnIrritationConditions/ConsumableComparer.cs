using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConsumableComparer : IIrritationCondition
{
    private Dictionary<ConsumableComparerType, Func<int, int, bool>> _consumalbeTypeGetter =
        new Dictionary<ConsumableComparerType, Func<int, int, bool>>()
        {
            { ConsumableComparerType.MoreBeer, (beer, food) => beer > food },
            { ConsumableComparerType.MoreFood, (beer, food) => beer < food },
            { ConsumableComparerType.EqualAmount, (beer, food) => beer == food },

        };
    
    [field:SerializeField] public ConsumableComparerType FoodType { get; private set; }
    
    public bool IsIrritated(int cardIndex)
    {
        int beerAmount = GameManager.Instance.GetNumberOfBeer();
        int foodAmount = GameManager.Instance.GetNumberOfFood();
        
        return _consumalbeTypeGetter[FoodType](beerAmount, foodAmount);
    }
}