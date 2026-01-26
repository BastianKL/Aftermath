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
        //Spiller atmosfære i loop lyd ved start
        audioSourceAmb = gameObject.AddComponent<AudioSource>();
        audioSourceAmb.clip = loopAudioClip;
        audioSourceAmb.loop = true;
        audioSourceAmb.playOnAwake = true;
        audioSourceAmb.Play();
        //Tilfældige lyde i et interval sættes til ikke at køre
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }
    //Afspiller en tilfældig lyd
    public void PlayRandomAudio()
    {
        if (randomAudioClips != null && randomAudioClips.Length > 0)
        {
            int index = Random.Range(0, randomAudioClips.Length);
            audioSource.clip = randomAudioClips[index];
            audioSource.Play();
        }
    }
    //Opdaterer timeren og afspiller en ny lyd indenfor intervallet
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