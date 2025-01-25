using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic; // Õœ»Î±≥æ∞“Ù¿÷µƒ AudioSource

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