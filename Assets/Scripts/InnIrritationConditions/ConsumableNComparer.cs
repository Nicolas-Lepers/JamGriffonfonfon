using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConsumableNComparer : IIrritationCondition
{
    private Dictionary<TargetComparerType, Func<int, int, bool>> _floorTypeGetter =
        new Dictionary<TargetComparerType, Func<int, int, bool>>()
        {
            { TargetComparerType.GreaterThanTarget, (target, current) => current > target },
            { TargetComparerType.GreaterOrEqualsThanTarget, (target, current) => current >= target },
            { TargetComparerType.EqualsToTarget, (target, current) => current == target },
            { TargetComparerType.LowerOrEqualsThanTarget, (target, current) => current <= target },
            { TargetComparerType.LowerThanTarget, (target, current) => current < target },
        };
    

    [field:SerializeField] public Consumable TargetConsumable { get; private set; }
    [field:SerializeField] public int TargetAmount { get; private set; }
    [field:SerializeField] public TargetComparerType ComparerType { get; private set; }
    
    public bool IsIrritated(int cardIndex)
    {
        int beerAmount = GameManager.Instance.GetNumberOfBeer();
        int foodAmount = GameManager.Instance.GetNumberOfFood();
        var currentConsumable = TargetConsumable == Consumable.BEER ? beerAmount : foodAmount;
        
        return _floorTypeGetter[ComparerType](TargetAmount, currentConsumable);
    }
}