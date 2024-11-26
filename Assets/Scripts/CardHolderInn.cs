using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHolderInn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static CardHolderInn Instance;

    public bool IsEnteredInn { get; private set; }

    [SerializeField] private Image _innHighlightImage;
    [SerializeField] private Transform _parentCardInn;
    [SerializeField] private float _offsetPosYCardInn = 3f;
    [SerializeField] private float _topYLimit;

    private void Awake()
    {
        Instance = this;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsEnteredInn = true;
        
        if(GameManager.Instance.CurrentCardDragging != null && GameManager.Instance.CurrentCardDragging.GetComponent<CardMovement>().IsDragging)
            OnPointerEnterAnim();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsEnteredInn = false;
        
        ResetHighlightAnim();
    }

    public void ReleaseCardOnIt(CardMovement card)
    {
        GameManager.Instance.AddCardInInn(card.GetComponent<CardInfo>());

        ResetHighlightAnim();

        int cardCount = GameManager.Instance.CardsInn.Count;
        card.SetSiblingIndex(0,0);

        float totalHeight = _offsetPosYCardInn * cardCount;
        if (totalHeight > _topYLimit)
        {
            _offsetPosYCardInn = _topYLimit / cardCount;
        }

        for (int i = 0; i < cardCount; i++)
        {
            CardMovement cardMovement = GameManager.Instance.CardsInn[i].GetComponent<CardMovement>();
            Vector3 targetPos = _parentCardInn.position + new Vector3(0, _offsetPosYCardInn * i, 0);
            cardMovement.MoveToPoint(targetPos, true);
            card.SetNewBasePos(targetPos);
        }

        GameManager.Instance.PutCardInInn(card.GetComponent<CardInfo>());
    }

    public void ReplaceCardInOrder(CardMovement card)
    {
        int cardCount = GameManager.Instance.CardsInn.Count;
        card.SetSiblingIndex(0,0);

        float totalHeight = _offsetPosYCardInn * cardCount;
        if (totalHeight > _topYLimit)
        {
            _offsetPosYCardInn = _topYLimit / cardCount;
        }

        for (int i = 0; i < cardCount; i++)
        {
            CardMovement cardMovement = GameManager.Instance.CardsInn[i].GetComponent<CardMovement>();
            Vector3 targetPos = _parentCardInn.position + new Vector3(0, _offsetPosYCardInn * i, 0);
            cardMovement.MoveToPoint(targetPos, true);
            card.SetNewBasePos(targetPos);
        }
    }

    private void OnPointerEnterAnim()
    {
        _innHighlightImage.DOFade(1, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    
    private void ResetHighlightAnim()
    {
        _innHighlightImage.DOKill();
        _innHighlightImage.DOFade(0, 0.25f);
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
