using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHolderInn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static CardHolderInn Instance;

    [SerializeField] private Transform _parentCardInn;
    public bool IsEnteredInn { get; private set; }
    
    private List<Vector3> _cardInnTargetPos = new List<Vector3>();
    private List<CardMovement> _cardsInn = new List<CardMovement>();
    
    [SerializeField] private float _offsetPosYCardInn = 3f;
    [SerializeField] private float _topYLimit;

    private void Awake()
    {
        Instance = this;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsEnteredInn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsEnteredInn = false;
    }

    public void ReleaseCardOnIt(CardMovement card)
    {
        _cardsInn.Add(card);
        card.transform.SetSiblingIndex(0);
        card.CardVisual.transform.SetSiblingIndex(0);

        float totalHeight = _offsetPosYCardInn * _cardsInn.Count;
        if (totalHeight > _topYLimit)
        {
            _offsetPosYCardInn = _topYLimit / _cardsInn.Count;
        }

        for (int i = 0; i < _cardsInn.Count; i++)
        {
            _cardsInn[i].MoveToPoint(_parentCardInn.position + new Vector3(0, _offsetPosYCardInn * i, 0), true);
        }
        
        GameManager.Instance.PhaseInn();
    }
    
    private void OnDrawGizmos()
    {
        if (_parentCardInn == null) return;

        Vector3 start = _parentCardInn.position;
        Vector3 end = new Vector3(start.x, start.y + _topYLimit, start.z);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);
    }
}
