using System;
using UnityEngine;
// using DG.Tweening;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class CardVisual : MonoBehaviour
{
    public Image CardImage => _cardImage;
    
    [Header("References")]
    [SerializeField] private Transform _shakeParent;
    [SerializeField] private Transform _tiltParent;
    [SerializeField] private Image _cardImage;
    [SerializeField] private Transform _shadowTransform;
    
    [Header("Follow Parameters")]
    [SerializeField] private float _followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] private float _rotationAmount = 20;
    [SerializeField] private float _rotationSpeed = 20;
    [SerializeField] private float _autoTiltAmount = 30;
    [SerializeField] private float _manualTiltAmount = 20;
    [SerializeField] private float _tiltSpeed = 20;


    [Header("Scale Parameters")]
    [SerializeField] private bool _scaleAnimations = true;
    [SerializeField] private float _scaleOnHover = 1.15f;
    [SerializeField] private float _scaleOnSelect = 1.25f;
    [SerializeField] private float _scaleTransition = .15f;
    [SerializeField] private Ease _scaleEase = Ease.OutBack;

    [Header("Select Parameters")]
    [SerializeField] private float _selectPunchAmount = 20;

    [Header("Hover Parameters")]
    [SerializeField] private float _hoverPunchAngle = 5;
    [SerializeField] private float _hoverTransition = .15f;

    [Header("Curve")]
    // [SerializeField] private CurveParameters curve;

    private CardMovement _parentCard;
    private Coroutine _pressCoroutine;
    private Transform _cardTransform;
    private Transform _startParent;
    private Vector3 _rotationDelta;
    private Vector3 _movementDelta;
    private int _savedIndex;
    private float _curveRotationOffset;
    private float _curveYOffset;
    private bool _initalize = false;
    private Vector3 _shadowLocalStartPos;
    
    public void Initialize(CardMovement target)
    {
        //Declarations
        _parentCard = target;
        _cardTransform = target.transform;
        _startParent = transform.parent;
        name = _parentCard.name + " Visual";
        _shadowLocalStartPos = _shadowTransform.localPosition;

        //Event Listening
        _parentCard.PointerEnterEvent.AddListener(PointerEnter);
        _parentCard.PointerExitEvent.AddListener(PointerExit);
        _parentCard.BeginDragEvent.AddListener(BeginDrag);
        _parentCard.EndDragEvent.AddListener(EndDrag);
        _parentCard.PointerDownEvent.AddListener(PointerDown);
        _parentCard.PointerUpEvent.AddListener(PointerUp);
        _parentCard.SelectEvent.AddListener(Select);

        _initalize = true;
    }

    void Update()
    {
        if (!_initalize || _parentCard == null) return;

        SmoothFollow();
        FollowRotation();
        
        if (_parentCard.IsEnlarge == true) return;
        
        CardTilt();
    }

    private void SmoothFollow()
    {
        Vector3 verticalOffset = (Vector3.up * (_parentCard.IsDragging ? 0 : _curveYOffset));
        transform.position = Vector3.Lerp(transform.position, _cardTransform.position + verticalOffset,
            _followSpeed * Time.deltaTime);
    }

    private void FollowRotation()
    {
        Vector3 movement = (transform.position - _cardTransform.position);
        _movementDelta = Vector3.Lerp(_movementDelta, movement, 25 * Time.deltaTime);
        
        Vector3 movementRotation = (_parentCard.IsDragging ? _movementDelta : movement) * _rotationAmount;
        _rotationDelta = Vector3.Lerp(_rotationDelta, movementRotation, _rotationSpeed * Time.deltaTime);
        
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y,
            Mathf.Clamp(_rotationDelta.x, -60, 60));
    }

    private void CardTilt()
    {
        _savedIndex = _parentCard.IsDragging ? 0 : 1;
        float sine = Mathf.Sin(Time.time + _savedIndex) * (_parentCard.IsDragging ? 0 : 1);
        float cosine = Mathf.Cos(Time.time + _savedIndex) * (_parentCard.IsDragging ? 0 : 1);

        float lerpX = Mathf.LerpAngle(_tiltParent.eulerAngles.x, sine * _autoTiltAmount, _tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(_tiltParent.eulerAngles.y, cosine * _autoTiltAmount, _tiltSpeed * Time.deltaTime);

        _tiltParent.eulerAngles = new Vector3(lerpX, lerpY, _tiltParent.eulerAngles.z);
    }

    private void Select(CardMovement card, bool state)
    {
        if (_parentCard.IsEnlarge == false) return;
        
        DOTween.Kill(2, true);
        float dir = state ? 1 : 0;
        _shakeParent.DOPunchPosition(_shakeParent.up * _selectPunchAmount * dir, _scaleTransition, 10, 1);
        _shakeParent.DOPunchRotation(Vector3.forward * (_hoverPunchAngle / 2), _hoverTransition, 20, 1).SetId(2);

        if (_scaleAnimations)
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);
    }
    
    private void BeginDrag(CardMovement card)
    {
        if (_scaleAnimations && _parentCard.IsEnlarge == false)
            transform.DOScale(_scaleOnSelect, _scaleTransition).SetEase(_scaleEase);

        _shadowTransform.DOLocalMoveY(_shadowLocalStartPos.y - 30, _scaleTransition).SetEase(_scaleEase);
        
        if (CardDraggedHandler.Instance == null)
        {
            Debug.LogWarning("CardDraggedHandler is missing from the scene.");
            return;
        }
        transform.SetParent(CardDraggedHandler.Instance.transform);
    }

    private void EndDrag(CardMovement card)
    {
        transform.DOScale(1, _scaleTransition).SetEase(_scaleEase);
        _shadowTransform.DOLocalMoveY(_shadowLocalStartPos.y, _scaleTransition).SetEase(_scaleEase);

        transform.SetParent(_startParent);
    }

    private void PointerEnter(CardMovement card)
    {
        if (_scaleAnimations && _parentCard.IsEnlarge == false)
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);

        DOTween.Kill(2, true);
        _shakeParent.DOPunchRotation(Vector3.forward * _hoverPunchAngle, _hoverTransition, 20, 1).SetId(2);
    }

    private void PointerExit(CardMovement card)
    {
        if (!_parentCard.WasDragged && _parentCard.IsEnlarge == false)
            transform.DOScale(1, _scaleTransition).SetEase(_scaleEase);
    }

    private void PointerUp(CardMovement card, bool longPress)
    {
        if (_scaleAnimations && _parentCard.IsEnlarge == false)
            transform.DOScale(longPress ? _scaleOnHover : _scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
    }

    private void PointerDown(CardMovement card)
    {
        if (_scaleAnimations && _parentCard.IsEnlarge == false)
            transform.DOScale(_scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
    }
}