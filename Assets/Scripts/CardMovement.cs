using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    [Header("Visual")]
    [SerializeField] private CardVisual _cardVisualPrefab;

    [Header("Movement")]
    [SerializeField] private float _moveSpeedLimit = 50;

    [Header("Events")]
    [HideInInspector] public UnityEvent<CardMovement> PointerEnterEvent;
    [HideInInspector] public UnityEvent<CardMovement> PointerExitEvent;
    [HideInInspector] public UnityEvent<CardMovement, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<CardMovement> PointerDownEvent;
    [HideInInspector] public UnityEvent<CardMovement> BeginDragEvent;
    [HideInInspector] public UnityEvent<CardMovement> EndDragEvent;
    [HideInInspector] public UnityEvent<CardMovement, bool> SelectEvent;

    public CardVisual CardVisual => _currentCardVisual;
    public bool WasDragged { get; private set; }
    public bool IsHovering { get; private set; }
    public bool IsDragging { get; private set; }
    public bool CanBeSelected { get; set; }

    public bool CanBeSelectedManager => GameManager.Instance.CanSelectedCard; 

    public bool IsEnlarge { get; private set; }

    private Canvas _canvas;
    private Image _imageComponent;
    private CardVisual _currentCardVisual;
    private Vector3 _lastPos;
    private int _siblingIndex;
    private int _siblingIndexVisual;
    private Transform _lastParent;
    
    void Start()
    {
        // TODO remove when cards will be instantiated from the code
        CanBeSelected = true;
        
        _canvas = GetComponentInParent<Canvas>();
        _imageComponent = GetComponent<Image>();
        _lastParent = transform.parent;

        if (CardVisualHandler.Instance == null)
        {
            Debug.LogWarning("CardVisualHandler is missing from the scene.");
        }
        else
        {
            _currentCardVisual = Instantiate(_cardVisualPrefab, CardVisualHandler.Instance.transform);
            _currentCardVisual.Initialize(this);
        }
    }
    void Update()
    {
        ClampPosition();

        if (IsDragging)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 velocity = direction * Mathf.Min(_moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
            
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }
    
    public void MoveToPoint(Vector3 point, bool blockSelected)
    {
        CanBeSelected = false;
        transform.DOMove(point, .5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if(blockSelected == false)
                CanBeSelected = true;
        });
    }
    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CanBeSelected == false || CanBeSelectedManager == false) return;
        
        BeginDragEvent.Invoke(this);
        GameManager.Instance.SetCurrentCard(gameObject);

        if (IsEnlarge)
        {
            CheckCardInfo();
        }
        // _canvas.GetComponent<GraphicRaycaster>().enabled = false;
        _imageComponent.raycastTarget = false;

        IsDragging = true;
        WasDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CanBeSelected == false || CanBeSelectedManager == false) return;

        EndDragEvent.Invoke(this);
        
        if (CardHolderInn.Instance != null && CardHolderInn.Instance.IsEnteredInn)
        {
            CardHolderInn.Instance.ReleaseCardOnIt(this);
        }

        _canvas.GetComponent<GraphicRaycaster>().enabled = true;
        _imageComponent.raycastTarget = true;
        
        IsDragging = false;
        
        StartCoroutine(FrameWait());
        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
            WasDragged = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsEnlarge) return;

        PointerEnterEvent.Invoke(this);
        IsHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsEnlarge) return;

        print("pointer exit");
        PointerExitEvent.Invoke(this);
        IsHovering = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      
    }
    
    private void EnlargeCard()
    {
        if (IsDragging) return;
        
        IsEnlarge = true;
        _currentCardVisual.transform.DOScale(Vector3.one * 5f, 0.3f).SetEase(Ease.OutBack);
        transform.DOScale(Vector3.one * 15f, 0.3f).SetEase(Ease.OutBack);
        
        _siblingIndex = transform.GetSiblingIndex();
        _siblingIndexVisual = _currentCardVisual.transform.GetSiblingIndex();
        
        transform.SetSiblingIndex(100);
        CardVisual.transform.SetSiblingIndex(100);
        transform.SetParent(CardDraggedHandler.Instance.transform);
    }
    
    private void MoveCardToCenter()
    {
        if (IsDragging) return;

        _lastPos = transform.position;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
        worldCenter.z = 0; // Ensure the z position is 0 to keep it on the same plane
        transform.position = worldCenter;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CheckCardInfo();
    }

    private void CheckCardInfo()
    {
        if (IsEnlarge == false)
        {
            MoveCardToCenter();
            EnlargeCard();
        }
        else
        {
            transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            _currentCardVisual.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            transform.position = _lastPos;
            
            transform.SetSiblingIndex(_siblingIndex);
            CardVisual.transform.SetSiblingIndex(_siblingIndexVisual);
            
            transform.SetParent(_lastParent);
            
            IsEnlarge = false;
        }
    }
    
    public void SetNewParent(Transform parent)
    {
        _lastParent = parent;
    }
}
