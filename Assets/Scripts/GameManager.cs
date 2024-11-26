using System;
using AntoineFoucault.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening.Core.Easing;
using static EffectQuitsAndNewComes;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action OnWin;
    public Action OnLose;


    [SerializeField] Transform _canvasRef;

    [SerializeField] Transform _discardPilePos;
    [SerializeField] Transform _deckPos;
    public Transform DiscardPilePos => _discardPilePos;
    public Transform DeckPos => _deckPos;

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
    private bool _hasGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    public bool CanSelectedCard = true;
    private void Start()
    {
        OnWin += GameOver;
        OnLose += GameOver;

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

    private void OnDisable()
    {
        OnWin -= GameOver;
        OnLose -= GameOver;
    }


    public bool CheckEndGame()
    {
        if (_discardPile.Count >= _numnerCardInDiscardPileToLose)
        {
            OnLose?.Invoke();
            return true;
        }

        if (_cardsDeck.Count > 0)
            return false;

        if (_cardsInn.Count >= _numberCardInInnToWin)
        {
            OnWin?.Invoke();
            return true;
        }
        else
        {
            OnLose?.Invoke();
            return true;
        }
    }

    private void GameOver()
    {
        _hasGameOver = true;
    }

    public IEnumerator PhaseBar()
    {
        Debug.Log("bar");
        var wait = new WaitForSeconds(.5f);
        yield return wait;

        while (_cardsBar.Count < 4 || _barIndexNul != -1)
        {
            if (_cardsDeck.Count <= 0)
            {
                CheckEndGame();
                break;
            }

            int cardsInBar = _cardsBar.Count;
            int rand = Random.Range(0, _cardsDeck.Count - 1);
            CardInfo cardInfo = _cardsDeck[rand];

            cardInfo.CardMovement.CardVisual.CardImage.sprite = cardInfo.CardDataRef.Sprite;
            Debug.Log(_barIndexNul);


            if (_barIndexNul < 0)
                _cardsBar.Add(cardInfo);
            //remove from deck
            _cardsDeck.RemoveAt(rand);

            CardMovement move = cardInfo.CardMovement;
            move.SetSiblingIndex(50,50);
            
            if (_barIndexNul < 0)
                move.MoveToPoint(_cardBarTargetPos[cardsInBar].position, false);
            else
            {
                move.MoveToPoint(_cardBarTargetPos[_barIndexNul].position, false);
                _cardsBar[_barIndexNul] = cardInfo;
                _barIndexNul = -1;
            }

            cardInfo.gameObject.SetActive(true);
            yield return wait;

            for (int i = _cardsBar.Count - 1; i >= 0; i--)
            {
                if (_cardsBar[i] == null)
                    _barIndexNul = i;
            }
        }


        if (GetNumberOfBattleInBar() >= 3)
            CheckNuisance(NuisanceType.BATTLE);
        else if (GetNumberOfNoiseInBar() >= 3)
            CheckNuisance(NuisanceType.NOISE);
        else if (GetNumberOfNoiseInBar() >= 3)
            CheckNuisance(NuisanceType.SMELL);

        CanSelectedCard = true;
    }
    public IEnumerator PhaseInn()
    {
        Debug.Log("inn");
        CanSelectedCard = false;

        var wait = new WaitForSeconds(.5f);

        yield return wait;

        CheckConditionInInn();


        if (CheckEndGame() == false)
            StartCoroutine(PhaseBar());
    }
    public void CheckConditionInInn()
    {
        for (int i = _cardsInn.Count - 1; i >= 0; i--)
        {
            if (i > _cardsInn.Count - 1)
            {
                CheckConditionInInn();
                break;
            }

            CardData cardData = _cardsInn[i].CardDataRef;
            if (cardData.InnIrritationCondition.IsIrritated(i) == false)
                continue;

            StartCoroutine(cardData.IrritationEffect.ActivateEffect(i));

            if (cardData.RestartCheckCondition == true)
                break;
        }
    }
    public void CheckConditionInInnAtIndex(int index)
    {
        for (int i = index; i >= 0; i--)
        {
            if (i > _cardsInn.Count - 1)
            {
                CheckConditionInInn();
                break;
            }

            if (_cardsInn[i].CardDataRef.InnIrritationCondition.IsIrritated(i) == false)
                continue;

            StartCoroutine(_cardsInn[i].CardDataRef.IrritationEffect.ActivateEffect(i));
        }
    }
    int _barIndexNul = -1;
    public void PutCardInInn(CardInfo cardInfo)
    {
        //_cardsInn.Add(cardInfo);
        int value = GetCardIndexInBar(cardInfo);
        _cardsBar[value] = null;
        _barIndexNul = value;
        StartCoroutine(PhaseInn());
    }

    public void MoveCardToPoint(CardInfo cardInfo, Vector3 position, bool canInteractWithCard = false)
    {
        CardMovement move = cardInfo.CardMovement;
        move.MoveToPoint(position, canInteractWithCard);
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
            return count > bestGroup ? count : bestGroup;
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
            return count > bestGroup ? count : bestGroup;
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
            if (i < 0 || i > _cardsInn.Count - 1) continue;
            cardsToShuffle.Add(_cardsInn[i]);
        }

        cardsToShuffle.Shuffle();

        for (int i = min; i <= max; i++)
        {
            if (i < 0 || i > _cardsInn.Count - 1) continue;
            _cardsInn[i] = cardsToShuffle[min < 0 ? i : (i - min)];
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
    public void CardLeaveInn(int cardIndex, bool replace = true)
    {
        if (cardIndex >= _cardsInn.Count || cardIndex < 0) return;
        var card = _cardsInn[cardIndex];
        if (_cardsInn.Contains(card) == false) return;

        MoveCardToPoint(card, _discardPilePos.position, true);
        _cardsInn[cardIndex].CardMovement.SetSiblingIndex(90, 90);
        _cardsInn.RemoveAt(cardIndex);
        if (_cardsInn.Count > 0 && replace == true)
            CardHolderInn.Instance.ReplaceCardInOrder(_cardsInn[_cardsInn.Count - 1].CardMovement);
    }
    public void CardLeaveInn(CardInfo card)
    {
        if (_cardsInn.Contains(card) == false) return;
        MoveCardToPoint(card, _discardPilePos.position, true);
        card.CardMovement.SetSiblingIndex(90, 90);
        _cardsInn.Remove(card);
        _discardPile.Add(card);
        if (_cardsInn.Count > 0)
            CardHolderInn.Instance.ReplaceCardInOrder(_cardsInn[_cardsInn.Count - 1].CardMovement);

    }
    public void CardGoDeck(int cardIndex, bool replace = true)
    {
        MoveCardToPoint(_cardsInn[cardIndex], _deckPos.position, true);
        _cardsInn[cardIndex].CardMovement.CardVisual.CardImage.sprite = _cardsInn[cardIndex].CardBackSprite;

        int rand = Random.Range(0, _cardsDeck.Count - 1);
        _cardsDeck.Insert(rand, _cardsInn[cardIndex]);

        _cardsInn.RemoveAt(cardIndex);
        if (_cardsInn.Count > 0 && replace == true)
            CardHolderInn.Instance.ReplaceCardInOrder(_cardsInn[_cardsInn.Count - 1].CardMovement);
    }

    private CardInfo GetRandomCardInDeck()
    {
        return _cardsDeck[Random.Range(0, _cardsDeck.Count)];
    }

    public void AddCardFromDeskToInn(int index, Vector2 pos)
    {
        if (_cardsDeck.Count <= 0)
            return;

        CardInfo randomCard = GetRandomCardInDeck();

        randomCard.CardMovement.CardVisual.CardImage.sprite = randomCard.CardDataRef.Sprite;

        randomCard.CardMovement.SetSiblingIndex(50,50);
        MoveCardToPoint(randomCard, pos, true);

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

        MoveCardToPoint(card, _cardBarTargetPos[_barIndexNul].position, false);

        _cardsInn.Remove(card);
        _cardsBar[_barIndexNul] = card;
        _barIndexNul = -1;
    }



    public void AddCardToDiscardPile(CardInfo card)
    {
        _discardPile.Add(card);
    }

    public void MoveFirstDeckCardToDiscardPile()
    {
        if (_cardsDeck.Count <= 0)
            return;

        CardInfo cardInfo = _cardsDeck[0];

        cardInfo.CardMovement.CardVisual.CardImage.sprite = cardInfo.CardDataRef.Sprite;

        _discardPile.Add(cardInfo);


        MoveCardToPoint(cardInfo, _discardPilePos.position, true);

        _cardsDeck.Remove(cardInfo);
    }

    public void MoveAllCardInDeck()
    {
        for (int i = 0; i < _cardsBar.Count; i++)
        {
            if (_cardsBar[i] == null)
                continue;


            int rand = Random.Range(0, _cardsDeck.Count);
            _cardsDeck.Insert(rand, _cardsBar[i]);
            MoveCardToPoint(_cardsBar[i], _deckPos.position, true);
            _cardsBar.RemoveAt(i);
            i--;
        }
        _cardsBar.Clear();
        _barIndexNul = -1;
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

    public IEnumerator CheckNuisance(NuisanceType nuisanceToCheck)
    {
        for (int i = _cardsInn.Count - 1; i >= 0; i--)
        {
            CardInfo currentCard = _cardsInn[i];
            CardData cardData = currentCard.CardDataRef;
            if ((int)nuisanceToCheck != (int)cardData.BarIrritationCondition)
                continue;

            CardLeaveInn(currentCard);

            StartCoroutine(cardData.IrritationEffect.ActivateEffect(i));

            yield return new WaitForSeconds(0.2f);
        }
    }

    public CardInfo GetCardInBar()
    {
        return _cardsBar[Random.Range(0, _cardsBar.Count - 1)];
    }

    public void SwitchCard(CardInfo card, int index)
    {
        CardInfo temp = _cardsInn[index];

        //MoveCardToPoint(_cardsInn[index], card.transform.position, true);

        //MoveCardToPoint(card, temp.transform.position, false);

        int targetIndex = GetCardIndexInInn(card);
        _cardsInn[index] = card;
        _cardsInn[targetIndex] = temp;

        if (_cardsInn.Count > 0)
            CardHolderInn.Instance.ReplaceCardInOrder(temp.CardMovement);

    }

    public void SwitchCardFromBar(CardInfo card, int index, int cardIndex)
    {
        var pos = _cardsInn[index].transform.position;

        MoveCardToPoint(card, pos, true);

        _cardsBar[GetCardIndexInBar(card)] = null;
        _cardsInn.Add(card);
        CardLeaveInn(_cardsInn[cardIndex]);

        if (_cardsInn.Count > 0)
            CardHolderInn.Instance.ReplaceCardInOrder(card.CardMovement);

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
