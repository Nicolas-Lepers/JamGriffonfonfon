using System;

[Serializable]
public class EffectQuitAllGoblins : IIrrationEffect
{
    public void ActivateEffect(int cardIndex)
    {
        // PSEUDO CODE
        /*foreach (var card in GameManager.Instance.CardsHostel)
        {
            if (card.IsGoblin) card.Quit();
        }*/
    }
}