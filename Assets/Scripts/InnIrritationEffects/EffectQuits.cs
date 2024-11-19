using System;

[Serializable]
public class EffectQuits : IIrrationEffect
{
    public void ActivateEffect(int cardIndex)
    {
        // PSEUDO CODE
        // GameManager.Instance.QuitAt(cardIndex);
    }
}