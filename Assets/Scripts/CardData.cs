using UnityEngine;

/// <summary>
/// The data of a card
/// </summary>
[CreateAssetMenu(fileName = "CardData", menuName = "Card Data")]
public class CardData : ScriptableObject
{
    [field:SerializeField] public Sprite Sprite { get; private set; }
    [field:SerializeField] public string Name { get; private set; }
    [field:SerializeField] public Consumable BroughtConsumable { get; private set; }
    [field:SerializeField] public NuisanceType BroughtNuisance { get; private set; }
    [field: SerializeField] public int NuisancePower { get; private set; } = 1;
    [field: SerializeField] public bool IsGoblin { get; private set; } = false;
    [field:SerializeField] public NuisanceType BarIrritationCondition { get; private set; } // TO CHANGE - The nuisance type can be null here
    [field:SerializeReference, SerializeField, SubclassSelector] public IIrritationCondition InnIrritationCondition { get; private set; }
    [field:SerializeReference, SerializeField, SubclassSelector] public IIrrationEffect IrritationEffect { get; private set; }
}
