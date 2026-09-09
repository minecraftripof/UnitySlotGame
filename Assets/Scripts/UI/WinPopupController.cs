using TMPro;
using UnityEngine;

public class WinPopupController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text winPopupText;

    public bool IsOpen => gameObject.activeSelf;

    // shows the popup with the amount won
    public void Show(int winAmount)
    {
        winPopupText.text = $"You won\n{winAmount} credits!";
        gameObject.SetActive(true);
    }

    // closes the popup
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}