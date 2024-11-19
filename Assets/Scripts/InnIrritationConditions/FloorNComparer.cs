using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

[Serializable]
public class FloorNComparer : IIrritationCondition
{
    private Dictionary<FloorComparerType, Func<int, int, bool>> _floorTypeGetter =
        new Dictionary<FloorComparerType, Func<int, int, bool>>()
        {
            { FloorComparerType.GreaterThanTargetFloor, (target, current) => current > target },
            { FloorComparerType.GreaterOrEqualsThanTargetFloor, (target, current) => current >= target },
            { FloorComparerType.EqualsToTargetFloor, (target, current) => current == target },
            { FloorComparerType.LowerOrEqualsThanTargetFloor, (target, current) => current <= target },
            { FloorComparerType.LowerThanTargetFloor, (target, current) => current < target },
        };

    [field:SerializeField] public int TargetFloor { get; private set; }
    [field:SerializeField] public FloorComparerType FloorType { get; private set; }

    public bool IsIrritated(int cardIndex)
    {
        return _floorTypeGetter[FloorType](TargetFloor, cardIndex);
    }
}