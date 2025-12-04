using UnityEngine;
[System.Serializable]
public class SoundFXClip
{
    public AudioClip AudioClip;
    [Range(0, 100)] public float Volume;
}

public class SoundFXManager : Singleton<SoundFXManager>
{
    [SerializeField] AudioSource soundFXObject;
    AudioSource _repeatSource = null;
    float _timer = 0f;
    float _repeatDelay = 0f;
    int _iterationsLeft = 0;

    void Update()
    {
        if (_timer <= 0 && _iterationsLeft > 0)
        {
            _repeatSource.Play();
            _timer = _repeatSource.clip.length + _repeatDelay;
            _iterationsLeft--;
        }

        _timer -= Time.deltaTime;
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource _audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        _audioSource.clip = audioClip;
        _audioSource.volume = Mathf.Log10(volume / 100 + 1);
        _audioSource.Play();

        float _clipLength = _audioSource.clip.length;

        DontDestroyOnLoad(_audioSource.gameObject);
        Destroy(_audioSource.gameObject, _clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int _rand = Random.Range(0, audioClip.Length);

        AudioSource _audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        _audioSource.clip = audioClip[_rand];
        _audioSource.volume = Mathf.Log10(volume / 100 + 1);
        _audioSource.Play();

        float _clipLength = _audioSource.clip.length;

        Destroy(_audioSource.gameObject, _clipLength);
    }

    public void PlaySoundFXtimes(AudioClip audioClip, Transform spawnTransform, float volume, int iterations, float delay)
    {
        _iterationsLeft = iterations;
        _repeatDelay = delay;
        _repeatSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        _repeatSource.clip = audioClip;
        _repeatSource.volume = Mathf.Log10(volume / 100 + 1);

        float _clipLength = _repeatSource.clip.length;

        Destroy(_repeatSource.gameObject, iterations * (_clipLength + delay));
    }

    public void PlaySoundFXRepeat(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource _audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        _audioSource.clip = audioClip;
        _audioSource.volume = volume;
        _audioSource.loop = true;
        _audioSource.Play();
    }
}
