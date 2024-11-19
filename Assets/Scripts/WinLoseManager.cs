using UnityEngine;


public class WinLoseManager : MonoBehaviour
{
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

    private void Win()
    {
        Debug.Log("Win");
    }

    private void Lose()
    {
        Debug.Log("Lose");
    }
}