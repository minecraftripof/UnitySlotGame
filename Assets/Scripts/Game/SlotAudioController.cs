using UnityEngine;

public class SlotAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sounds")]
    [SerializeField] private AudioClip leverPull;
    [SerializeField] private AudioClip reelStop;
    [SerializeField] private AudioClip normalWin;
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip jackpot;

    // plays the lever pull sound
    public void PlayLeverPull() { audioSource.PlayOneShot(leverPull); }

    // plays the reel stopping sound
    public void PlayReelStop() { audioSource.PlayOneShot(reelStop); }

    // plays the win sound
    public void PlayNormalWin() { audioSource.PlayOneShot(normalWin); }

    // plays the game over sound
    public void PlayGameOver() { audioSource.PlayOneShot(gameOver); }

    // plays the jackpot sound
    public void PlayJackpot() { audioSource.PlayOneShot(jackpot); }
}