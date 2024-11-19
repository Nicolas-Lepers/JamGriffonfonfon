using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WinLoseManager : MonoBehaviour
{
    [SerializeField] private Transition _transition;
    [SerializeField] private float _waitTransitionTime;
    [SerializeField] private TMP_Text _winText;
    [SerializeField] private TMP_Text _loseText;
    
    [Header("Polish")]
    [SerializeField] private float _textMoveOffset;
    [SerializeField] private float _textMoveTime;
    [SerializeField] private Ease _textMoveEase;
    
    private void Start()
    {
        GameManager.Instance.OnWin += Win;
        GameManager.Instance.OnWin += Lose;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.OnWin -= Win;
        GameManager.Instance.OnWin -= Lose;
    }

    [ContextMenu("Win")]
    private void Win()
    {
        _winText.gameObject.SetActive(true);
        MoveText(_winText);
        StartCoroutine(WaitForTransition());
    }
    
    [ContextMenu("Lose")]
    private void Lose()
    {
        _loseText.gameObject.SetActive(true);
        MoveText(_loseText);
        StartCoroutine(WaitForTransition());
    }
    private void MoveText(TMP_Text text)
    {
        var currentY = text.transform.hierarchyCapacity;

        text.transform.position = new Vector3(_winText.transform.position.x, currentY + _textMoveOffset, _winText.transform.position.z);
        text.transform.DOMoveY(currentY, _textMoveTime).SetEase(_textMoveEase);
    }

    private IEnumerator WaitForTransition()
    {
        yield return new WaitForSeconds(_waitTransitionTime);
        _transition.SetTransition(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }
}