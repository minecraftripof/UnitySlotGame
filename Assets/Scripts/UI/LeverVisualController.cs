using System.Collections;
using UnityEngine;

public class LeverVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject leverVisual;
    [SerializeField] private GameObject leverPulledVisual;

    [Header("Animation")]
    [SerializeField] private float pulledDuration = 0.7f;

    private bool isAnimating;

    // swaps to the pulled lever sprite before returning to normal
    public void PlayPullAnimation()
    {
        if (isAnimating)
            return;

        StartCoroutine(PullAnimation());
    }

    // handles the timing and visual swap for the lever pull
    private IEnumerator PullAnimation()
    {
        isAnimating = true;

        leverVisual.SetActive(false);
        leverPulledVisual.SetActive(true);

        yield return new WaitForSeconds(pulledDuration);

        leverPulledVisual.SetActive(false);
        leverVisual.SetActive(true);

        isAnimating = false;
    }
}