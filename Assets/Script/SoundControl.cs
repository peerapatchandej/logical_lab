using UnityEngine;

public class SoundControl : MonoBehaviour
{
    [SerializeField] private AudioClip click, success, error, win, lose, crash, broken;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(click);
    }

    public void PlaySuccess()
    {
        audioSource.PlayOneShot(success);
    }

    public void PlayError()
    {
        audioSource.PlayOneShot(error);
    }

    public void PlayLose()
    {
        audioSource.PlayOneShot(lose);
    }

    public void PlayWin()
    {
        audioSource.PlayOneShot(win);
    }

    public void PlayCrash()
    {
        audioSource.PlayOneShot(crash);
    }

    public void PlayBroken()
    {
        audioSource.PlayOneShot(broken);
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
