using System;

[Serializable]
public class EffectQuitsFirstFromDeckQuits : IIrrationEffect
{
    public void ActivateEffect(int cardIndex)
    {
        // PSEUDO CODE
        // GameManager.Instance.QuitAt(cardIndex);
        // GameManager.Instance.RemoveFirstFromDeck();
    }
}