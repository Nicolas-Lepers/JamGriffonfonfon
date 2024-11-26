using UnityEngine;

/// <summary>
/// The data of a card
/// </summary>
[CreateAssetMenu(fileName = "CardData", menuName = "Card Data")]
public class CardData : ScriptableObject
{
    [field:SerializeField] public Sprite Sprite { get; private set; }
    [field:SerializeField] public string Name { get; private set; }
    [field:SerializeField] public Consumable Consumable { get; private set; }
    [field:SerializeField] public NuisanceType Nuisance { get; private set; }
    [field: SerializeField] public int NuisancePower { get; private set; } = 1;
    [field: SerializeField] public bool IsGoblin { get; private set; } = false;
    [field:SerializeField] public NuisanceTypeNullable BarIrritationCondition { get; private set; }
    [field:Space(10)]
    [field:SerializeReference, SerializeField, SubclassSelector] public IIrritationCondition InnIrritationCondition { get; private set; }
    [field:Space(10)]
    [field:SerializeReference, SerializeField, SubclassSelector] public IIrrationEffect IrritationEffect { get; private set; }
    [field: SerializeField] public bool RestartCheckCondition { get; set; } = false;

}
