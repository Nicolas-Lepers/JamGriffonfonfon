using AntoineFoucault.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [SerializeField] Transform _canvasRef;

    [SerializeField] Transform _discardPilePos;
    [SerializeField] Transform _deckPos;

    [SerializeField] List<Transform> _cardBarTargetPos = new List<Transform>();
    [SerializeField] Transform _cardInnTargetDefaultPos;
    [SerializeField] float _offsetPosCardInn = 0.5f;

    [SerializeField] GameObject _prefabCardVisual;

    [Header("Cards")]
    [SerializeField] List<CardData> _cardsData = new List<CardData>();

    [SerializeField] List<CardInfo> _cardsInn = new List<CardInfo>();
    [SerializeField] List<CardInfo> _cardsBar = new List<CardInfo>();
    [SerializeField] List<CardInfo> _cardsDeck = new List<CardInfo>();
    [SerializeField] List<CardInfo> _discardPile = new List<CardInfo>();

    public GameObject CurrentCardDragging { get; private set; }
    public List<CardInfo> CardsInn => _cardsInn;
    public List<CardInfo> CardsBar => _cardsBar;
    public List<CardInfo> CardsDeck => _cardsDeck;


    [SerializeField] int _numnerCardInDiscardPileToLose = 8;
    [SerializeField] int _numberCardInInnToWin = 7;
    private int _beerCount = 0;
    private int _foodCount = 0;
    private int _beerNeighborCount = 0;
    private int _foodNeighborCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public bool CanSelectedCard = true;
    private void Start()
    {
        for (int i = 0; i < _cardsData.Count; i++)
        {
            GameObject go = Instantiate(_prefabCardVisual, _deckPos.transform.position, Quaternion.identity, _deckPos);
            //go.SetActive(false);

            if (go.TryGetComponent(out CardInfo card) != false)
            {
                card.CardDataRef = _cardsData[i];
                _cardsDeck.Add(card);
            }
        }

        StartCoroutine(PhaseBar());
    }
    public void CheckEndGame()
    {
        if (_discardPile.Count >= _numnerCardInDiscardPileToLose)
            Debug.Log("Loser");

        if (_cardsDeck.Count >= 0)
            return;

        if (_cardsInn.Count < _numberCardInInnToWin)
        {
            Debug.Log("Loser");
            return;
        }
        Debug.Log("Winer");

    }

    public IEnumerator PhaseBar()
    {
        CanSelectedCard = true;
        Debug.Log("bar");
        var wait = new WaitForSeconds(1);
        yield return wait;

        while (_cardsBar.Count < 4)
        {
            int cardsInBar = _cardsBar.Count;
            CardInfo cardInfo = _cardsDeck[^1];
            _cardsDeck.RemoveAt(_cardsDeck.Count - 1);

            int rand = Random.Range(0, _cardsDeck.Count - 1);

            //add to bar
            cardInfo.CardDataRef = _cardsData[rand];
            cardInfo.CardMovement.CardVisual.CardImage.sprite = cardInfo.CardDataRef.Sprite;
            //CardInfo cardData = _cardsDeck[rand];
            _cardsBar.Add(cardInfo);
            //remove from deck
            _cardsDeck.RemoveAt(rand);

            // cardInfo.GetComponent<CardMovement>()
            if (_barIndexNul < 0)
                cardInfo.transform.position = _cardBarTargetPos[cardsInBar].position;
            else
                cardInfo.transform.position = _cardBarTargetPos[_barIndexNul].position;

            cardInfo.gameObject.SetActive(true);
            yield return wait;
        }


        if (_cardsInn.Count <= 0)
            yield return null;

        if (GetNumberOfBattleInBar() >= 3)
        {

        }
        else if (GetNumberOfNoiseInBar() >= 3)
        {

        }
        else if (GetNumberOfNoiseInBar() >= 3)
        {

        }

        //for (int i = 0; i < _cardsBar.Count; i++)
        //{
        //    //check all condition in bar
        //    if (_cardsBar[i].CardDataRef.BarIrritationCondition == false)
        //        continue;
        //    _cardsBar[i].CardDataRef.IrritationEffect.ActivateEffect(i);
        //    yield return wait;
        //}
    }
    public IEnumerator PhaseInn()
    {
        CanSelectedCard = false;

        var wait = new WaitForSeconds(1);

        for (int i = 0; i < _cardsInn.Count; i++)
        {
            //check consequance in Inn
            if (_cardsInn[i].CardDataRef.InnIrritationCondition.IsIrritated(i) == false)
                continue;

            _cardsInn[i].CardDataRef.IrritationEffect.ActivateEffect(i);
            yield return wait;
        }

        StartCoroutine(PhaseBar());
    }

    int _barIndexNul = -1;
    public void PutCardInInn(CardInfo cardInfo)
    {
        _cardsInn.Add(cardInfo);
        int value = GetCardIndexInBar(cardInfo);
        _cardsBar[value] = null;
        _barIndexNul = value;
        //_cardsBar.Remove(cardInfo);
        StartCoroutine(PhaseInn());
    }

    public int GetNumberOfGolbin()
    {
        int count = 0;
        for (int i = 0; i < _cardsInn.Count; i++)
        {
            if (_cardsInn[i].CardDataRef.IsGoblin)
                count++;
        }
        return count;
    }

    public int GetNumberOfFood(bool needFollow = false)
    {
        int count = 0;
        int bestGroup = 0;
        for (int i = 0; i < _cardsInn.Count; i++)
        {
            if (_cardsInn[i].CardDataRef.Consumable == Consumable.FOOD && needFollow == false)
            {
                count++;
                continue;
            }

            if (_cardsInn[i].CardDataRef.Consumable != Consumable.FOOD && needFollow == true)
            {
                if (count > bestGroup)
                    bestGroup = count;
                continue;
            }
            else if (_cardsInn[i].CardDataRef.Consumable == Consumable.FOOD && needFollow == true)
                count++;

        }


        if (needFollow == true)
            return bestGroup;
        else
            return count;
    }
    public int GetNumberOfBeer(bool needFollow = default)
    {
        int count = 0;
        int bestGroup = 0;
        for (int i = 0; i < _cardsInn.Count; i++)
        {
            if (_cardsInn[i].CardDataRef.Consumable == Consumable.BEER && needFollow == false)
            {
                count++;
                continue;
            }

            if (_cardsInn[i].CardDataRef.Consumable != Consumable.BEER && needFollow == true)
            {
                if (count > bestGroup)
                    bestGroup = count;
                continue;
            }
            else if (_cardsInn[i].CardDataRef.Consumable == Consumable.BEER && needFollow == true)
                count++;
        }

        if (needFollow == true)
            return bestGroup;
        else
            return count;
    }
    public int GetNumberOfBattleInBar()
    {
        int count = 0;
        for (int i = 0; i < _cardsBar.Count; i++)
        {
            if (_cardsBar[i].CardDataRef.Nuisance == NuisanceType.BATTLE)
                count++;
        }
        return count;
    }

    public int GetNumberOfSmellInBar()
    {
        int count = 0;
        for (int i = 0; i < _cardsBar.Count; i++)
        {
            if (_cardsBar[i].CardDataRef.Nuisance == NuisanceType.SMELL)
                count++;
        }
        return count;
    }

    public int GetNumberOfNoiseInBar()
    {
        int count = 0;
        for (int i = 0; i < _cardsBar.Count; i++)
        {
            if (_cardsBar[i].CardDataRef.Nuisance == NuisanceType.NOISE)
                count++;
        }
        return count;
    }


    public void SetCurrentCard(GameObject card)
    {
        CurrentCardDragging = card;
    }

    public void ShuffleInnCardsInRange(int min, int max)
    {
        var cardsToShuffle = new List<CardInfo>();
        for (int i = min; i <= max; i++)
        {
            if (i < 0 || i > _cardsInn.Count) continue;
            cardsToShuffle.Add(_cardsInn[i]);
        }

        cardsToShuffle.Shuffle();

        for (int i = min; i <= max; i++)
        {
            if (i < 0 || i > _cardsInn.Count) continue;
            _cardsInn[i] = cardsToShuffle[i];
        }
    }
    public void AddCardInInn(CardInfo card)
    {
        _cardsInn.Add(card);
    }

    public void AddCardInInnAtIndex(CardInfo card, int index)
    {
        _cardsInn.Insert(index, card);
    }
    public void AddCardToBar(CardInfo card)
    {
        _cardsBar.Add(card);
    }
    public void CardLeaveInn(int cardIndex)
    {
        _cardsInn.RemoveAt(cardIndex);
    }
    private CardInfo GetRandomCardInDeck()
    {
        return _cardsDeck[Random.Range(0, _cardsDeck.Count)];
    }

    public void AddCardFromDeskInInn(int index)
    {
        var randomCard = GetRandomCardInDeck();

        AddCardInInnAtIndex(randomCard, index);
        _cardsDeck.Remove(randomCard);
    }
    /// <summary>
    /// Remove one card in inn to put in the bar
    /// </summary>
    public void ReplaceCardToBar(CardInfo card)
    {
        if (_cardsInn.Contains(card) == false)
            return;

        _cardsInn.Remove(card);
        _cardsBar.Add(card);
    }

    public void CardLeaveInn(CardInfo card)
    {
        if (_cardsInn.Contains(card) == false) return;

        _cardsInn.Remove(card);
    }

    public void MoveFirstDeckCardToDiscardPile()
    {
        _discardPile.Add(_cardsDeck[0]);
        _cardsDeck.Remove(_cardsDeck[0]);
    }

    public void MoveAllCardInDeck()
    {
        for (int i = 0; i < _cardsBar.Count; i++)
        {
            int rand = Random.Range(0, _cardsDeck.Count);
            _cardsDeck.Insert(rand, _cardsBar[i]);
            _cardsBar.RemoveAt(i);
            i--;
        }
    }

    public void CardLeaveInnWithNeihgbourUp(CardInfo card, int nbNeihgbourUp)
    {
        if (_cardsInn.Contains(card) == false) return;

        int value = GetCardIndexInInn(card);

        for (int i = nbNeihgbourUp - 1; i >= 0; i--)
        {
            int neighbour = value + i;
            if (neighbour >= _cardsInn.Count)
                break;

            CardLeaveInn(_cardsInn[neighbour]);
        }

        _cardsInn.Remove(card);
    }

    public void CheckNuisance(NuisanceType nuisanceToCheck)
    {
        for (int i = _cardsInn.Count - 1; i >= 0; i--)
        {
            CardData cardData = _cardsInn[i].CardDataRef;
            if ((int)nuisanceToCheck != (int)cardData.BarIrritationCondition)
                continue;

            cardData.IrritationEffect.ActivateEffect(i);
        }
    }

    public void SetCardToDiscardPile(CardData card)
    {

    }


    public void SwitchCard(CardInfo card, int index)
    {
        CardInfo temp = _cardsInn[index];
        int targetIndex = GetCardIndexInInn(temp);
        _cardsInn[index] = card;
        _cardsInn[targetIndex] = temp;
    }

    private int GetCardIndexInInn(CardInfo card)
    {
        return _cardsInn.IndexOf(card);
    }
    private int GetCardIndexInBar(CardInfo card)
    {
        return _cardsBar.IndexOf(card);
    }
    private CardInfo GetCardIndexInDeck(CardInfo card)
    {
        return _cardsDeck[Random.Range(0, _cardsDeck.Count)];
    }

    public CardInfo GetRandomCardInBar()
    {
        return _cardsBar[Random.Range(0, _cardsBar.Count)];
    }



    public void SetPositionInInnForAllCard()
    {
        for (int i = 0; i < _cardsInn.Count; i++)
        {
            Transform card = _cardsInn[i].transform;
            Vector2 pos = card.position;

            pos = _cardInnTargetDefaultPos.position;
            pos.y += _offsetPosCardInn * i;

            card.position = pos;
        }
    }

    public void SetPositionInInnForCardAt(int index)
    {
        Transform card = _cardsInn[index].transform;
        Vector2 pos = card.position;

        pos = _cardInnTargetDefaultPos.position;
        pos.y += _offsetPosCardInn * index;

        card.position = pos;
    }

    public void SetPositionInInnForCard(CardInfo cardInfo)
    {
        int index = GetCardIndexInInn(cardInfo);

        Transform card = _cardsInn[index].transform;
        Vector2 pos = card.position;

        pos = _cardInnTargetDefaultPos.position;
        pos.y += _offsetPosCardInn * index;

        card.position = pos;
    }
}
