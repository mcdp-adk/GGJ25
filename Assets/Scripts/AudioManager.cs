using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusic; // ���뱳�����ֵ� AudioSource

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play(); // ���ű�������
        }
    }
}