using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
    private List<CardInfo> _cardsQueue = new List<CardInfo>();


    [Header("Cards")]
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

    private void Start()
    {
        for (int i = 0; i < _cardsDeck.Count; i++)
        {
            GameObject go = Instantiate(_prefabCardVisual, _deckPos.transform.position, Quaternion.identity, _deckPos);
            //go.SetActive(false);

            if (go.TryGetComponent(out CardInfo card) != false)
                _cardsQueue.Add(card);
        }

        PhaseBar();
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
    public void PhaseBar()
    {
        while (_cardsBar.Count < 4)
        {
            int cardsInBar = _cardsBar.Count;
            CardInfo cardInfo = _cardsQueue[^1];
            _cardsQueue.RemoveAt(_cardsQueue.Count - 1);

            int rand = Random.Range(0, _cardsDeck.Count - 1);

            //add to bar
            CardInfo cardData = _cardsDeck[rand];
            _cardsBar.Add(cardData);

            //remove from deck
            _cardsDeck.RemoveAt(rand);

            cardInfo.CardDataRef = cardData.CardDataRef;
            // cardInfo.GetComponent<CardMovement>()
            cardInfo.transform.position = _cardBarTargetPos[cardsInBar].position;
            cardInfo.gameObject.SetActive(true);
        }

        for (int i = 0; i < _cardsBar.Count; i++)
        {
            //check all condition in bar
            if (_cardsBar[i].CardDataRef.InnIrritationCondition.IsIrritated(i) == false)
                continue;
            _cardsBar[i].CardDataRef.IrritationEffect.ActivateEffect(i);
        }
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

    public int GetNumberOfFood()
    {
        int count = 0;

        for (int i = 0; i < _cardsInn.Count; i++)
        {
            if (_cardsInn[i].CardDataRef.Consumable == Consumable.FOOD)
                count++;
        }
        return count;
    }
    public int GetNumberOfBeer()
    {
        int count = 0;

        for (int i = 0; i < _cardsInn.Count; i++)
        {
            if (_cardsInn[i].CardDataRef.Consumable == Consumable.BEER)
                count++;
        }
        return count;
    }

    public void SetCurrentCard(GameObject card)
    {
        CurrentCardDragging = card;
    }
    public void PhaseInn()
    {
        for (int i = 0; i < _cardsInn.Count; i++)
        {
            //check consequance in Inn
            if (_cardsInn[i].CardDataRef.InnIrritationCondition.IsIrritated(i) == false)
                continue;

            _cardsInn[i].CardDataRef.IrritationEffect.ActivateEffect(i);
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

    public void GoblinLeaveInn()
    {

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

    public void CheckNuisance(CardInfo card)
    {
        CardLeaveInn(card);

        NuisanceType nuisance = card.CardDataRef.Nuisance;

        for (int i = _cardsInn.Count - 1; i >= 0; i--)
        {
            CardData cardData = _cardsInn[i].CardDataRef;
            if (cardData.InnIrritationCondition.IsIrritated(i) == false)
                continue;

            cardData.IrritationEffect.ActivateEffect(i);
            CheckNuisance(_cardsInn[i]);
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
