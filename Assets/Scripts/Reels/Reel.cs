using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Reel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image[] symbolSlots;

    [Header("Animation")]
    [SerializeField] private float symbolSpacing = 105f;
    [SerializeField] private float spinSpeed = 900f;
    [SerializeField] private float accelerationTime = 0.25f;
    [SerializeField] private float stopDecelerationPower = 0.25f;

    public bool IsSpinning { get; private set; }

    private SymbolData[] availableSymbols;

    // stores symbols, randomizes starting display
    public void Initialize(SymbolData[] symbols)
    {
        availableSymbols = symbols;
        RandomizeSymbols();
    }

    // spins reel to result
    public void SpinTo(SymbolData result, float spinDuration)
    {
        if (IsSpinning ||
            result == null ||
            availableSymbols == null ||
            availableSymbols.Length == 0)
        {
            return;
        }

        StartCoroutine(SpinRoutine(result, spinDuration));
    }

    // handles acceleration, spinning and deceleration to the final result
    private IEnumerator SpinRoutine(SymbolData result, float spinDuration)
    {
        IsSpinning = true;

        float elapsed = 0f;

        // accelerates to full speed and stays there
        while (elapsed < spinDuration)
        {
            float accelerationProgress = Mathf.Clamp01(elapsed / accelerationTime);

            float speedMultiplier = Mathf.SmoothStep(0f, 1f, accelerationProgress);

            MoveSymbols(spinSpeed * speedMultiplier * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Image landingSlot = FindNextLandingSlot();
        landingSlot.sprite = result.Sprite;

        float remainingDistance = landingSlot.rectTransform.anchoredPosition.y;

        float totalStopDistance = remainingDistance;

        // continues from full speed before rapidly decelerating toward the result
        while (remainingDistance > 0f)
        {
            float distanceRatio = Mathf.Clamp01(remainingDistance / totalStopDistance);

            float currentSpeed = spinSpeed * Mathf.Pow(distanceRatio, stopDecelerationPower);

            float distanceThisFrame = Mathf.Min(currentSpeed * Time.deltaTime, remainingDistance);

            MoveSymbols(distanceThisFrame);
            remainingDistance -= distanceThisFrame;

            yield return null;
        }

        SnapSymbolsToGrid();

        IsSpinning = false;
    }

    // moves symbols downward by distance and recycles symbols too low
    private void MoveSymbols(float distance)
    {
        float loopHeight = symbolSpacing * symbolSlots.Length;

        float lowerRecycleLimit = -loopHeight / 2f;

        foreach (Image slot in symbolSlots)
        {
            RectTransform rectTransform = slot.rectTransform;
            Vector2 position = rectTransform.anchoredPosition;

            position.y -= distance;

            // reuses slots that leave the bottom
            while (position.y < lowerRecycleLimit)
            {
                position.y += loopHeight;
                slot.sprite = GetRandomUnweightedVisualSymbol().Sprite;
            }

            rectTransform.anchoredPosition = position;
        }
    }

    // finds the closest slot to the middle that is completely hidden above the reel window
    private Image FindNextLandingSlot()
    {
        Image closestHiddenSlot = null;
        float closestY = float.MaxValue;

        RectTransform reelRect = (RectTransform)transform;
        float reelTop = reelRect.rect.height * 0.5f;

        foreach (Image slot in symbolSlots)
        {
            float y = slot.rectTransform.anchoredPosition.y;
            float halfSymbolHeight = slot.rectTransform.rect.height * 0.5f;

            // only selects slots whose bottom edge is above the reel window
            if (y - halfSymbolHeight >= reelTop && y < closestY)
            {
                closestY = y;
                closestHiddenSlot = slot;
            }
        }

        return closestHiddenSlot;
    }

    // fixes minor positioning inaccuracies between spins
    private void SnapSymbolsToGrid()
    {
        foreach (Image slot in symbolSlots)
        {
            RectTransform rectTransform = slot.rectTransform;
            Vector2 position = rectTransform.anchoredPosition;

            position.y = Mathf.Round(position.y / symbolSpacing) * symbolSpacing;

            rectTransform.anchoredPosition = position;
        }
    }

    // gives each symbol slot an unweighted random sprite
    private void RandomizeSymbols()
    {
        foreach (Image slot in symbolSlots)
        {
            slot.sprite = GetRandomUnweightedVisualSymbol().Sprite;
        }
    }

    // gets an unweighted random symbol for decoration only
    private SymbolData GetRandomUnweightedVisualSymbol()
    {
        int index = Random.Range(0, availableSymbols.Length);

        return availableSymbols[index];
    }
}