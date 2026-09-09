using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SlotMachineController slotMachine;
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private Button spinButton;

    // refreshes credits, bet and win information from the slot machine
    public void RefreshUI()
    {
        creditsText.text = $"Credits:\n{slotMachine.Credits}";
        betText.text = $"Bet:\n{slotMachine.BetAmount}";
        winText.text = $"Win:\n{slotMachine.LastWinAmount}";

        spinButton.interactable = slotMachine.CanSpin;
    }
}