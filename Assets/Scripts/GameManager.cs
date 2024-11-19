using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            else
            {
                Debug.LogError("2 GameManager");
            }

            return _instance;
        }
    }

    [SerializeField] Transform _canvasRef;

    [SerializeField] Transform _discardPilePos;
    [SerializeField] Transform _deckPos;

    [SerializeField] List<Transform> _cardBarTargetPos = new List<Transform>();
    [SerializeField] Transform _cardInnTargetDefaultPos;
    [SerializeField] float _offsetPosCardInn = 0.5f;

    [SerializeField] GameObject _prefabCardVisual;
    private List<CardInfo> _cardsQueue = new List<CardInfo>();


    [Header("Cards")]
    [SerializeField] List<CardData> _cardsInn = new List<CardData>();
    [SerializeField] List<CardData> _cardsBar = new List<CardData>();
    [SerializeField] List<CardData> _cardsDeck = new List<CardData>();
    [SerializeField] List<CardData> _discardPile = new List<CardData>();

    public List<CardData> CardsInn => _cardsInn;
    public List<CardData> CardsBar => _cardsBar;
    public List<CardData> CardsDeck => _cardsDeck;


    [SerializeField] int _numnerCardInDiscardPileToLose = 8;
    [SerializeField] int _numberCardInInnToWin = 7;
    private int _beerCount = 0;
    private int _foodCount = 0;
    private int _beerNeighborCount = 0;
    private int _foodNeighborCount = 0;

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
            CardInfo cardInfo = _cardsQueue[_cardsQueue.Count - 1];
            _cardsQueue.RemoveAt(_cardsQueue.Count - 1);

            int rand = Random.Range(0, _cardsDeck.Count - 1);

            //add to bar
            CardData cardData = _cardsDeck[rand];
            _cardsBar.Add(cardData);

            //remove from deck
            _cardsDeck.RemoveAt(rand);

            cardInfo.CardDataRef = cardData;
            cardInfo.transform.position = _cardBarTargetPos[cardsInBar].position;
            cardInfo.gameObject.SetActive(true);
        }

        for (int i = 0; i < _cardsBar.Count; i++)
        {
            //check all condition in bar
            if (_cardsBar[i].InnIrritationCondition.IsIrritated(i))
            {
                _cardsBar[i].IrritationEffect.ActivateEffect(i);
            }
        }
    }

    public int GetNumberOfGolbin()
    {
        int count = 0;

        for (int i = 0; i < _cardsInn.Count; i++)
        {
            if (_cardsInn[i].IsGoblin)
                count++;
        }
        return count;
    }

    public int GetNumberOfFood()
    {
        int count = 0;

        for (int i = 0; i < _cardsInn.Count; i++)
        {
            if (_cardsInn[i].Consumable == Consumable.FOOD)
                count++;
        }
        return count;
    }
    public int GetNumberOfBeer()
    {
        int count = 0;

        for (int i = 0; i < _cardsInn.Count; i++)
        {
            if (_cardsInn[i].Consumable == Consumable.BEER)
                count++;
        }
        return count;
    }

    public void PhaseInn()
    {
        for (int i = 0; i < _cardsInn.Count; i++)
        {
            //check consequance in Inn
            if (_cardsInn[i].InnIrritationCondition.IsIrritated(i))
            {
                _cardsInn[i].IrritationEffect.ActivateEffect(i);
            }
        }
    }

    public void AddCardInInn(CardData card)
    {
        _cardsInn.Add(card);
    }

    public void AddCardInInnAtIndex(CardData card, int index)
    {
        _cardsInn.Insert(index, card);
    }
    public void AddCardToBar(CardData card)
    {
        _cardsBar.Add(card);
    }

    /// <summary>
    /// Remove one card in inn to put in the bar
    /// </summary>
    public void ReplaceCardToBar(CardData card)
    {
        if (_cardsInn.Contains(card) == false)
            return;

        _cardsInn.Remove(card);
        _cardsBar.Add(card);
    }

    public void CardLeaveInn(CardData card)
    {
        if (_cardsInn.Contains(card) == false) return;

        _cardsInn.Remove(card);
    }

    public void CardLeaveInnWithNeihgbourUp(CardData card, int nbNeihgbourUp)
    {
        if (_cardsInn.Contains(card) == false) return;

        int value = GetCardIndexInInn(card);

        for (int i = nbNeihgbourUp - 1; i >= 0; i--)
        {
            CardData cardNeihgbour = _cardsInn[value + i];
            CardLeaveInn(cardNeihgbour);
        }

        _cardsInn.Remove(card);
    }

    


    public void SwitchCard(CardData card, int index)
    {
        CardData temp = _cardsInn[index];
        int targetIndex = GetCardIndexInInn(temp);
        _cardsInn[index] = card;
        _cardsInn[targetIndex] = temp;
    }

    private int GetCardIndexInInn(CardData card)
    {
        return _cardsInn.IndexOf(card);
    }

    public CardData GetRandomCardInBar()
    {
        return _cardsBar[Random.Range(0, _cardsBar.Count)];
    }



}
