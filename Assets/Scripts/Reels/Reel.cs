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
    [SerializeField] private float stopDuration = 0.12f;

    public bool IsSpinning { get; private set; }

    private SymbolData[] availableSymbols;

    // stores symbols, randomizes starting display
    public void Initialize(SymbolData[] symbols)
    {
        availableSymbols = symbols;
        RandomizeSymbols();
    }

    // spin reel to result
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

    // acceleration, spinning and slow to stop
    private IEnumerator SpinRoutine(SymbolData result, float spinDuration)
    {
        IsSpinning = true;

        float elapsed = 0f;

        // accelerate to speed and stay at speed
        while (elapsed < spinDuration)
        {
            float accelerationProgress =
                Mathf.Clamp01(elapsed / accelerationTime);

            float speedMultiplier =
                Mathf.SmoothStep(0f, 1f, accelerationProgress);

            MoveSymbols(
                spinSpeed *
                speedMultiplier *
                Time.deltaTime
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        Image landingSlot = FindNextLandingSlot();
        landingSlot.sprite = result.Sprite;

        float requiredDistance =
            landingSlot.rectTransform.anchoredPosition.y;

        float distanceAlreadyMoved = 0f;
        float stopElapsed = 0f;

        while (stopElapsed < stopDuration)
        {
            float progress =
                Mathf.Clamp01(stopElapsed / stopDuration);

            // ease-out to sharp stop
            float easedProgress =
                1f - Mathf.Pow(1f - progress, 4f);

            float targetTotalDistance =
                requiredDistance * easedProgress;

            float distanceThisFrame =
                targetTotalDistance - distanceAlreadyMoved;

            MoveSymbols(distanceThisFrame);

            distanceAlreadyMoved = targetTotalDistance;
            stopElapsed += Time.deltaTime;

            yield return null;
        }

        MoveSymbols(requiredDistance - distanceAlreadyMoved);
        SnapSymbolsToGrid();

        IsSpinning = false;
    }

    // move symbols downward by distance and recycle symbols too low
    private void MoveSymbols(float distance)
    {
        float loopHeight =
            symbolSpacing * symbolSlots.Length;

        float lowerRecycleLimit =
            -loopHeight / 2f;

        foreach (Image slot in symbolSlots)
        {
            RectTransform rectTransform = slot.rectTransform;
            Vector2 position = rectTransform.anchoredPosition;

            position.y -= distance;

            // reusing slots
            while (position.y < lowerRecycleLimit)
            {
                position.y += loopHeight;
                slot.sprite = GetRandomUnweightedVisualSymbol().Sprite;
            }

            rectTransform.anchoredPosition = position;
        }
    }

    // finds the closest slot to the middle that is completely hidden above the reel window.
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

            // slot is only eligible once its bottom edge is above the reel's top edge.
            if (y - halfSymbolHeight >= reelTop && y < closestY)
            {
                closestY = y;
                closestHiddenSlot = slot;
            }
        }

        return closestHiddenSlot;
    }

    // fixes minor inaccuracies between spinsA
    private void SnapSymbolsToGrid()
    {
        foreach (Image slot in symbolSlots)
        {
            RectTransform rectTransform = slot.rectTransform;
            Vector2 position = rectTransform.anchoredPosition;

            position.y =
                Mathf.Round(position.y / symbolSpacing) *
                symbolSpacing;

            rectTransform.anchoredPosition = position;
        }
    }

    // gives each symbol an unweighted random sprite
    private void RandomizeSymbols()
    {
        foreach (Image slot in symbolSlots)
        {
            slot.sprite = GetRandomUnweightedVisualSymbol().Sprite;
        }
    }

    // gets a true random sprite (only for decoration purposes)
    private SymbolData GetRandomUnweightedVisualSymbol()
    {
        int index = Random.Range(0, availableSymbols.Length);

        return availableSymbols[index];
    }
}