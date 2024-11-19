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


    [SerializeField] List<Transform> _cardBarTargetPos = new List<Transform>();
    [SerializeField] Transform _cardHotselTargetDefaultPos;
    [SerializeField] float _offsetPosCardHostel = 0.5f;

    [Header("Cards")]
    [SerializeField] List<GameObject> _cardsHostel = new List<GameObject>();
    [SerializeField] List<GameObject> _cardsBar = new List<GameObject>();
    [SerializeField] List<GameObject> _cardsDeck = new List<GameObject>();

    public List<GameObject> CardsHostel => _cardsHostel;
    public List<GameObject> CardsBar => _cardsBar;
    public List<GameObject> CardsDeck => _cardsDeck;

    //lose condition = 8 cartes dans la défausse (parti)
    //win condition = 7 cartes + dans l'auberge
    //end condition = plus de cartes dans la pioche

    public bool _barPhase = true;

    private void Update()
    {
        RunGame();
    }

    private void RunGame()
    {
        if (_barPhase)
        {
            int cardsInBar = _cardsBar.Count;
            while (cardsInBar < 3)
            {
                int rand = Random.Range(0, _cardsDeck.Count);

                //add to bar
                _cardsBar.Add(CardsDeck[rand]);

                //remove from deck
                _cardsDeck.RemoveAt(rand);

                Debug.Log($"add card {cardsInBar}");
            }

            for (int i = 0; i < cardsInBar; i++)
            {
                //check all condition in bar
            }

        }
        else
        {
            for (int i = 0; i < _cardsHostel.Count; i++)
            {
                //check consequance in hostel

            }

        }
    }
    public void AddCardInHostel(GameObject card)
    {
        _cardsHostel.Add(card);
    }

    public void AddCardInHostelAtIndex(GameObject card, int index)
    {
        _cardsHostel.Insert(index, card);
    }
    public void AddCardToBar(GameObject card)
    {
        _cardsBar.Add(card);
    }

    /// <summary>
    /// Remove one card in hostel to put in the bar
    /// </summary>
    public void ReplaceCardToBar(GameObject card)
    {
        if (_cardsHostel.Contains(card) == false)
            return;

        _cardsHostel.Remove(card);
        _cardsBar.Add(card);
    }

    public void SwitchCard(GameObject card,int index)
    {
        var temp = _cardsHostel[index];
        int targetIndex = GetCardIndexInHostel(temp);
        _cardsHostel[index] = card;
        _cardsHostel[targetIndex] = temp;
    }

    private int GetCardIndexInHostel(GameObject card)
    {
        return _cardsHostel.IndexOf(card); 
    }

    public GameObject GetRandomCardInBar()
    {
        return _cardsBar[Random.Range(0, _cardsBar.Count)];
    }

}
