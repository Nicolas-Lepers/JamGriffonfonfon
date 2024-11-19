using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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
    [SerializeField] Transform _cardHotselTargetDefaultPos;
    [SerializeField] float _offsetPosCardHostel = 0.5f;

    [SerializeField] GameObject _prefabCardVisual;
    private List<CardInfo> _cardsQueue = new List<CardInfo>();


    [Header("Cards")]
    [SerializeField] List<CardData> _cardsHostel = new List<CardData>();
    [SerializeField] List<CardData> _cardsBar = new List<CardData>();
    [SerializeField] List<CardData> _cardsDeck = new List<CardData>();
    [SerializeField] List<CardData> _discardPile = new List<CardData>();

    public List<CardData> CardsHostel => _cardsHostel;
    public List<CardData> CardsBar => _cardsBar;
    public List<CardData> CardsDeck => _cardsDeck;


    [SerializeField] int _numnerCardInDiscardPileToLose = 8;
    [SerializeField] int _numberCardInHostelToWin = 7;

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

        if (_cardsHostel.Count < _numberCardInHostelToWin)
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
            Debug.Log("Condition for all cards");
        }
    }

    public void PhaseHostel()
    {
        for (int i = 0; i < _cardsHostel.Count; i++)
        {
            //check consequance in hostel
            Debug.Log("Condition for all cards");
        }
    }

    public void AddCardInHostel(CardData card)
    {
        _cardsHostel.Add(card);
    }

    public void AddCardInHostelAtIndex(CardData card, int index)
    {
        _cardsHostel.Insert(index, card);
    }
    public void AddCardToBar(CardData card)
    {
        _cardsBar.Add(card);
    }

    /// <summary>
    /// Remove one card in hostel to put in the bar
    /// </summary>
    public void ReplaceCardToBar(CardData card)
    {
        if (_cardsHostel.Contains(card) == false)
            return;

        _cardsHostel.Remove(card);
        _cardsBar.Add(card);
    }

    public void SwitchCard(CardData card, int index)
    {
        CardData temp = _cardsHostel[index];
        int targetIndex = GetCardIndexInHostel(temp);
        _cardsHostel[index] = card;
        _cardsHostel[targetIndex] = temp;
    }

    private int GetCardIndexInHostel(CardData card)
    {
        return _cardsHostel.IndexOf(card);
    }

    public CardData GetRandomCardInBar()
    {
        return _cardsBar[Random.Range(0, _cardsBar.Count)];
    }

}
