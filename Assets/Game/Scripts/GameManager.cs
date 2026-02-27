using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioClip loopAudioClip;
    public AudioClip[] randomAudioClips;
    private AudioSource audioSource;
    private AudioSource audioSourceAmb;
    private float timer = 0f;
    private float nextPlayTime = 0f;

    private void Awake()
    {
        audioSourceAmb = gameObject.AddComponent<AudioSource>();
        audioSourceAmb.clip = loopAudioClip;
        audioSourceAmb.loop = true;
        audioSourceAmb.playOnAwake = true;
        audioSourceAmb.Play();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    public void PlayRandomAudio()
    {
        if (randomAudioClips != null && randomAudioClips.Length > 0)
        {
            int index = Random.Range(0, randomAudioClips.Length);
            audioSource.clip = randomAudioClips[index];
            audioSource.Play();
        }
    }
 
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextPlayTime)
        {
            PlayRandomAudio();
            nextPlayTime = timer + Random.Range(45f, 120f);
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}