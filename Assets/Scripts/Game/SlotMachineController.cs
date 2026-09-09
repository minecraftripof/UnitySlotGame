using System.Collections;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Reel[] reels;
    [SerializeField] private SymbolData[] symbols;
    [SerializeField] private SlotUI slotUI;
    [SerializeField] private ResultPopupController resultPopup;
    [SerializeField] private SymbolRainController symbolRain;
    [SerializeField] private SlotAudioController slotAudio;

    [Header("Spin Timing")]
    [SerializeField] private float firstReelSpinDuration = 1.2f;
    [SerializeField] private float reelStopDelay = 0.3f;

    [Header("Game Settings")]
    [SerializeField] private int startingCredits = 100;
    [SerializeField] private int betAmount = 10;
    [SerializeField] private int jackpotCredits = 250;

    public int Credits { get; private set; }
    public int LastWinAmount { get; private set; }
    public int BetAmount => betAmount;
    public bool IsSpinning => IsAnyReelSpinning();

    public bool CanSpin =>
        Credits >= betAmount &&
        !IsSpinning &&
        !resultPopup.IsOpen &&
        !gameEnded;

    private SymbolData[] currentResults;
    private bool gameEnded;

    // initializes credits and every reel with the set of symbols available in the game
    private void Start()
    {
        if (symbols == null || symbols.Length == 0)
        {
            Debug.LogError("No symbols have been configured for the slot machine.");
            return;
        }

        currentResults = new SymbolData[reels.Length];

        foreach (Reel reel in reels)
        {
            reel.Initialize(symbols);
        }

        ResetGame();
    }

    // creates reels' outcomes, deducts the bet and starts spin animation
    public void Spin()
    {
        if (!CanSpin) return;

        Credits -= betAmount;
        LastWinAmount = 0;

        for (int i = 0; i < reels.Length; i++)
        {
            SymbolData result = GetRandomWeightedSymbol();
            currentResults[i] = result;

            // later reels spin slightly longer so they stop in succession
            float spinDuration = firstReelSpinDuration + (reelStopDelay * i);

            reels[i].SpinTo(result, spinDuration);
        }

        // refreshes after the reels start so the lever is immediately disabled
        slotUI.RefreshUI();

        StartCoroutine(EvaluateSpinWhenFinished());
    }

    // waits for all reels to stop before checking the final result
    private IEnumerator EvaluateSpinWhenFinished()
    {
        while (IsAnyReelSpinning()) { yield return null; }

        EvaluateResult();
    }

    // checks the spin result, applies payouts and handles end-game states
    private void EvaluateResult()
    {
        SymbolData firstResult = currentResults[0];
        bool isWin = true;

        for (int i = 1; i < currentResults.Length; i++)
        {
            if (currentResults[i] != firstResult)
            {
                isWin = false;
                break;
            }
        }

        if (isWin)
        {
            LastWinAmount = betAmount * firstResult.PayoutMultiplier;
            Credits += LastWinAmount;

            Debug.Log($"Win! {firstResult.SymbolName} x{currentResults.Length} " +
                      $"- payout: {LastWinAmount}, credits: {Credits}");

            // winning symbol rain appears alongside either winning popup
            symbolRain.Play(firstResult.Sprite);

            if (Credits >= jackpotCredits)
            {
                gameEnded = true;
                resultPopup.ShowJackpot(Credits);
                slotAudio.PlayJackpot();
            }
            else
            {
                resultPopup.ShowWin(LastWinAmount);
                slotAudio.PlayNormalWin();
            }
        }
        else
        {
            Debug.Log($"No win. Credits: {Credits}");

            if (Credits < betAmount)
            {
                gameEnded = true;
                resultPopup.ShowGameOver();
                slotAudio.PlayGameOver();
            }
        }

        slotUI.RefreshUI();
    }

    // acknowledges the current popup or restarts after an end-game popup
    public void CloseResultPopup()
    {
        resultPopup.Hide();

        if (gameEnded) { ResetGame(); }
        else { slotUI.RefreshUI(); }
    }

    // resets credits and state for a new game
    private void ResetGame()
    {
        Credits = startingCredits;
        LastWinAmount = 0;
        gameEnded = false;

        slotUI.RefreshUI();
    }

    // checks for any reel spinning
    private bool IsAnyReelSpinning()
    {
        foreach (Reel reel in reels)
        {
            if (reel.IsSpinning) { return true; }
        }

        return false;
    }

    // weighted random symbol selection (considers probability, use for outcomes)
    private SymbolData GetRandomWeightedSymbol()
    {
        int totalWeight = 0;

        foreach (SymbolData symbol in symbols)
        {
            // just in case a symbol was accidentally set to negative weight
            totalWeight += Mathf.Max(0, symbol.Weight);
        }

        if (totalWeight <= 0)
        {
            Debug.LogError("At least one symbol must have a positive RNG weight.");
            return symbols[0];
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        foreach (SymbolData symbol in symbols)
        {
            cumulativeWeight += Mathf.Max(0, symbol.Weight);

            if (randomValue < cumulativeWeight) { return symbol; }
        }

        // should never be reached, but ensures a valid return value
        return symbols[symbols.Length - 1];
    }
}