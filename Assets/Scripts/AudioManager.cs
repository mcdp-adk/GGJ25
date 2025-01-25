using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusic; // Õœ»Î±≥æ∞“Ù¿÷µƒ AudioSource

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play(); // ≤•∑≈±≥æ∞“Ù¿÷
        }
    }
}