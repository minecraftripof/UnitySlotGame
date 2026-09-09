using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SymbolRainController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image symbolPrefab;
    [SerializeField] private RectTransform rainLayer;

    [Header("Rain Settings")]
    [SerializeField] private int symbolCount = 20;
    [SerializeField] private float spawnDuration = 0.8f;
    [SerializeField] private float minimumFallSpeed = 500f;
    [SerializeField] private float maximumFallSpeed = 800f;
    [SerializeField] private float minimumRotationSpeed = -180f;
    [SerializeField] private float maximumRotationSpeed = 180f;
    [SerializeField] private float minimumScale = 0.6f;
    [SerializeField] private float maximumScale = 1.1f;

    // starts raining copies of the winning symbol across the screen
    public void Play(Sprite winningSprite)
    {
        StartCoroutine(SpawnRain(winningSprite));
    }

    // spreads symbol spawning over a short period instead of creating them all at once
    private IEnumerator SpawnRain(Sprite winningSprite)
    {
        float delayBetweenSymbols = spawnDuration / symbolCount;

        for (int i = 0; i < symbolCount; i++)
        {
            SpawnSymbol(winningSprite);

            yield return new WaitForSeconds(delayBetweenSymbols);
        }
    }

    // creates one randomized size/location falling symbol above the screen
    private void SpawnSymbol(Sprite winningSprite)
    {
        Image symbol = Instantiate(symbolPrefab, rainLayer);
        symbol.sprite = winningSprite;
        symbol.raycastTarget = false;

        RectTransform symbolRect = symbol.rectTransform;

        float halfWidth = rainLayer.rect.width * 0.5f;
        float startY = rainLayer.rect.height * 0.5f + symbolRect.rect.height;

        symbolRect.anchoredPosition = new Vector2(Random.Range(-halfWidth, halfWidth), startY);

        float scale = Random.Range(minimumScale, maximumScale);
        symbolRect.localScale = Vector3.one * scale;

        float fallSpeed = Random.Range(minimumFallSpeed, maximumFallSpeed);

        float rotationSpeed = Random.Range(minimumRotationSpeed, maximumRotationSpeed);

        StartCoroutine(FallSymbol(symbol, fallSpeed, rotationSpeed));
    }

    // moves and rotates one symbol until it falls below the screen
    private IEnumerator FallSymbol(
        Image symbol,
        float fallSpeed,
        float rotationSpeed)
    {
        RectTransform symbolRect = symbol.rectTransform;

        float bottomLimit = -rainLayer.rect.height * 0.5f - symbolRect.rect.height;

        while (symbolRect.anchoredPosition.y > bottomLimit)
        {
            symbolRect.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

            symbolRect.Rotate(0f,0f, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(symbol.gameObject);
    }
}