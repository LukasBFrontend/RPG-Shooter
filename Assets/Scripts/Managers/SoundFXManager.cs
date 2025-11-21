using UnityEngine;
[System.Serializable]
public class SoundFXClip
{
    public AudioClip audioClip;
    [Range(0, 100)] public float volume;
}
public class SoundFXManager : Singleton<SoundFXManager>
{

    [SerializeField] private AudioSource soundFXObject;
    private AudioSource repeatSource = null;
    private float timer = 0f;
    private float repeatDelay = 0f;
    private int iterationsLeft = 0;

    void Update()
    {
        if (timer <= 0 && iterationsLeft > 0)
        {
            repeatSource.Play();
            timer = repeatSource.clip.length + repeatDelay;
            iterationsLeft--;
        }

        timer -= Time.deltaTime;
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = Mathf.Log10(volume / 100 + 1);
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        DontDestroyOnLoad(audioSource.gameObject);
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];
        audioSource.volume = Mathf.Log10(volume / 100 + 1);
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlaySoundFXtimes(AudioClip audioClip, Transform spawnTransform, float volume, int iterations, float delay)
    {
        iterationsLeft = iterations;
        repeatDelay = delay;
        repeatSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        repeatSource.clip = audioClip;
        repeatSource.volume = Mathf.Log10(volume / 100 + 1);

        float clipLength = repeatSource.clip.length;

        Destroy(repeatSource.gameObject, iterations * (clipLength + delay));
    }

    public void PlaySoundFXRepeat(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
    }
}
