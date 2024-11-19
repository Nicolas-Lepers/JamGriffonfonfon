using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
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

    private Canvas _canvas;
    private Image _imageComponent;
    private CardVisual _currentCardVisual;
    private bool _canBeSelected = true;
    
    void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _imageComponent = GetComponent<Image>();

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
    
    public void MoveToPoint(Vector3 point)
    {
        _canBeSelected = false;
        transform.DOMove(point, .5f).SetEase(Ease.OutBack).OnComplete(() => _canBeSelected = true);
    }
    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_canBeSelected == false) return;
        
        BeginDragEvent.Invoke(this);
        GameManager.Instance.SetCurrentCard(gameObject);

        // _canvas.GetComponent<GraphicRaycaster>().enabled = false;
        _imageComponent.raycastTarget = false;

        IsDragging = true;
        WasDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_canBeSelected == false) return;

        EndDragEvent.Invoke(this);
        
        if (CardHolderInn.Instance.IsEnteredInn)
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
        PointerEnterEvent.Invoke(this);
        IsHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
        IsHovering = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
