using TMPro;
using UnityEngine;

public class ResultPopupController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text resultPopupText;

    public bool IsOpen => gameObject.activeSelf;

    // shows the popup for a normal winning spin
    public void ShowWin(int winAmount)
    {
        resultPopupText.text = $"You won\n{winAmount} credits!";
        gameObject.SetActive(true);
    }

    // shows the popup when the player reaches the jackpot target
    public void ShowJackpot(int credits)
    {
        resultPopupText.text =
            $"JACKPOT! You reached {credits} credits!\nPress OKAY to play again.";

        gameObject.SetActive(true);
    }

    // shows the popup when the player can no longer afford a spin
    public void ShowGameOver()
    {
        resultPopupText.text =
            "GAME OVER! Not enough credits.\nPress OKAY to play again.";

        gameObject.SetActive(true);
    }

    // closes the currently displayed popup
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}